import { inject, Injectable, signal } from "@angular/core";
import { SessionApiService } from "../api/session-api.service";
import { LoginDto, RegistrationDto, TokensDto } from "../api/types";
import { Observable, tap } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private readonly api = inject(SessionApiService);
  public readonly accessToken =
    signal<string | null>(localStorage.getItem('access_token'));

  public login(dto: LoginDto): Observable<TokensDto> {
    return this.api.login(dto).pipe(
      tap(tokens => this.saveTokens(tokens)));
  }

  public register(dto: RegistrationDto): Observable<TokensDto> {
    return this.api.login(dto).pipe(
      tap(tokens => this.saveTokens(tokens)));
  }

  public refresh(dto: TokensDto): Observable<TokensDto> {
    return this.api.refresh(dto).pipe(
      tap(tokens => this.saveTokens(tokens)));
  }

  public logout() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');

    this.accessToken.set(null);
  }

  private saveTokens(tokens: TokensDto) {
    localStorage.setItem('access_token', tokens.accessToken);
    localStorage.setItem('refresh_token', tokens.refreshToken);

    this.accessToken.set(tokens.accessToken);
  }
}
