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
    // This token automatically assumes that the user is "Max" and is a teacher
    this.mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyIiwiZnVsbF9uYW1lIjoiTWF4Iiwic2NvcGUiOiJ0ZWFjaGVyIiwidXNlcm5hbWUiOiJNSiIsImV4cCI6MTYxNTE2MjY3MH0.4a8y5LBmXoe4Kqpfg-GSGslNvzMAf8avtLPrm5gEVAA';
  }

  /**
   * Authenticates the user login using mock data.
   * @param userName - The username of the user.
   * @param password - The password of the user.
   * @returns An Observable that emits an access token if the login is successful, or an error message if the login fails.
   */
  loginAuthentication(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    const premadeUsers = [
      { userName: "MJ", password: "Pa$$w0rd" }, // This user is a teacher
      { userName: "NH", password: "Pa$$w0rd" },
      { userName: "Alexander", password: "Pa$$w0rd" },
      { userName: "Johan", password: "Pa$$w0rd"}
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

  /**
   * Retrieves the user ID from the token stored in the local storage.
   * @returns The user ID if the token is valid, or null if the token is invalid or not found.
   */
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
  
  /**
   * Retrieves the role from the token stored in the local storage.
   * @returns The role if the token is valid, or null if the token is invalid or not found.
   */
  getRole(){
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        console.log(decodedToken);
        return decodedToken.scope || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }
}