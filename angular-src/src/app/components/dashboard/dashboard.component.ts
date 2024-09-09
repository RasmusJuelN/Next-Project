import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private authService = inject(AuthService); // Use AppAuthService
  private router = inject(Router);

  ngOnInit(): void {
    // Subscribe to isAuthenticated$ to check the authentication status
    this.authService.isAuthenticated$.subscribe(isAuthenticated => {
      if (!isAuthenticated) {
        // Redirect to login if the user is not authenticated
        this.router.navigate(['/']);
      } else {
        // User is authenticated, handle role-based redirection
        const currentRoute = this.router.url;
        if (currentRoute === '/dashboard') {
          if (this.authService.hasRole('admin')) {
            this.router.navigate(['/dashboard/admin']);
          } else if (this.authService.hasRole('teacher')) {
            this.router.navigate(['/dashboard/teacher']);
          } else {
            this.router.navigate(['/']);
          }
        }
      }
    });
  }
}