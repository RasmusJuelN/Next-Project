import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { LocalStorageService } from './services/misc/local-storage.service';

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
  localStorageService = inject(LocalStorageService);

  /**
   * Initializes the component.
   * Checks if a token exists in the local storage and retrieves the user name from the decoded token.
   */
  ngOnInit() {
    this.tokenExists = this.localStorageService.getToken() !== null;

    if (this.tokenExists) {
      const token = this.localStorageService.getToken()
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
    this.localStorageService.removeData('token');
    this.tokenExists = false;
    alert('Token deleted');
  }
}
