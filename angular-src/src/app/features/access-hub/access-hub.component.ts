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

  userRole: 'teacher' | 'admin' | null = null;
  isAuthenticated: boolean = false;

  // Define navLinks with a specific type
  navLinks: Record<'teacher' | 'admin', { name: string; route: string }[]> = {
    teacher: [
      {name: 'Teacher dashboard', route:'/teacher-dashboard'}
    ],
    admin: [
      { name: 'Template Manager', route: '/templates' },
      { name: 'Active Questionnaire Manager', route: '/active-questionnaire' },
      { name: 'Settings', route: '/settings' },
      { name: 'Logs', route: '/logs' }
    ]
  };

  ngOnInit(): void {
    // Subscribe to authentication state changes
    this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });

    this.authService.userRole$.subscribe((role) => {
      this.userRole = role as 'teacher' | 'admin' | null;
    });
  }
}
