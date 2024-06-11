import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { MockAuthService } from '../../services/mock-auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

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

  constructor(private http: HttpClient) {}

  checkLogin() {
    // Headers
    const headers = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded'
    });

    // URL-encoded form data
    let body = new URLSearchParams();
    body.set('username', this.userName);
    body.set('password', this.password);

    this.http.post('http://localhost:4200/api/v1/auth', body.toString(), { headers: headers })
    .subscribe({
      next: response => {
        if ('access_token' in response) {
          this.router.navigate(['/']);
        }
      },
      error: error => {
        console.log('Login failed', error);
      }
    });
  }

  onSubmit(form: any): void {
    if (form.valid) {
      this.checkLogin();
    }
  }
}