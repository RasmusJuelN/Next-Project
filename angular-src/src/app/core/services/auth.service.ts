import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, catchError, interval, map, of, switchMap, tap, timer, firstValueFrom, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';
import { ApiService } from './api.service';
import { User } from '../../shared/models/user.model';

/**
 * Authentication service.
 *
 * Handles:
 * - Login/logout and token storage.
 * - Token refresh flow (throws if no tokens available).
 * - Online/offline server reachability with retry.
 * - Exposes auth state (`isAuthenticated$`), role (`userRole$`), and connectivity (`isOnline$`).
 */
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


  /**
   * Logs in with username/password.
   * - Sends `POST /auth` (form-encoded).
   * - On success: stores tokens, updates auth/role/online state.
   *
   * @returns Observable emitting API response or `false` on failure.
   */
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

  /**
   * Refreshes the access token using the refresh token.
   * - Sends `POST /auth/refresh` with `{ expiredToken }` body and `Authorization: Bearer <refreshToken>`.
   * - On success: updates stored tokens.
   * - If no tokens exist: logs out and **throws**.
   *
   * @returns Observable emitting refreshed tokens or error.
   */
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
    .post<{ authToken: string; refreshToken: string }>(url,   { expiredToken } , undefined, headers)
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

  /**
   * Logs the user out: clears tokens/state and stops offline retry loop.
   */
  public logout(): void {
    this.tokenService.clearToken();
    this.tokenService.clearRefreshToken();
    this.clearAuthState();
    this.stopRetrying();
  }

  /**
   * Initializes auth state on app start:
   * 1) Validates local token.
   * 2) Pings server for connectivity.
   * 3) If offline, starts periodic retries.
   *
   * @returns Promise resolving to `true` if authenticated, else `false`.
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
            this.startRetryingConnection();
          }
        })
      )
    );
  }

  /** Performs a `HEAD /system/ping` to confirm server connectivity. */
  private checkServerConnection() {
    return this.apiService.head<boolean>(`${this.baseUrl}/system/ping`).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }

  /**
   * Starts periodic connectivity checks while offline.
   * - Logs out if the token expires during offline period.
   * - Restores authenticated state once server is reachable and token is valid.
   */
  private startRetryingConnection() {
    // Avoid multiple subscriptions
    this.stopRetrying();

    this.retrySubscription = interval(this.retryInterval)
      .pipe(
        tap(() => {
          if (!this.tokenService.tokenExists() || this.tokenService.isTokenExpired()) {
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
          this.stopRetrying();
        } else {
          this.isOnlineSubject.next(false);
        }
      });
  }

  /** Stops any ongoing retry subscription. */
  private stopRetrying() {
    if (this.retrySubscription) {
      this.retrySubscription.unsubscribe();
      this.retrySubscription = null;
    }
  }

  /**
   * Builds a `User` model from token claims, if all are present.
   * @returns `{ id, userName, fullName, role }` or `null`.
   */
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

  /** Gets the current user's id (`sub` claim) or `null`. */
  getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  /** Gets the current user's role (`role` claim) or `null`. */
  getUserRole(): string | null {
    return this.getTokenInfo<string>('role');
  }

  /**
   * Reads a specific claim from the decoded token.
   * @param key - Claim key to read.
   */
  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }
  
  /** Resets auth/role/online subjects to defaults. */
  private clearAuthState(): void {
    this.isAuthenticatedSubject.next(false);
    this.userRoleSubject.next(null);
    this.isOnlineSubject.next(true);
  }
}
