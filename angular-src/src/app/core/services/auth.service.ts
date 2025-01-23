import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, catchError, map, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ApiService } from './api.service';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isAuthenticatedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private baseUrl = environment.apiUrl

  apiService = inject(ApiService)
  tokenService = inject(TokenService)
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();


  loginAuthentication(userName: string, password: string) {
    const url = `${this.baseUrl}/auth`;
    const body = new URLSearchParams();
    body.set('username', userName);
    body.set('password', password);

    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded',
    });

    return this.apiService
      .post<{ access_token: string; token_type: string }>(url, body.toString(), undefined, headers)
      .pipe(
        tap((response) => {
          if (response.access_token) {
            localStorage.setItem('access_token', response.access_token);
            this.isAuthenticatedSubject.next(true);
          }
        }),
        map(() => true),
        catchError((error) => {
          console.error('Authentication failed:', error);
          this.isAuthenticatedSubject.next(false);
          return of(false);
        })
      );
  }
  
  logout(): void {
    this.tokenService.clearToken();
    this.isAuthenticatedSubject.next(false);
  }

  getUserId(): string | null {
    return this.getTokenInfo<string>('sub');
  }

  getUserRole(): string | null {
    return this.getTokenInfo<string>('scope');
  }

  private getTokenInfo<T>(key: string): T | null {
    const decodedToken = this.tokenService.getDecodedToken();
    return decodedToken && key in decodedToken ? (decodedToken[key] as T) : null;
  }
  
}
