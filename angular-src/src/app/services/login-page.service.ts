import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageService } from './misc/local-storage.service';
import { ErrorHandlingService } from './error-handling.service';
import { catchError, map, mergeMap, Observable, of, throwError } from 'rxjs';
import { AuthService } from './auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginPageService {

  constructor(
    public router: Router,
    private authService: AuthService,
    private errorHandlingService: ErrorHandlingService
  ) {}

    /**
   * Handle user redirection based on their role and active questionnaire status.
   */
    handleLoggedInUser(
      goToDashboard: boolean,
      goToActiveQuestionnaire: boolean
    ): Observable<{ goToDashboard: boolean; goToActiveQuestionnaire: boolean; activeQuestionnaireString: string | null }> {
      return this.authService.checkForActiveQuestionnaire().pipe(
        map((activeQuestionnaireResponse) => {
          const activeQuestionnaireId = activeQuestionnaireResponse.urlString;
          goToActiveQuestionnaire = !!activeQuestionnaireId;
  
          if (this.authService.hasRole('admin') || this.authService.hasRole('teacher')) {
            goToDashboard = true;
          }
  
          return {
            goToDashboard,
            goToActiveQuestionnaire,
            activeQuestionnaireString: activeQuestionnaireId,
          };
        }),
        catchError((error) => {
          this.errorHandlingService.handleError(error, 'Error handling logged in user');
          return of({
            goToDashboard: false,
            goToActiveQuestionnaire: false,
            activeQuestionnaireString: null,
          });
        })
      );
    }

  /**
   * Perform login action and handle navigation based on response.
   */
  login(userName: string, password: string): Observable<void> {
    return this.authService.loginAuthentication(userName, password).pipe(
      mergeMap((response) => {
        if ('access_token' in response) {
          return this.authService.checkForActiveQuestionnaire().pipe(
            map((activeQuestionnaireResponse) => {
              if (activeQuestionnaireResponse.hasActive) {
                this.router.navigate([`/answer/${activeQuestionnaireResponse.urlString}`]);
              } else {
                this.router.navigate(['/dashboard']);
              }
            })
          );
        } else if ('error' in response) {
          this.handleLoginError(new Error(response.error));
          return throwError(() => new Error(response.error));
        }
        return throwError(() => new Error('Unexpected login response'));

      }),
      catchError((error) => {
        this.errorHandlingService.handleError(error, 'Login failed');
        return throwError(() => error); 
      })
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
    this.authService.logout()
    alert('Token deleted');
  }
}
