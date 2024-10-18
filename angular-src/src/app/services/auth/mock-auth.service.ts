import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';
import { User } from '../../models/questionare';
import { LocalStorageService } from '../misc/local-storage.service';
import { JWTTokenService } from './jwt-token.service';
import { Router } from '@angular/router';
import { MockDbService } from '../mock/mock-db.service';

@Injectable({
  providedIn: 'root',
})
export class MockAuthService {
  private localStorageService = inject(LocalStorageService);
  private jwtTokenService = inject(JWTTokenService);
  private router = inject(Router);
  private mockDbService = inject(MockDbService);

  // Initialize isAuthenticatedSubject before constructor logic
  private isAuthenticatedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor() {
    this.mockDbService.loadInitialMockData();
    this.isAuthenticatedSubject.next(this.hasValidToken());
  }

  /**
   * Authenticates the user login using mock data from MockDbService.
   * @param userName - The username of the user.
   * @param password - The password of the user.
   * @returns An Observable that emits an access token if the login is successful, or an error message if the login fails.
   */
  loginAuthentication(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    const matchedUser = this.mockDbService.mockData.mockUsers.find(user => user.userName === userName);
    
    // Check if user is found and password matches (mock password check)
    if (matchedUser && password === 'Pa$$w0rd') {
      const token = this.generateMockToken(matchedUser);

      return of({ access_token: token }).pipe(
        tap(response => {
          this.jwtTokenService.setToken(response.access_token); // Store the token
          this.isAuthenticatedSubject.next(true); // Update authentication status
        })
      );
    } else {
      return throwError(() => new Error('Login failed. Please check your credentials.'));
    }
  }

  private generateMockToken(user: User): string {
    const header = {
      alg: 'HS256',
      typ: 'JWT',
    };
  
    const payload = {
      sub: user.id,
      full_name: user.fullName,
      scope: user.role,
      username: user.userName,
      exp: Math.floor(Date.now() / 1000) + 60 * 60, // 1-hour expiration
    };
  
    // Function to encode in base64 with UTF-8 support
    const base64UrlEncode = (obj: any) => {
      return btoa(encodeURIComponent(JSON.stringify(obj))
        .replace(/%([0-9A-F]{2})/g, (_, p1) => String.fromCharCode(parseInt(p1, 16))))
        .replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    };
  
    const encodedHeader = base64UrlEncode(header);
    const encodedPayload = base64UrlEncode(payload);
    const mockSignature = 'mock-signature';
  
    return `${encodedHeader}.${encodedPayload}.${mockSignature}`;
  }
  
  

  /**
   * Refreshes the user data based on the current token.
   */
  refreshUserData(): void {
    const decodedToken = this.jwtTokenService.getDecodeToken();
    if (decodedToken) {
      const userId = decodedToken['sub'];
      const user = this.mockDbService.mockData.mockUsers.find(u => u.id === userId);
      
      if (user) {
        const newToken = this.generateMockToken(user);
        this.jwtTokenService.setToken(newToken); // Refresh token
      }
    }
  }

  /**
   * Checks if the current user has a specific role.
   * @param role - The role to check against.
   * @returns True if the user has the specified role, false otherwise.
   */
  hasRole(role: string): boolean {
    const userRole = this.getUserRole();
    return userRole === role;
  }

  /**
   * Retrieves the user ID from the token stored in the JWTTokenService.
   * @returns The user ID if the token is valid, or null if the token is invalid or not found.
   */
  getUserId(): string | null {
    const decodedToken = this.jwtTokenService.getDecodeToken();
    return decodedToken && decodedToken['sub'] ? decodedToken['sub'] : null;
  }

  /**
   * Retrieves the role from the token stored in the JWTTokenService.
   * @returns The role if the token is valid, or null if the token is invalid or not found.
   */
  public getUserRole(): string | null {
    const decodedToken = this.jwtTokenService.getDecodeToken();
    return decodedToken ? decodedToken['scope'] : null;
  }

  /**
   * Logs out the user by clearing the token and updating the authentication status.
   */
  logout(): void {
    this.jwtTokenService.clearToken();
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/']); // Redirect to login page
  }

  /**
   * Checks if a valid token exists and is not expired.
   * @returns True if a valid token exists, false otherwise.
   */
  hasValidToken(): boolean {
    const existsAndNotExpired = this.jwtTokenService.tokenExists() && !this.jwtTokenService.isTokenExpired();

    if (!existsAndNotExpired) {
      this.jwtTokenService.clearToken();
      this.isAuthenticatedSubject.next(false); // Ensure auth status is updated
    }

    return existsAndNotExpired;
  }

  /**
   * Checks if the mock server connection is available.
   * @returns An observable that always returns true for mock purposes.
   */
  checkServerConnection(): Observable<boolean> {
    return of(true);
  }
}
