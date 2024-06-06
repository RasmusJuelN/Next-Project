import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MookAuthService {
  private mockToken = 'mock-jwt-token';

  loginAuthentication(userName: string, password: string): Observable<{ token: string } | { error: string }> {
    const premadeUsers = [
      { userName: "Max", password: "test1112" },
      { userName: "Nicklas", password: "test1212" },
      { userName: "Alexander", password: "test1234" }
    ];

    const matchedUser = premadeUsers.find(user => user.userName === userName && user.password === password);

    if (matchedUser) {
      return of({ token: this.mockToken }).pipe(
        tap(response => {
          console.log("Login success");
          localStorage.setItem('token', response.token);
        })
      );
    } else {
      return throwError(() => new Error('Login failed. Please check your credentials.')).pipe(
        tap(() => {
          console.log("Login failure");
        })
      );
    }
  }

  constructor() { }
}