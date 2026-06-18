export interface LoginDto {
  login: string,
  password: string,
}

export interface RegistrationDto extends LoginDto { }

export interface TokensDto {
  accessToken: string,
  refreshToken: string,
}
