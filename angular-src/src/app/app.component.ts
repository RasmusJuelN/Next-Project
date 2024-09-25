import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth/auth.service';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink, RouterModule],
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.css']
})
export class AppComponent implements OnInit {
  isAuthenticated$: Observable<boolean> = of(false);
  userName: string | null = null;
  authService = inject(AuthService);  // Inject AuthService

  ngOnInit() {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
    this.isAuthenticated$.subscribe((isAuthenticated) => {
      if (isAuthenticated) {
        // If authenticated, get the token and decode it to retrieve the user's name
        this.setUserNameFromToken();
      } else {
        this.userName = null;
      }
    });
  }

  /**
   * Decodes the JWT token and retrieves the user's name.
   */
  private setUserNameFromToken(): void {
    const token = this.authService.jwtTokenService.getToken();
    if (token) {
      try {
        const decodedToken: any = this.authService.jwtTokenService.getDecodeToken();
        this.userName = decodedToken.full_name || 'User'; // Fallback to 'User' if no name is present
      } catch (error) {
        console.error('Error decoding token', error);
        this.userName = null;
      }
    } else {
      this.userName = null;
    }
  }

  /**
   * Logs out the user and clears the user state.
   */
  logout() {
    this.authService.logout();
    this.userName = null;  // Clear the username on logout
  }
}
