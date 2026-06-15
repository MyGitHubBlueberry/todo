import { inject, Injectable, signal } from "@angular/core";
import { SessionApiService } from "../api/session-api.service";
import { LoginDto, RegistrationDto, TokensDto } from "../api/types";
import { Observable, tap, throwError } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private readonly api = inject(SessionApiService);
  public readonly accessToken =
    signal<string | null>(localStorage.getItem('access_token'));
  private readonly refreshToken =
    signal<string | null>(localStorage.getItem('refresh_token'));

  public login(dto: LoginDto): Observable<TokensDto> {
    return this.api.login(dto).pipe(
      tap(tokens => this.saveTokens(tokens)));
  }

  public register(dto: RegistrationDto): Observable<TokensDto> {
    return this.api.register(dto).pipe(
      tap(tokens => this.saveTokens(tokens)));
  }

  public refresh(): Observable<TokensDto> {
    if (!this.accessToken() || !this.refreshToken()) {
      this.logout();

      return throwError(() => new Error('Cannot refresh: Tokens are missing.'));
    }

    var dto: TokensDto = {
      accessToken: this.accessToken()!,
      refreshToken: this.refreshToken()!
    };

    return this.api.refresh(dto).pipe(
      tap(tokens => this.saveTokens(tokens)));
  }

  public logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');

    this.accessToken.set(null);
    this.refreshToken.set(null);
  }

  private saveTokens(tokens: TokensDto) {
    localStorage.setItem('access_token', tokens.accessToken);
    localStorage.setItem('refresh_token', tokens.refreshToken);

    this.accessToken.set(tokens.accessToken);
    this.refreshToken.set(tokens.refreshToken);
  }
}
