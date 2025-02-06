import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, catchError, interval, map, of, switchMap, takeUntil, tap, timer } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { TokenService } from './token.service';
import { ApiService } from './api.service';

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

  constructor() {
    this.initializeAuthState();
  }

  public login(userName: string, password: string) {
    const url = `${this.baseUrl}/auth`;
    const body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

    return this.apiService
      .post<{ access_token: string; token_type: string }>(url, body.toString(), undefined, headers)
      .pipe(
        tap((response) => {
          if (response.access_token) {
            this.tokenService.setToken(response.access_token);
            this.isAuthenticatedSubject.next(true);
            this.userRoleSubject.next(this.getUserRole());
            this.isOnlineSubject.next(true); // We just successfully hit the server
          }
        }),
        catchError((err) => {
          console.error('Authentication failed:', err);
          this.logout();
          return of(false);
        }),
        map(() => true)
      );
  }

  public logout(): void {
    this.tokenService.clearToken();
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
  private initializeAuthState(): void {
    const tokenExists = this.tokenService.tokenExists();
    const tokenValid = !this.tokenService.isTokenExpired();

    // If no local token or it's expired -> treat as logged out
    if (!tokenExists || !tokenValid) {
      this.logout();
      return;
    }

    // If we have a valid token, do an initial connectivity check
    this.checkServerConnection().subscribe((serverIsOnline) => {
      if (serverIsOnline) {
        // If server is reachable, mark user as authenticated
        this.isOnlineSubject.next(true);
        this.isAuthenticatedSubject.next(true);
        this.userRoleSubject.next(this.getUserRole());
      } else {
        // Server offline - set isOnline to false but do NOT logout
        this.isOnlineSubject.next(false);
        // Start retry mechanism
        this.startRetryingConnection();
      }
    });
  }

  /**
   * Performs a simple server "ping" to confirm connectivity.
   */
  private checkServerConnection() {
    return this.apiService.get<boolean>(`${this.baseUrl}/ping`).pipe(
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
  getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  getUserRole(): string | null {
    return this.getTokenInfo<string>('scope');
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
