import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MockAuthService } from './auth/mock-auth.service';
import { LocalStorageService } from './misc/local-storage.service';
import { ErrorHandlingService } from './error-handling.service';
import { catchError, map, Observable } from 'rxjs';
import { AppAuthService } from './auth/app-auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginPageService {

  constructor(
    public router: Router,
    private authService: AppAuthService, // Use AuthService directly here
    private localStorageService: LocalStorageService,
    private errorHandlingService: ErrorHandlingService
  ) {}

  /**
   * Check if the user is already logged in by verifying the token in local storage.
   * @returns boolean indicating whether the user is already logged in.
   */
  checkIfLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

    /**
   * Handle user redirection based on their role and active questionnaire status.
   */
    handleLoggedInUser(goToDashboard: boolean, goToActiveQuestionnaire: boolean): Observable<{ goToDashboard: boolean, goToActiveQuestionnaire: boolean, activeQuestionnaireString: string | null }> {
      return new Observable(observer => {
        try {
          if(this.authService.hasRole('admin') || this.authService.hasRole('teacher')){
            goToDashboard = true;
          }
          
          
          const activeQuestionnaireId = this.authService.checkForActiveQuestionnaire().urlString;
          if (activeQuestionnaireId) {
            goToActiveQuestionnaire = true;
          }
      
          observer.next({ goToDashboard, goToActiveQuestionnaire, activeQuestionnaireString: activeQuestionnaireId });
          observer.complete();
        } catch (error) {
          this.errorHandlingService.handleError(error, 'Error handling logged in user');
          observer.error(error);
        }
      });
    }

  /**
   * Perform login action and handle navigation based on response.
   */
  login(userName: string, password: string): Observable<void> {
    return this.authService.login(userName, password).pipe(
      map(response => {
        if ('access_token' in response) {
          const checkAnyActiveQuestionnaire = this.authService.checkForActiveQuestionnaire();
          if (checkAnyActiveQuestionnaire.hasActive) {
            this.router.navigate([`/answer/${checkAnyActiveQuestionnaire.urlString}`]);
          } else {
            this.router.navigate(['/dashboard']);
          }
        } else if ('error' in response) {
          this.handleLoginError(new Error(response.error));
        }
      }),
      catchError(error => this.errorHandlingService.handleError(error, 'Login failed'))
    );
  }

  /**
   * Handle login errors.
   */
  private handleLoginError(error: Error): void {
    const message = 'Login failed. Please check your credentials.';
    this.errorHandlingService.handleError(error, message);
  }

  /**
   * Logout the user by removing the token and resetting state.
   */
  logout(): void {
    this.localStorageService.removeData('token');
    alert('Token deleted');
  }
}
