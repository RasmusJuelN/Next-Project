import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, catchError, map, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { TokenService } from './token.service';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticatedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private userRoleSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);
  userRole$ = this.userRoleSubject.asObservable();

  private baseUrl = environment.apiUrl;
  private tokenService = inject(TokenService);
  private apiService = inject(ApiService);

  constructor() {
    this.refreshAuthenticationState();
  }

  login(userName: string, password: string) {
    const url = `${this.baseUrl}/auth`;
    const body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded',
    });

    return this.apiService
      .post<{ access_token: string; token_type: string }>(url, body.toString(), undefined, headers)
      .pipe(
        tap((response) => {
          if (response.access_token) {
            this.tokenService.setToken(response.access_token);
            this.isAuthenticatedSubject.next(true);
            this.userRoleSubject.next(this.getUserRole()); // Set role dynamically
          }
        }),
        map(() => true),
        catchError((error) => {
          console.error('Authentication failed:', error);
          this.isAuthenticatedSubject.next(false);
          this.userRoleSubject.next(null);
          return of(false);
        })
      );
  }

  logout(): void {
    this.tokenService.clearToken();
    this.isAuthenticatedSubject.next(false);
    this.userRoleSubject.next(null); // Clear role on logout
  }

  getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  getUserRole(): string | null {
    return this.getTokenInfo<string>('scope');
  }

  refreshAuthenticationState(): void {
    const tokenExists = this.tokenService.tokenExists();
    const tokenValid = !this.tokenService.isTokenExpired();

    this.isAuthenticatedSubject.next(tokenExists && tokenValid);

    if (tokenExists && tokenValid) {
      const role = this.getUserRole();
      this.userRoleSubject.next(role); // Update role based on token
    } else {
      this.userRoleSubject.next(null); // Clear role if invalid
    }
  }

  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }
}
