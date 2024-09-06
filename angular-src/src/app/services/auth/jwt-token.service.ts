import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { LocalStorageService } from '../misc/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class JWTTokenService {
  private jwtToken: string | null = null;
  private decodedToken: { [key: string]: any } | null = null;
  private readonly tokenKey = 'token';

  constructor(private localStorageService: LocalStorageService) {}

  setToken(token: string): void {
    if (token) {
      this.jwtToken = token;
      this.localStorageService.saveData(this.tokenKey, token);
      this.decodeToken();
    }
  }

  getToken(): string | null {
    if (!this.jwtToken) {
      this.jwtToken = this.localStorageService.getData(this.tokenKey);
    }
    return this.jwtToken;
  }

  decodeToken(): void {
    const token = this.getToken();
    if (token) {
      this.decodedToken = jwtDecode(token);
    }
  }

  getDecodeToken(): { [key: string]: any } | null {
    if (!this.decodedToken) {
      this.decodeToken();
    }
    return this.decodedToken;
  }

  isTokenExpired(): boolean {
    const decoded = this.getDecodeToken();
    if (!decoded) {
      return true; // Token is considered expired if it does not exist
    }
    const expiryTime = decoded['exp'];
    return expiryTime ? (1000 * expiryTime) - (new Date()).getTime() < 0 : true;
  }

  clearToken(): void {
    this.jwtToken = null;
    this.decodedToken = null;
    this.localStorageService.removeData(this.tokenKey);
  }

  tokenExists(): boolean {
    return !!this.getToken();
  }
}