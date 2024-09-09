import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginPageService } from '../../services/login-page.service';
import { AuthService } from '../../services/auth/auth.service';


/**
 * Represents the login page component.
 */
@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent {
  private loginPageService = inject(LoginPageService);
  private authService = inject(AuthService)
  
  errorMessage: string | null = null;
  userName: string = '';
  password: string = '';
  
  loggedInAlready: boolean = false;
  activeQuestionnaireString: string | null = '';
  errorHasHapped: boolean = false;
  goToDashboard: boolean = false;
  goToActiveQuestionnaire: boolean = false;


  /**
   * Initializes the component and tries to redirect to the dashboard if the user is already logged in.
   */
  ngOnInit(): void {
    // Subscribe to the authentication status observable
    this.authService.isAuthenticated$.subscribe((isAuthenticated: boolean) => {
      this.loggedInAlready = isAuthenticated;

      if (this.loggedInAlready) {
        // If the user is logged in, handle the navigation
        this.loginPageService.handleLoggedInUser(this.goToDashboard, this.goToActiveQuestionnaire)
          .subscribe({
            next: ({ goToDashboard, goToActiveQuestionnaire, activeQuestionnaireString }) => {
              this.goToDashboard = goToDashboard;
              this.goToActiveQuestionnaire = goToActiveQuestionnaire;
              this.activeQuestionnaireString = activeQuestionnaireString;
            },
            error: err => this.errorMessage = 'Error initializing login page'
          });
      } else {
        // Reset the login page if the user is not logged in
        this.resetLoginPage();
      }
    });
  }

  /**
   * Handles form submission.
   * @param form - The form data.
   */
  onSubmit(form: any): void {
    if (form.valid) {
      this.loginPageService.login(this.userName, this.password).subscribe({
        error: err => {
          this.errorMessage = 'Login failed. Please check your credentials.';
          this.errorHasHapped = true;
        }
      });
    }
  }

  logout() {
    this.loginPageService.logout();
    this.loggedInAlready = false;
    this.goToDashboard = false;
    this.goToActiveQuestionnaire = false;
  }

  toDashboard() {
    this.resetLoginPage()
    this.loginPageService.router.navigate(['/dashboard']);
  }

  toActiveQuestionnaire(urlString: string) {
    this.resetLoginPage()
    this.loginPageService.router.navigate([`/answer/${urlString}`]);
  }

  private resetLoginPage(): void {
    // Clear form fields and reset any flags or messages
    this.userName = '';
    this.password = '';
    this.errorMessage = null;
    this.loggedInAlready = false;
    this.goToDashboard = false;
    this.goToActiveQuestionnaire = false;
    this.errorHasHapped = false;
    this.activeQuestionnaireString = null;
  }
}
