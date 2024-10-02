import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, of, throwError, BehaviorSubject } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { JWTTokenService } from './jwt-token.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  httpClient = inject(HttpClient);
  jwtTokenService = inject(JWTTokenService);
  router = inject(Router);

  // BehaviorSubject to track authentication state
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable(); // Expose as observable

  /**
   * Authenticates the user with the provided username and password.
   * This method sends a request to the backend and stores the token on success.
   * @param userName - The username of the user.
   * @param password - The password of the user.
   * @returns An Observable that emits either an access token or an error message.
   */
  loginAuthentication(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded'
    });
  
    let body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);
  
    return this.httpClient.post<{ access_token: string }>(`${environment.apiUrl}/auth`, body.toString(), { headers }).pipe(
      tap(response => {
        if (response && response.access_token) {
          this.jwtTokenService.setToken(response.access_token);
          this.isAuthenticatedSubject.next(true);
          console.log('Login successful, token saved.');
        } else {
          this.isAuthenticatedSubject.next(false);
          console.error('Login failed: No access token received.');
        }
      }),
      catchError(error => {
        console.error('Login error:', error.message || error);
        return throwError(() => new Error('Login failed. Please check your credentials or try again later.'));
      })
    );
  }

  /**
   * Logs out the user and clears the token.
   */
  logout(): void {
    this.jwtTokenService.clearToken();
    this.isAuthenticatedSubject.next(false); // Update auth state on logout
    this.router.navigate(['/']); // Redirect to login page
  }

  /**
   * Checks if the user has a valid token (exists and is not expired).
   * @returns true if the user is logged in, otherwise false.
   */
  hasValidToken(): boolean {
    const existsAndNotExpired = this.jwtTokenService.tokenExists() && !this.jwtTokenService.isTokenExpired();
    
    if (!existsAndNotExpired) {
      this.jwtTokenService.clearToken();
      // Check if isAuthenticatedSubject is initialized before calling .next()
      if (this.isAuthenticatedSubject) {
        this.isAuthenticatedSubject.next(false);
      } else {
        console.error('isAuthenticatedSubject is not initialized');
      }
    }
    
    return existsAndNotExpired;
  }

  /**
   * Checks if the user has a specific role based on the stored token.
   * @param role - The role to check.
   * @returns true if the user has the role, otherwise false.
   */
  hasRole(role: string): boolean {
    const userRole = this.getUserRole();
    return userRole === role;
  }

  /**
   * Checks if there is an active questionnaire for the user.
   * This method should make a request to the backend to verify if the user has an active questionnaire.
   * @returns An Observable that emits the active questionnaire status and URL string.
   */
  checkForActiveQuestionnaire(): Observable<{ hasActive: boolean, urlString: string }> {
    const role = this.getUserRole();
    const id = this.getUserId();
    
    if (!id || !role) {
      return of({ hasActive: false, urlString: '' });
    }

    return this.httpClient.get<{ hasActive: boolean, urlString: string }>(`${environment.apiUrl}/questionnaires/active/${role}/${id}`).pipe(
      catchError(error => {
        console.error('Error checking for active questionnaire', error);
        return of({ hasActive: false, urlString: '' });
      })
    );
  }

  /**
   * Retrieves the user ID from the stored token.
   * @returns The user ID or null if the token is invalid or not present.
   */
  getUserId(): string | null {
    const decodedToken = this.jwtTokenService.getDecodeToken();
    return decodedToken && decodedToken['sub'] ? decodedToken['sub'] : null;
  }

  /**
   * Retrieves the role of the user from the stored token.
   * @returns The user's role or null if the token is invalid or not present.
   */
  getUserRole(): string | null {
    const decodedToken = this.jwtTokenService.getDecodeToken();
    return decodedToken ? decodedToken['scope'] : null;
  }
}
