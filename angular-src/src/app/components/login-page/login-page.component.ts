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
  userName: string = "";
  password: string = "";
  loginService = inject(MockAuthService); // Use Mock for now
  router = inject(Router);
  errorMessage: string | null = null;

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      this.router.navigate(['/']);
    }
  }

  checkLogin() {
    this.loginService.loginAuthentication(this.userName, this.password).subscribe({
      next: response => {
        if ('token' in response) {
          console.log('JWT Token:', response.token);
          this.router.navigate(['/']);
        }
      },
      error: error => {
        console.log('Login failed', error);
        this.errorMessage = error.message;
      }
    });
  }

  onSubmit(form: any): void {
    if (form.valid) {
      console.log('Form Submitted!', form.value);
    }
  }
}