import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  
  userName = '';
  password = '';
  errorMessage = "";

  login() {

    this.authService.loginAuthentication(this.userName, this.password).subscribe({
      next: (isAuthenticated) => {
        if (isAuthenticated) {
          // WIP
        } else {
          this.errorMessage = 'Invalid username or password.';
        }
      },
      error: (error) => {
        this.errorMessage = 'An error occurred during login. Please try again.';
      }
    });
  }


  logout(){
    this.authService.logout();
  }

  onSubmit() {
    this.login();
  }
  
}
