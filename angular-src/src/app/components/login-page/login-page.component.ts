import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginPageService } from '../../services/login-page.service';


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
  ngOnInit() {
    this.loggedInAlready = this.loginPageService.checkIfLoggedIn();
    if (this.loggedInAlready) {
      this.loginPageService.handleLoggedInUser(this.goToDashboard, this.goToActiveQuestionnaire).subscribe({
        next: ({ goToDashboard, goToActiveQuestionnaire, activeQuestionnaireString }) => {
          this.goToDashboard = goToDashboard;
          this.goToActiveQuestionnaire = goToActiveQuestionnaire;
          this.activeQuestionnaireString = activeQuestionnaireString;
        },
        error: err => this.errorMessage = 'Error initializing login page'
      });
    }
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
    this.loginPageService.router.navigate(['/dashboard']);
  }

  toActiveQuestionnaire(urlString: string) {
    this.loginPageService.router.navigate([`/answer/${urlString}`]);
  }
}
