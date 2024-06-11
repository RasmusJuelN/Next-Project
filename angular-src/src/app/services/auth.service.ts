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

  loginAuthentication(userName: string, password: string): Observable<{ token: string } | { error: string }> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded'
    });

    // URL-encoded form data
    let body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    return this.httpClient.post<{ token: string }>(`${environment.apiUrl}/auth`, body.toString(), { headers: headers }).pipe(
      tap(response => {
        if (response && response.token) {
          this.token = response.token;
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

  getRole(){
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        return decodedToken.role || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }

}