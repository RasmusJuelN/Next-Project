import { HttpHeaders } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { catchError, interval, map, of, switchMap, tap, firstValueFrom, throwError, Subscription, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';
import { ApiService } from './api.service';
import { Role, User } from '../../shared/models/user.model';
import { LoginErrorCode, LoginResult } from '../../features/home/models/login.model';

interface AuthTokens {
  authToken: string;
  refreshToken: string;
}


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

  private baseUrl = environment.apiUrl;

  private tokenService = inject(TokenService);
  private apiService = inject(ApiService);

  // Private writable signals
  private _user = signal<User | null>(null);
  private _isOnline = signal<boolean>(true);

  // Public readonly signals
  public readonly isAuthenticated = computed(() => this._user() !== null);
  public readonly user = this._user.asReadonly();
  public readonly isOnline = this._isOnline.asReadonly();

  /**
   * How often (in ms) to retry checking server connectivity if offline.
   * For example, 5000 = 5 seconds
   */
  private retryInterval = 5000;
  private retrySubscription: Subscription | null = null;


  /**
   * Logs in with username/password.
   * - Sends `POST /auth` (form-encoded).
   * - On success: stores tokens, updates auth/role/online state.
   *
   * @returns Observable emitting API response or `false` on failure.
   */
  public login(userName: string, password: string): Observable<LoginResult> {
    const url = `${this.baseUrl}/auth`;
    const body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

    return this.apiService.post<AuthTokens>(url, body.toString(), undefined, headers).pipe(
      tap(({ authToken, refreshToken }) => {
        this.tokenService.setToken(authToken);
        this.tokenService.setRefreshToken(refreshToken);
        this._user.set(this.buildUserFromToken());
        this._isOnline.set(true);
      }),
      map(() => ({ success: true } as const)),
      catchError(err => {
      const code: LoginErrorCode =
        err.status === 0 ? 'NETWORK' :
        err.status === 401 ? 'INVALID_CREDENTIALS' :
        err.status === 500 ? 'SERVER' :
        'UNKNOWN';

      // Optional: only logout on non-401 cases
      if (code !== 'INVALID_CREDENTIALS') this.logout();

      return of({ success: false, code });
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
            this._isOnline.set(true);
            this._user.set(this.buildUserFromToken());
          } else {
            this._isOnline.set(false);
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
          this._isOnline.set(true);
          // If the token is still valid, set user as authenticated
          if (this.tokenService.tokenExists() && !this.tokenService.isTokenExpired()) {
            this._user.set(this.buildUserFromToken());
          }
          this.stopRetrying();
        } else {
          this._isOnline.set(false);
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
   * Reads a specific claim from the decoded token.
   * @param key - Claim key to read.
   */
  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }
  
  /** Resets auth/role/online subjects to defaults. */
  private clearAuthState(): void {
    this._user.set(null);
    this._isOnline.set(true);
  }

private buildUserFromToken(): User | null {
  const id = this.getTokenInfo<string>('sub');
  const userName = this.getTokenInfo<string>('unique_name');
  const fullName = this.getTokenInfo<string>('name');
  const roleStr = this.getTokenInfo<string>('role');

  const role = this.mapToRoleEnum(roleStr);
  if (id && userName && fullName && role) {
    return { id, userName, fullName, role };
  }

  return null;
}
private mapToRoleEnum(value: string | null): Role | null {
  if (!value) return null;

  switch (value.toLowerCase()) {
    case Role.Student:
      return Role.Student;
    case Role.Teacher:
      return Role.Teacher;
    case Role.Admin:
      return Role.Admin;
    default:
      return null;
  }
}

}
