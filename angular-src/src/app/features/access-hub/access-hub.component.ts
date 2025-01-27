import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-access-hub',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './access-hub.component.html',
  styleUrls: ['./access-hub.component.css']
})
export class AccessHubComponent {
  private authService = inject(AuthService);

  // Explicitly define the roles as a union of specific strings
  userRole: 'teacher' | 'admin' | null = null; // User role, matching keys in navLinks
  isAuthenticated: boolean = false; // Tracks if the user is authenticated

  // Define navLinks with a specific type
  navLinks: Record<'teacher' | 'admin', { name: string; route: string }[]> = {
    teacher: [
      { name: 'Overview', route: '/hub' }
    ],
    admin: [
      { name: 'Template Manager', route: '/templates' },
      { name: 'Active Questionnaire Manager', route: '/active-questionnaire' },
      { name: 'Settings', route: '/settings' },
      { name: 'Logs', route: '/logs' }
    ]
  };

  ngOnInit(): void {
    // Refresh authentication state
    this.authService.refreshAuthenticationState();

    // Subscribe to authentication state changes
    this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });

    // Subscribe to role changes
    this.authService.userRole$.subscribe((role) => {
      // Ensure userRole is either 'teacher', 'admin', or null
      this.userRole = role as 'teacher' | 'admin' | null;
    });
  }
}
