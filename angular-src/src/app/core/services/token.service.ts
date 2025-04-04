import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private decodedToken: { [key: string]: any } | null = null;
  private readonly tokenKey = 'token';
  private readonly refreshTokenKey = 'refresh_token';

  setToken(token: string): void {
    if (token) {
      localStorage.setItem(this.tokenKey, token);
      this.decodedToken = this.decodeToken(token);
    }
  }

  setRefreshToken(refreshToken: string): void {
    localStorage.setItem(this.refreshTokenKey, refreshToken);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  private decodeToken(token: string): { [key: string]: any } | null {
    try {
      return jwtDecode(token);
    } catch {
      return null;
    }
  }

  getDecodedToken(): { [key: string]: any } | null {
    if (!this.decodedToken) {
      const token = this.getToken();
      if (token) {
        this.decodedToken = this.decodeToken(token);
      }
    }
    return this.decodedToken;
  }

  isTokenExpired(): boolean {
    const decoded = this.getDecodedToken();
    if (!decoded) return true;
    const expiryTime = decoded['exp'];
    return expiryTime ? 1000 * expiryTime - Date.now() < 0 : true;
  }

  clearToken(): void {
    localStorage.removeItem(this.tokenKey);
    this.decodedToken = null;
  }

  clearRefreshToken(): void {
    localStorage.removeItem(this.refreshTokenKey);
  }

  tokenExists(): boolean {
    return !!this.getToken();
  }

  refreshTokenExists(): boolean {
    return !!this.getRefreshToken();
  }
}
