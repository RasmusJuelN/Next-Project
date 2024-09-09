import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AppAuthService } from '../../services/auth/app-auth.service'; // Use AppAuthService

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private authService = inject(AppAuthService); // Use AppAuthService
  private router = inject(Router);

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/']); // Redirect to login if token is missing or expired
    } else {
      // Check if the user is at the base 'dashboard' route
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
  }
}