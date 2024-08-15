import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { LocalStorageService } from '../misc/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class JWTTokenService {
  private jwtToken: string | null = null;
  private decodedToken: { [key: string]: any } | null = null;
  private readonly tokenKey = 'token'; // Key used to store the token in local storage

  constructor(private localStorageService: LocalStorageService) {}

  /**
   * Set the JWT token and store it in local storage.
   */
  setToken(token: string): void {
    if (token) {
      this.jwtToken = token;
      this.localStorageService.saveData(this.tokenKey, token);
    }
  }

  /**
   * Decode the JWT token stored in the service.
   */
  decodeToken(): void {
    if (!this.jwtToken) {
      this.jwtToken = this.localStorageService.getData(this.tokenKey);
    }
    if (this.jwtToken) {
      this.decodedToken = jwtDecode(this.jwtToken);
    }
  }

  tokenExists(){
    return this.localStorageService.getToken();
  }

  /**
   * Get the decoded token.
   * @returns The decoded token.
   */
  getDecodeToken(): { [key: string]: any } | null {
    if (!this.decodedToken) {
      this.decodeToken();
    }
    return this.decodedToken;
  }

  /**
   * Get the user from the decoded token.
   * @returns The user name.
   */
  getUser(): string | null {
    const decoded = this.getDecodeToken();
    return decoded ? decoded['username'] : null;
  }

  
  /**
   * Get the expiry time from the decoded token.
   * @returns The expiry time.
   */
  getExpiryTime(): number | null {
    const decoded = this.getDecodeToken();
    return decoded ? decoded['userName'] : null;
  }

  /**
   * Check if the token is expired.
   * @returns True if the token is expired, otherwise false.
   */
  isTokenExpired(): boolean {
    const expiryTime: number | null = this.getExpiryTime();
    if (expiryTime) {
      return (1000 * expiryTime) - (new Date()).getTime() < 5000;
    }
    return false;
  }

  /**
   * Clear the token from the service and local storage.
   */
  clearToken(): void {
    this.jwtToken = null;
    this.decodedToken = null;
    this.localStorageService.removeData(this.tokenKey);
  }
}
