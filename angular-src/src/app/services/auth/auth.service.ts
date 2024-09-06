import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  httpClient = inject(HttpClient);


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

    // URL-encoded form data
    let body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    // Post request to the backend API
    return this.httpClient.post<{ access_token: string }>(`${environment.apiUrl}/auth`, body.toString(), { headers }).pipe(
      tap(response => {
        if (response && response.access_token) {
          const token = response.access_token;
          localStorage.setItem('token', token);
          console.log('Login success');
        }
      }),
      catchError(error => {
        console.error('Login error', error);
        return throwError(() => new Error('Login failed. Please check your credentials.'));
      })
    );
  }

  /**
   * Checks if the user has a specific role based on the stored token.
   * @param role - The role to check.
   * @returns true if the user has the role, otherwise false.
   */
  hasRole(role: string): boolean {
    const userRole = this.getRole();
    return userRole === role;
  }

  /**
   * Checks if the user is logged in by verifying if a token exists in localStorage.
   * @returns true if logged in, otherwise false.
   */
  isLoggedIn(): boolean {
    return !!this.getDecodedToken();
  }

  /**
   * Checks if there is an active questionnaire for the user.
   * This method should make a request to the backend to verify if the user has an active questionnaire.
   * @returns An Observable that emits the active questionnaire status and URL string.
   */
  checkForActiveQuestionnaire(): Observable<{ hasActive: boolean, urlString: string }> {
    const role = this.getRole();
    const id = this.getUserId();
    
    if (!id) {
      return of({ hasActive: false, urlString: '' });
    }

    // Backend request to check for active questionnaires (example URL)
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
    const token = this.getDecodedToken();
    return token ? token.sub : null;
  }

  /**
   * Retrieves the role of the user from the stored token.
   * @returns The user's role or null if the token is invalid or not present.
   */
  getRole(): string | null {
    const token = this.getDecodedToken();
    return token ? token.scope : null;  // Scope usually contains the role
  }

  /**
   * Decodes the JWT token stored in localStorage.
   * @returns The decoded token object or null if token is invalid.
   */
  private getDecodedToken(): any | null {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        return jwtDecode(token);
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }
}
