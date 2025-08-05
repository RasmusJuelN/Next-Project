import { Component, EventEmitter, Output, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-login',
    imports: [FormsModule, CommonModule],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  @Output() loggedIn = new EventEmitter<boolean>(); // Emits true if login succeeds
  @Output() errorOccurred = new EventEmitter<string>(); // Emits error message if login fails

  userName = '';
  password = '';
  errorMessage = '';

  login() {
    this.authService.login(this.userName, this.password).subscribe({
      next: (isAuthenticated) => {
        if (isAuthenticated) {
          this.loggedIn.emit(true); // Notify parent component of successful login
        } else {
          const errorMsg = 'Invalid username or password.';
          this.errorMessage = errorMsg;
          this.errorOccurred.emit(errorMsg); // Notify parent component of the error
        }
      },
      error: (error) => {
        const errorMsg = 'An error occurred during login. Please try again.';
        this.errorMessage = errorMsg;
        this.errorOccurred.emit(errorMsg);
      },
    });
  }

  onSubmit() {
    this.login();
  }
}
