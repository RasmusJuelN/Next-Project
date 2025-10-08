export enum LoginErrorCode {
  InvalidCredentials = 'INVALID_CREDENTIALS',
  Network            = 'NETWORK',
  Forbidden          = 'FORBIDDEN',
  RateLimited        = 'RATE_LIMITED',
  BadRequest         = 'BAD_REQUEST',
  Server             = 'SERVER',
  Unavailable        = 'UNAVAILABLE',
  Timeout            = 'TIMEOUT',
  Unknown            = 'UNKNOWN',
}

export type LoginResult = { success: true } | { success: false; code: LoginErrorCode };