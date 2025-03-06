import { Injectable } from "@angular/core";
import { jwtDecode } from "jwt-decode";

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private decodedToken: { [key: string]: any } | null = null;
  private readonly tokenKey = 'token';

  setToken(token: string): void {
    if (token) {
      localStorage.setItem(this.tokenKey, token);
      this.decodedToken = this.decodeToken(token);
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
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

  tokenExists(): boolean {
    return !!this.getToken();
  }
}
