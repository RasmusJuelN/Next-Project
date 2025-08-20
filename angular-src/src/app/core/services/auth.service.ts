import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, catchError, interval, map, of, switchMap, tap, timer, firstValueFrom, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';
import { ApiService } from './api.service';
import { User } from '../../shared/models/user.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private userRoleSubject = new BehaviorSubject<string | null>(null);
  public userRole$ = this.userRoleSubject.asObservable();

  private isOnlineSubject = new BehaviorSubject<boolean>(true);
  public isOnline$ = this.isOnlineSubject.asObservable();

  private baseUrl = environment.apiUrl;

  private tokenService = inject(TokenService);
  private apiService = inject(ApiService);

  /**
   * How often (in ms) to retry checking server connectivity if offline.
   * For example, 5000 = 5 seconds
   */
  private retryInterval = 5000;
  private retrySubscription: any;

  public login(userName: string, password: string) {
    const url = `${this.baseUrl}/auth`;
    const body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

    return this.apiService
      .post<{ authToken: string, refreshToken: string }>(url, body.toString(), undefined, headers)
      .pipe(
        tap((response) => {
          if (response.authToken && response.refreshToken) {
            this.tokenService.setToken(response.authToken);
            this.tokenService.setRefreshToken(response.refreshToken);
            this.isAuthenticatedSubject.next(true);
            this.userRoleSubject.next(this.getUserRole());
            this.isOnlineSubject.next(true);
          }
        }),
        catchError((err) => {
          console.error('Authentication failed:', err);
          this.logout();
          return of(false);
        })
      );
  }

public refreshToken() {
  const refreshToken = this.tokenService.getRefreshToken();
  const expiredToken = this.tokenService.getToken();
  if (!refreshToken || !expiredToken) {
    this.logout();
    return throwError(() => new Error('No tokens to refresh'));
  }

  const url = `${this.baseUrl}/auth/refresh`;
  const headers = new HttpHeaders({
    'Authorization': `Bearer ${refreshToken}`,
    'Content-Type': 'application/json'
  });

  return this.apiService
    .post<{ authToken: string; refreshToken: string }>(url, expiredToken, undefined, headers)
    .pipe(
      tap((res) => {
        this.tokenService.setToken(res.authToken);
        this.tokenService.setRefreshToken(res.refreshToken);
      }),
      catchError((err) => {
        this.logout();
        return throwError(() => err);
      })
    );
}

  public logout(): void {
    this.tokenService.clearToken();
    this.tokenService.clearRefreshToken();
    this.clearAuthState();
    // Optionally stop any retry attempts if the user logs out
    this.stopRetrying();
  }

  /**
   * Called once on service construction. 
   * 1) Checks if token is still valid locally
   * 2) If valid, tries server connectivity
   * 3) If server is offline, attempts to retry instead of immediate logout
   */
  public initializeAuthState(): Promise<boolean> {
    const tokenExists = this.tokenService.tokenExists();
    const tokenValid = !this.tokenService.isTokenExpired();

    if (!tokenExists || !tokenValid) {
      this.logout();
      return Promise.resolve(false);
    }

    return firstValueFrom(
      this.checkServerConnection().pipe(
        tap((serverIsOnline) => {
          if (serverIsOnline) {
            this.isOnlineSubject.next(true);
            this.isAuthenticatedSubject.next(true);
            this.userRoleSubject.next(this.getUserRole());
          } else {
            this.isOnlineSubject.next(false);
            // Start retrying connectivity if needed
            this.startRetryingConnection();
          }
        })
      )
    );
  }

  /**
   * Performs a simple server "ping" to confirm connectivity.
   */
  private checkServerConnection() {
    return this.apiService.head<boolean>(`${this.baseUrl}/system/ping`).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }

  /**
   * Start a timer that periodically checks server connectivity.
   * If connectivity is restored AND the token is still valid, we
   * restore the "authenticated" state. If the token expires during
   * this offline period, we log out.
   */
  private startRetryingConnection() {
    // Avoid multiple subscriptions
    this.stopRetrying();

    this.retrySubscription = interval(this.retryInterval)
      .pipe(
        // Before each ping, check token validity
        tap(() => {
          if (!this.tokenService.tokenExists() || this.tokenService.isTokenExpired()) {
            // Token has expired while offline -> log out
            this.logout();
          }
        }),
        switchMap(() => this.checkServerConnection())
      )
      .subscribe((serverIsOnline) => {
        if (serverIsOnline) {
          this.isOnlineSubject.next(true);
          // If the token is still valid, set user as authenticated
          if (this.tokenService.tokenExists() && !this.tokenService.isTokenExpired()) {
            this.isAuthenticatedSubject.next(true);
            this.userRoleSubject.next(this.getUserRole());
          }
          // Stop further retries
          this.stopRetrying();
        } else {
          // Still offline
          this.isOnlineSubject.next(false);
        }
      });
  }

  /**
   * Stop any ongoing retry subscription, if present.
   */
  private stopRetrying() {
    if (this.retrySubscription) {
      this.retrySubscription.unsubscribe();
      this.retrySubscription = null;
    }
  }

  getUser(): User | null {
    const id = this.getTokenInfo<string>('sub');
    const userName = this.getTokenInfo<string>('unique_name');
    const fullName = this.getTokenInfo<string>('name');
    const role = this.getTokenInfo<string>('role');
    
    if (id && userName && fullName && role) {
      return { id, userName, fullName, role };
    }
    
    return null;
  }

  getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  getUserRole(): string | null {
    return this.getTokenInfo<string>('role');
  }

  /**
   * Helper to read a specific claim/key from the token.
   */
  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }

  private clearAuthState(): void {
    this.isAuthenticatedSubject.next(false);
    this.userRoleSubject.next(null);
    this.isOnlineSubject.next(true);
  }
}
