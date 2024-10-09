import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { DataService } from '../../services/data/data.service';
import { Router } from '@angular/router';
import { catchError, map, mergeMap, Observable, of, throwError } from 'rxjs';
import { ErrorHandlingService } from '../../services/error-handling.service';
import { ActiveQuestionnaire } from '../../models/questionare';

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
    this.authService.isAuthenticated$.subscribe(isAuthenticated => {
      this.loggedInAlready = isAuthenticated;
      isAuthenticated ? this.handleLoggedInUser() : this.resetLoginPage();
    }, error => this.handleError('Error during authentication status check', error));
  }

  onSubmit(form: any): void {
    if (form.valid) {
      this.authService.loginAuthentication(this.userName, this.password)
        .pipe(
          mergeMap(response => this.handleLoginResponse(response)),
          catchError(error => this.handleError('Login failed', error))
        )
        .subscribe();
    }
  }

  private handleLoggedInUser(): void {
    const { id, role } = this.getUserDetails();
    if (id && role) {
      this.dataService.getActiveQuestionnaireById(id)
        .pipe(
          map(activeQuestionnaire => this.setRedirectionFlags(activeQuestionnaire, role)),
          catchError(error => this.handleError('Error handling logged in user', error))
        )
        .subscribe();
    } else {
      this.resetLoginPage();
    }
  }

  private handleLoginResponse(response: { access_token: string } | { error: string }): Observable<void> {
    if ('access_token' in response) {
      // Authentication successful, redirection is handled by handleLoggedInUser triggered by ngOnInit
      return of(void 0);
    }
  
    // Use handleError instead of handleLoginError
    this.handleError('Login failed. Please check your credentials.', new Error(response.error));
    return throwError(() => new Error(response.error || 'Unexpected login response'));
  }

  private setRedirectionFlags(activeQuestionnaire: ActiveQuestionnaire | null, role: string): void {
    this.goToActiveQuestionnaire = !!activeQuestionnaire;
    this.goToDashboard = role === 'admin' || role === 'teacher';
    this.activeQuestionnaireString = activeQuestionnaire ? activeQuestionnaire.id : null;
  }

  private getUserDetails(): { id: string | null; role: string | null } {
    return {
      id: this.authService.getUserId(),
      role: this.authService.getUserRole(),
    };
  }

  private handleError(errorMessage: string, error: any): Observable<never> {
    this.errorMessage = errorMessage;
    this.errorHandlingService.handleError(error, errorMessage);
    return throwError(() => new Error(errorMessage));
  }

  logout(): void {
    this.authService.logout();
    alert('Token deleted');
    this.resetLoginPage();
  }

  toDashboard(): void {
    this.navigateTo('/dashboard/nav');
  }

  toActiveQuestionnaire(urlString: string): void {
    this.navigateTo(`/answer/${urlString}`);
  }

  private navigateTo(route: string): void {
    this.resetLoginPage();
    this.router.navigate([route]);
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
}
