import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import  {jwtDecode } from 'jwt-decode'


@Injectable({
  providedIn: 'root'
})
/**
 * Service responsible for handling authentication-related functionality.
 */
export class AuthService {
  httpClient = inject(HttpClient);
  private token = '';

  /**
   * Authenticates the user with the provided username and password.
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
    return this.httpClient.post<{ access_token: string }>(`${environment.apiUrl}/auth`, body.toString(), { headers: headers }).pipe(
      tap(response => {
        if (response && response.access_token) {
          this.token = response.access_token;
          localStorage.setItem('token', this.token);
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
   * Retrieves the user ID from the stored token.
   * @returns The user ID or null if the token is invalid or not present.
   */
  getUserId(){
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = this.decodeToken(token);
        return decodedToken.sub || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }
  
  /**
   * Retrieves the role of the user from the stored token.
   * @returns The user's role or null if the token is invalid or not present.
   */
  getRole(){
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = this.decodeToken(token);
        // Scope refers to the role of the user
        return decodedToken.scope || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }

  decodeToken(token: string): any {
    return jwtDecode(token);
  }

  getUserFromToken(token: string): { userId: number, role: string } | null {
    try {
      const decodedToken: any = this.decodeToken(token);
      const userId = decodedToken.sub;
      const role = decodedToken.scope;
      if (userId && role) {
        return { userId, role };
      }
      return null;
    } catch (error) {
      console.error('Invalid token', error);
      return null;
    }
  }
}