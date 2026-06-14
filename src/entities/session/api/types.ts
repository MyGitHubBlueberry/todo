export interface LoginDto {
  login: string,
  password: string,
}

export interface RegistrationDto {
  login: string,
  password: string,
}

export interface TokensDto {
  accessToken: string,
  refreshToken: string,
}
