import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { MockAuthService } from '../../services/mock-auth.service';

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

  errorMessage: string | null = null;
  userName: string = "";
  password: string = "";

  errorHasHapped: boolean = false;

  /**
   * Initializes the component and tries to redirect to the dashboard if the user is already logged in.
   */
  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      this.router.navigate(['/']);
    }
  }

  /**
   * Checks the login credentials and performs the login action.
   */
  checkLogin() {
    this.authService.loginAuthentication(this.userName, this.password).subscribe({
      next: response => {
        if ('access_token' in response) {
          this.router.navigate(['/']);
        } else if ('error' in response) {
          this.errorMessage = response.error;
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
}