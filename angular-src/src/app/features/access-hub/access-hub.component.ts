import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Role } from '../../shared/models/user.model';

@Component({
  selector: 'app-access-hub',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './access-hub.component.html',
  styleUrls: ['./access-hub.component.css']
})
export class AccessHubComponent {
  private authService = inject(AuthService);

  userRole:Role | null = null;
  isAuthenticated: boolean = false;

  // Define navLinks with a specific type
  navLinks: Partial<Record<Role, { name: string; route: string }[]>> = {
    [Role.Student]: [],
    [Role.Teacher]: [
      { name: 'Teacher dashboard', route: '/teacher-dashboard' }
    ],
    [Role.Admin]: [
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
      this.userRole = role as Role
    });
  }
}
