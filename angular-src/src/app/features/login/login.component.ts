import { Component, EventEmitter, Output, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, TranslateModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  

  @Output() loggedIn = new EventEmitter<boolean>(); // Emits true if login succeeds
  @Output() errorOccurred = new EventEmitter<string>(); // Emits error message if login fails

  userName = '';
  password = '';
  errorMessage = '';
  // 
   isLoading = false;

  login() {
        this.isLoading = true; //  Set loading to true

    this.authService.login(this.userName, this.password).subscribe({
      next: (isAuthenticated) => {
        this.isLoading = false; // Set loading to false after login attempt
        if (isAuthenticated) {
          this.loggedIn.emit(true); // Notify parent component of successful login
        } else {
          const errorMsg = 'Invalid username or password.'; //{{'LOGIN_ERROR_INVALID' | translate  }}
          this.errorMessage = errorMsg;
          this.errorOccurred.emit(errorMsg); // Notify parent component of the error
        }
      },
      error: (error) => {
        this.isLoading = false; // Set loading to false on error
        const errorMsg = 'An error occurred during login. Please try again.'; //{{'LOGIN_ERROR_GENERIC' | translate }}
        this.errorMessage = errorMsg;
        this.errorOccurred.emit(errorMsg);
      },
    });
  }

  onSubmit() {
    this.login();
  }
}
