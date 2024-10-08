import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { DataService } from '../../services/data/data.service';
import { Router } from '@angular/router';
import { catchError, map, mergeMap, Observable, of, throwError } from 'rxjs';
import { ErrorHandlingService } from '../../services/error-handling.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css'],
})
export class LoginPageComponent implements OnInit {
  private authService = inject(AuthService);
  private dataService = inject(DataService);
  private errorHandlingService = inject(ErrorHandlingService);
  private router = inject(Router);

  errorMessage: string | null = null;
  userName = '';
  password = '';
  loggedInAlready = false;
  activeQuestionnaireString: string | null = '';
  goToDashboard = false;
  goToActiveQuestionnaire = false;

  ngOnInit(): void {
    this.authService.isAuthenticated$.subscribe({
      next: (isAuthenticated: boolean) => {
        this.loggedInAlready = isAuthenticated;
        if (this.loggedInAlready) {
          this.handleLoggedInUser();
        } else {
          this.resetLoginPage();
        }
      },
      error: (error) => this.handleError('Error during authentication status check', error),
    });
  }

  private handleLoggedInUser(): void {
    const { id, role } = this.getUserDetails();

    if (!id || !role) {
      this.resetLoginPage();
      return;
    }

    this.dataService.checkForActiveQuestionnaire(id, role)
      .pipe(
        map(response => this.buildRedirectResponse(response, role)),
        catchError(error => this.handleError('Error handling logged in user', error))
      )
      .subscribe(({ goToDashboard, goToActiveQuestionnaire, activeQuestionnaireString }) => {
        this.goToDashboard = goToDashboard;
        this.goToActiveQuestionnaire = goToActiveQuestionnaire;
        this.activeQuestionnaireString = activeQuestionnaireString;
      });
  }

  onSubmit(form: any): void {
    if (form.valid) {
      this.authService.loginAuthentication(this.userName, this.password)
        .pipe(
          mergeMap(response => this.handleLoginResponse(response)),
          catchError(error => this.handleError('Login failed', error))
        )
        .subscribe({
          error: () => {
            this.errorMessage = 'Login failed. Please check your credentials.';
          },
        });
    }
  }

  private handleLoginResponse(response: { access_token: string } | { error: string }): Observable<void> {
    if ('access_token' in response) {
      const { id, role } = this.getUserDetails();

      if (id && role) {
        return this.dataService.checkForActiveQuestionnaire(id, role).pipe(
          map(activeQuestionnaireResponse => this.redirectUserBasedOnQuestionnaire(activeQuestionnaireResponse))
        );
      } else {
        return throwError(() => new Error('User ID or role is missing after login.'));
      }
    } else if ('error' in response) {
      this.handleLoginError(new Error(response.error));
      return throwError(() => new Error(response.error));
    }

    return throwError(() => new Error('Unexpected login response'));
  }

  private getUserDetails(): { id: string | null; role: string | null } {
    return {
      id: this.authService.getUserId(),
      role: this.authService.getUserRole(),
    };
  }

  private handleLoginError(error: Error): void {
    this.errorMessage = 'Login failed. Please check your credentials.';
    this.errorHandlingService.handleError(error, this.errorMessage);
  }

  private redirectUserBasedOnQuestionnaire(activeQuestionnaireResponse: { hasActive: boolean, urlString: string }): void {
    const route = activeQuestionnaireResponse.hasActive ? `/answer/${activeQuestionnaireResponse.urlString}` : '/dashboard';
    this.router.navigate([route]);
  }

  private buildRedirectResponse(response: { urlString: string }, role: string) {
    const goToActiveQuestionnaire = !!response.urlString;
    const goToDashboard = role === 'admin' || role === 'teacher';
    return { goToDashboard, goToActiveQuestionnaire, activeQuestionnaireString: response.urlString };
  }

  private handleError(errorMessage: string, error: any): Observable<never> {
    this.errorHandlingService.handleError(error, errorMessage);
    this.errorMessage = errorMessage;
    return throwError(() => new Error(errorMessage));
  }

  private resetLoginPage(): void {
    this.userName = '';
    this.password = '';
    this.errorMessage = null;
    this.loggedInAlready = false;
    this.goToDashboard = false;
    this.goToActiveQuestionnaire = false;
    this.activeQuestionnaireString = null;
  }

  private navigateTo(route: string): void {
    this.resetLoginPage();
    this.router.navigate([route]);
  }

  toDashboard(): void {
    this.navigateTo('/dashboard/nav');
  }

  toActiveQuestionnaire(urlString: string): void {
    this.navigateTo(`/answer/${urlString}`);
  }

  logout(): void {
    this.authService.logout();
    alert('Token deleted');
    this.resetLoginPage();
  }
}
