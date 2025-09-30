export type LoginErrorCode = 'INVALID_CREDENTIALS' | 'NETWORK' | 'SERVER' | 'UNKNOWN';
export type LoginResult = { success: true } | { success: false; code: LoginErrorCode };