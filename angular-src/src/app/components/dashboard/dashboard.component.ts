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
  private authService = inject(AuthService); // Inject the AuthService
  private router = inject(Router); // Inject the Router service

  ngOnInit(): void {
    // Subscribe to the isAuthenticated$ observable to track the authentication status
    this.authService.isAuthenticated$.subscribe(isAuthenticated => {
      if (!isAuthenticated) {
        // If the user is not authenticated, navigate to the login/home route
        this.router.navigate(['/']);
      } else {
        // If the user is authenticated, handle role-based redirection
        this.redirectBasedOnRole();
      }
    });
  }

  private redirectBasedOnRole(): void {
    const currentRoute = this.router.url;
    
    if (currentRoute === '/dashboard') {
      if (this.authService.hasRole('admin')) {
        this.router.navigate(['/dashboard/admin'], { replaceUrl: true });
      } else if (this.authService.hasRole('teacher')) {
        this.router.navigate(['/dashboard/teacher'], { replaceUrl: true });
      } else {
        // If the user doesn't have a valid role, send them to the homepage
        this.router.navigate(['/']);
      }
    }
  }
}
