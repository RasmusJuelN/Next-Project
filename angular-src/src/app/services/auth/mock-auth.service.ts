import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ActiveQuestionnaire, Question, User } from '../../models/questionare';
import { LocalStorageService } from '../misc/local-storage.service';
import { JWTTokenService } from './jwt-token.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class MockAuthService {
  private adminMockToken: string;
  private teacherMockToken: string;
  private studentMockToken: string;
  private localStorageService = inject(LocalStorageService);
  private jwtTokenService = inject(JWTTokenService);
  private router = inject(Router)

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable(); 
  

  constructor() {
    // This token assumes that the user is "Max" and is a teacher
    this.teacherMockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZnVsbF9uYW1lIjoiTWF4Iiwic2NvcGUiOiJ0ZWFjaGVyIiwidXNlcm5hbWUiOiJNSiIsImV4cCI6MTc2NzIyNTYwMH0.sK5gcVr4AZBccqR7sRzHmsqCTFL2H8YzPKRmruH77w0';
    this.adminMockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZnVsbF9uYW1lIjoiQWRtaW4iLCJzY29wZSI6ImFkbWluIiwidXNlcm5hbWUiOiJBZG1pbiIsImV4cCI6MTc2NzIyNTYwMH0.rm4eMTa8ZoRS0unm013ZsjCloZWwcy9bZ7kpOFmtFHQ';
    this.studentMockToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0IiwiZnVsbF9uYW1lIjoiSm9oYW4iLCJzY29wZSI6InN0dWRlbnQiLCJ1c2VybmFtZSI6IkpIIiwiZXhwIjoxNzY3MjI1NjAwfQ.3fSMXJMA3eDEs9X0TUNTkVRmeEU3w_6prcmOc9K9EIc"
  }

  refreshUserData(): void {
    const token = this.jwtTokenService.getDecodeToken();
    if (token) {
      const decodedToken: any = this.jwtTokenService.getDecodeToken();
      if (decodedToken) {
        const role = decodedToken['scope'];
        if (role === 'admin') {
          this.jwtTokenService.setToken(this.adminMockToken);
        } else if (role === 'teacher') {
          this.jwtTokenService.setToken(this.teacherMockToken);
        } else if (role === 'student') {
          this.jwtTokenService.setToken(this.studentMockToken);
        }
      }
    }
  }


  /**
   * Authenticates the user login using mock data.
   * @param userName - The username of the user.
   * @param password - The password of the user.
   * @returns An Observable that emits an access token if the login is successful, or an error message if the login fails.
   */
  loginAuthentication(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    const premadeUsers = [
      { userName: "Admin", password: "Pa$$w0rd", role: 'admin' },
      { userName: "MJ", password: "Pa$$w0rd", role: 'teacher' },
      { userName: "NH", password: "Pa$$w0rd", role: 'student' },
      { userName: "AS", password: "Pa$$w0rd", role: 'student' },
      { userName: "JW", password: "Pa$$w0rd", role: 'student' } 
    ];

    const matchedUser = premadeUsers.find(user => user.userName === userName && user.password === password);

    if (matchedUser) {
      let token = this.studentMockToken;
      switch (matchedUser.role) {
        case 'admin':
          token = this.adminMockToken;
          break;
        case 'student':
          token = this.studentMockToken;
          break;
        case 'teacher':
          token = this.teacherMockToken;
          break;
      }
      
      return of({ access_token: token }).pipe(
        tap(response => {
          console.log("Login success");
          this.jwtTokenService.setToken(response.access_token); // Use JWTTokenService to store the token
          this.isAuthenticatedSubject.next(true);
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

  checkServerConnection(): Observable<boolean> {
    return of(false)
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

  logout(): void {
    this.jwtTokenService.clearToken();
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/']); // Redirect to login page
  }

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

}
