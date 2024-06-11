import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { MockAuthService } from '../../services/mock-auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent {
  router = inject(Router);
  authService = inject(AuthService);

  errorMessage: string | null = null;
  userName: string = "";
  password: string = "";

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      this.router.navigate(['/']);
    }
  }

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
        console.log('Login failed', error);
        this.errorMessage = 'Login failed. Please check your credentials.';
      }
    });
  }

  onSubmit(form: any): void {
    if (form.valid) {
      this.checkLogin();
    }
  }
}