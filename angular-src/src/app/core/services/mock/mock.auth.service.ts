import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, of, interval } from 'rxjs';
import { TokenService } from '../token.service';

/**
 * A mock AuthService to use in tests or dev without hitting real endpoints.
 * Provides the same interface & public methods as the real AuthService.
 */
@Injectable({
  providedIn: 'root',
})
export class MockAuthService {
  // Public Observables matching the real AuthService
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private userRoleSubject = new BehaviorSubject<string | null>(null);
  public userRole$ = this.userRoleSubject.asObservable();

  private isOnlineSubject = new BehaviorSubject<boolean>(true);
  public isOnline$ = this.isOnlineSubject.asObservable();

  private tokenService = inject(TokenService);

  private readonly TEST_USERS = [
    {
      userName: 'adminUser',
      password: 'password',
      userId: 'mockAdminId',
      role: 'admin',
    },
    {
      userName: 'teacherUser',
      password: 'password',
      userId: 'mockId12345',
      role: 'teacher',
    },
    {
      userName: 'studentUser',
      password: 'password',
      userId: 'mockStudentId',
      role: 'student',
    },
  ];


  constructor() {
    this.initializeAuthState();
  }


  public login(userName: string, password: string): Observable<boolean> {
    // Find a matching user in our test "database"
    const foundUser = this.TEST_USERS.find(
      (u) => u.userName === userName && u.password === password
    );

    if (foundUser) {
      // Create a fake token that includes the userId & role as the JWT payload
      const fakeToken = this.createFakeToken(foundUser.userId, foundUser.role, 3600);
      this.tokenService.setToken(fakeToken);

      // Update local auth state
      this.isAuthenticatedSubject.next(true);
      this.userRoleSubject.next(foundUser.role);
      this.isOnlineSubject.next(true);

      return of(true);
    } else {
      this.clearAuthState();
      return of(false);
    }
  }

  public logout(): void {
    this.tokenService.clearToken();
    this.clearAuthState();
  }

  public getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  public getUserRole(): string | null {
    return this.getTokenInfo<string>('scope');
  }

  /**
   * Called once in constructor to mimic real AuthService initialization.
   */
  private initializeAuthState(): void {
    const tokenExists = this.tokenService.tokenExists();
    const tokenValid = !this.tokenService.isTokenExpired();

    if (tokenExists && tokenValid) {
      // Pretend we are "online"
      this.isOnlineSubject.next(true);
      this.isAuthenticatedSubject.next(true);
      this.userRoleSubject.next(this.getUserRole());
    } else {
      this.clearAuthState();
    }
  }

  /**
   * Utility to create a fake JWT so TokenService can decode it.
   */
  private createFakeToken(userId: string, userRole: string, expiresInSeconds: number): string {
    const header = { alg: 'HS256', typ: 'JWT' };
    const payload = {
      sub: userId,
      scope: userRole,
      exp: Math.floor(Date.now() / 1000) + expiresInSeconds,
    };
    const base64UrlEncode = (obj: any) =>
      btoa(JSON.stringify(obj))
        .replace(/=/g, '')
        .replace(/\+/g, '-')
        .replace(/\//g, '_');

    return `${base64UrlEncode(header)}.${base64UrlEncode(payload)}.signature-placeholder`;
  }

  /**
   * Parse token to get a specific key from payload
   */
  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }

  /**
   * Clear all local auth-related subjects
   */
  private clearAuthState(): void {
    this.tokenService.clearToken();
    this.isAuthenticatedSubject.next(false);
    this.userRoleSubject.next(null);
    this.isOnlineSubject.next(true);
  }
}
