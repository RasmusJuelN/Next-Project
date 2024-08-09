import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { MockAuthService } from '../../services/mock-auth.service';
import { MockDataService } from '../../services/mock-data.service';

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
  router = inject(Router);
  authService = inject(MockAuthService); // Change this to AuthService when using it really
  dataService = inject(MockDataService);

  errorMessage: string | null = null;
  userName: string = "";
  password: string = "";

  errorHasHapped: boolean = false;
  loggedInAlready: boolean = false;
  goToDashboard: boolean = false;
  goToActiveQuestionnaire: boolean = false;
  activeQuestionnaireString:string = "";

  /**
   * Initializes the component and tries to redirect to the dashboard if the user is already logged in.
   */
  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      this.loggedInAlready = true;
      const role = this.authService.getRole();
      if (role && role === "admin") {
        this.goToDashboard = true;
      }
  
      const activeQuestionnaireId = this.dataService.getFirstActiveQuestionnaireId();
      if (activeQuestionnaireId) {
        this.goToActiveQuestionnaire = true;
        this.activeQuestionnaireString = activeQuestionnaireId;
      }
    }
  }


  
  /**
   * Checks the login credentials and performs the login action.
   */
  checkLogin() {
    this.authService.loginAuthentication(this.userName, this.password).subscribe({
      next: response => {
        if ('access_token' in response) {
          this.loggedInAlready = true;
          this.errorHasHapped = false;
          const checkAnyActiveQuestionnaire = this.authService.checkForActiveQuestionnaire();
          if (checkAnyActiveQuestionnaire.hasActive){
            this.router.navigate([`/answer/${checkAnyActiveQuestionnaire.urlString}`]);
          }
          else {
            this.router.navigate(['/dashboard']);
          }
  
        } else if ('error' in response) {
          this.errorMessage = response.error; // I dont know if i am going to use it here
          this.router.navigate(['/error'], { queryParams: { message: this.errorMessage } });
        }
      },
      error: error => {
        this.errorHasHapped = true;
        console.log('Login failed', error);
        this.errorMessage = 'Login failed. Please check your credentials.';
      }
    });
  }
  
  /**
   * A test funtion for the form submission.
   * @param form - The form data.
   */
  onSubmit(form: any): void {
    if (form.valid) {
      this.checkLogin();
    }
  }

  logout(){
    localStorage.removeItem('token');
    alert('Token deleted');
    this.loggedInAlready = false;
  }
  toDashboard(){
    this.router.navigate(['/dashboard']);
  }
  toActiveQuestionnaire(urlString: string) {
    this.router.navigate([`/answer/${urlString}`]);
  }
}