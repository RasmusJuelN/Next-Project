import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';
import {jwtDecode} from 'jwt-decode';


@Injectable({
  providedIn: 'root'
})
export class MockAuthService {
  private mockToken = '';

  constructor() {
    // This token automaticly assumes the the user is "Max" and is a teacher
    this.mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Ik1heCIsInJvbGUiOiJ0ZWFjaGVyIiwiaWF0IjoxNjE1MTU5MDcwLCJleHAiOjE2MTUxNjI2NzB9.58DRS8vsDQb4ZQrWFl3aVvz_wNR9fi-mx4u9EdMn6fM';
  }

  loginAuthentication(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    const premadeUsers = [
      { userName: "MJ", password: "Pa$$w0rd" }, // This user is a teacher
      { userName: "NH", password: "Pa$$w0rd" },
      { userName: "Alexander", password: "Pa$$w0rd" }
    ];

    const matchedUser = premadeUsers.find(user => user.userName === userName && user.password === password);

    if (matchedUser) {
      return of({ access_token: this.mockToken }).pipe(
        tap(response => {
          console.log("Login success");
          localStorage.setItem('token', response.access_token);
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
  
  getRole(){
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        console.log(decodedToken);
        return decodedToken.role || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }
}