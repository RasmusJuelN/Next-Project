import { inject, Injectable } from '@angular/core';
import { LocalStorageService } from '../misc/local-storage.service';
import { Router } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { MockAuthService } from './mock-auth.service';

@Injectable({
  providedIn: 'root'
})
export class AppAuthService {

  authService = inject(MockAuthService)
  router = inject(Router)

  /**
   * Authenticates the user.
   * @param userName - The username of the user.
   * @param password - The password of the user.
   * @returns An observable that emits an access token if the login is successful, or an error message if the login fails.
   */
  login(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    return this.authService.loginAuthentication(userName, password);
  }

  /**
   * Logs out the user by removing the token from local storage and redirecting to login.
   */
  logout(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  /**
   * Checks if the user is logged in by verifying the presence of a token.
   * @returns True if the user is logged in, false otherwise.
   */
  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  hasRole(role: string): boolean {
    return this.authService.hasRole(role);
  }

  /**
   * Gets the current user's role from the token.
   * @returns The role of the user (e.g., 'admin', 'student', 'teacher').
   */
  getRole(): string | null {
    return this.authService.getRole();
  }

  /**
   * Gets the current user's ID from the token.
   * @returns The ID of the user.
   */
  getUserId(): string | null {
    return this.authService.getUserId();
  }

  /**
   * Checks for an active questionnaire based on the user's role.
   * @returns An object containing `hasActive` (boolean) and `urlString` (string).
   */
  checkForActiveQuestionnaire(): { hasActive: boolean, urlString: string } {
    return this.authService.checkForActiveQuestionnaire();
  }
}
