import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';

/**
 * Token service.
 *
 * Handles storing, retrieving, decoding, and clearing JWT access & refresh tokens
 * from `localStorage`.
 *
 * Features:
 * - Encapsulates token storage keys.
 * - Provides helpers for checking expiration and existence.
 * - Caches decoded token for performance.
 *
 * Used by `AuthService` and HTTP interceptors.
 */
@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private decodedToken: { [key: string]: any } | null = null;
  private readonly tokenKey = 'token';
  private readonly refreshTokenKey = 'refresh_token';

  /**
   * Stores the JWT access token in localStorage and decodes it.
   * @param token - The JWT access token string.
   */
  setToken(token: string): void {
    if (token) {
      localStorage.setItem(this.tokenKey, token);
      this.decodedToken = this.decodeToken(token);
    }
  }

  /**
   * Stores the refresh token in localStorage.
   * @param refreshToken - The refresh token string.
   */
  setRefreshToken(refreshToken: string): void {
    localStorage.setItem(this.refreshTokenKey, refreshToken);
  }

  /** Retrieves the access token from localStorage. */
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  /** Retrieves the refresh token from localStorage. */
  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  /**
   * Decodes a JWT safely.
   * @param token - JWT token string.
   * @returns Decoded payload object or `null` if invalid.
   */
  private decodeToken(token: string): { [key: string]: any } | null {
    try {
      return jwtDecode(token);
    } catch {
      return null;
    }
  }
  /** Retrieves the cached decoded token (decodes if missing). */
  getDecodedToken(): { [key: string]: any } | null {
    if (!this.decodedToken) {
      const token = this.getToken();
      if (token) {
        this.decodedToken = this.decodeToken(token);
      }
    }
    return this.decodedToken;
  }

  /**
   * Checks whether the current token is expired.
   * Relies on the `exp` claim.
   * @returns `true` if expired or missing, otherwise `false`.
   */
  isTokenExpired(): boolean {
    const decoded = this.getDecodedToken();
    if (!decoded) return true;
    const expiryTime = decoded['exp'];
    return expiryTime ? 1000 * expiryTime - Date.now() < 0 : true;
  }

  /** Clears the access token and cached decoded token. */
  clearToken(): void {
    localStorage.removeItem(this.tokenKey);
    this.decodedToken = null;
  }

  /** Clears the refresh token. */
  clearRefreshToken(): void {
    localStorage.removeItem(this.refreshTokenKey);
  }

  /** Checks if an access token exists. */
  tokenExists(): boolean {
    return !!this.getToken();
  }

  /** Checks if a refresh token exists. */
  refreshTokenExists(): boolean {
    return !!this.getRefreshToken();
  }
}
