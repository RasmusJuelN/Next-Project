import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import  {jwtDecode } from 'jwt-decode'


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  httpClient = inject(HttpClient);
  private token = '';

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

  getUserId(){
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        return decodedToken.sub || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }
  
  // GetRole does not correctly as the token has yet to get role in token from backend
  getRole(){
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        // Scope refers to the role of the user
        return decodedToken.scope || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }

}