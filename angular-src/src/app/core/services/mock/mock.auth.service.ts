import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, of } from 'rxjs';
import { TokenService } from '../token.service';

@Injectable({
  providedIn: 'root',
})
export class MockAuthService {
  private isAuthenticatedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private userRoleSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);
  userRole$ = this.userRoleSubject.asObservable();

  private tokenService = inject(TokenService);

  constructor() {
    this.refreshAuthenticationState();
  }

  login(userName: string, password: string) {
    if (userName === 'test' && password === 'password') {
      const fakeToken = this.createFakeToken('mockId12345', 'admin', 3600);
      this.tokenService.setToken(fakeToken);

      this.isAuthenticatedSubject.next(true);
      this.userRoleSubject.next('admin'); // Set role dynamically
      return of(true);
    } else {
      this.isAuthenticatedSubject.next(false);
      this.userRoleSubject.next(null);
      return of(false);
    }
  }

  logout(): void {
    this.tokenService.clearToken();
    this.isAuthenticatedSubject.next(false);
    this.userRoleSubject.next(null); // Clear role on logout
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

  getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  getUserRole(): string | null {
    return this.getTokenInfo<string>('scope');
  }

  private createFakeToken(userId: string, userRole: string, expiresInSeconds: number): string {
    const header = {
      alg: 'HS256',
      typ: 'JWT',
    };
    const payload = {
      sub: userId,
      scope: userRole,
      exp: Math.floor(Date.now() / 1000) + expiresInSeconds,
    };
    const base64UrlEncode = (obj: any) => btoa(JSON.stringify(obj)).replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_');
    return `${base64UrlEncode(header)}.${base64UrlEncode(payload)}.signature-placeholder`;
  }

  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }
}
