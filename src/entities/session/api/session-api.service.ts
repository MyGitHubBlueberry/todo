import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { LoginDto, RegistrationDto, TokensDto } from "./types";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root' })
export class SessionApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = import.meta.env.NG_APP_API_URL + '/auth';

  public login(dto: LoginDto): Observable<TokensDto> {
    return this.http.post<TokensDto>(`${this.baseUrl}/login`, dto);
  }

  public register(dto: RegistrationDto): Observable<TokensDto> {
    return this.http.post<TokensDto>(`${this.baseUrl}/register`, dto);
  }

  public refresh(dto: TokensDto): Observable<TokensDto> {
    return this.http.post<TokensDto>(`${this.baseUrl}/refresh`, dto);
  }
}
