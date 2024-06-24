import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  templateUrl: 'app.component.html',
  styleUrl: 'app.component.css'
})
/**
 * Represents the root component of the application.
 */
export class AppComponent {
  tokenExists = false;
  userName: string | null = null;

  /**
   * Initializes the component.
   * Checks if a token exists in the local storage and retrieves the user name from the decoded token.
   */
  ngOnInit() {
    this.tokenExists = localStorage.getItem('token') !== null;

    if (this.tokenExists) {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const decodedToken: any = jwtDecode(token);
          // Scope refers to the role of the user
          this.userName = decodedToken.full_name || null;
        } catch (error) {
          console.error('Invalid token', error);
        }
      }
    }
  }

  /**
   * Deletes the token from the local storage and updates the tokenExists flag.
   */
  deleteToken() {
    localStorage.removeItem('token');
    this.tokenExists = false;
    alert('Token deleted');
  }
}
