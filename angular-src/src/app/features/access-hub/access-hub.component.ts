import { Component, computed, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

import { RouterModule } from '@angular/router';
import { Role } from '../../shared/models/user.model';

/**
 * AccessHubComponent
 * 
 * Acts as a central navigation hub that dynamically renders links based on roles * 
 * Features:
 * - Subscribes to authentication and role state from AuthService.
 * - Provides different navigation options for Students, Teachers, and Admins.
 */
@Component({
    selector: 'app-access-hub',
    imports: [RouterModule],
    templateUrl: './access-hub.component.html',
    styleUrls: ['./access-hub.component.css']
})
export class AccessHubComponent {
  private authService = inject(AuthService);

  readonly isAuthenticated = this.authService.isAuthenticated;
  readonly userRole = computed<Role | null>(() => this.authService.user()?.role ?? null);

  /** Navigation links available for each role. */
  navLinks: Partial<Record<Role, { name: string; route: string }[]>> = {
    [Role.Student]: [
      { name: 'Nuværende aktive spørgeskemaer', route: '/show-active-questionnaires' }
    ],
    [Role.Teacher]: [
      { name: 'Oversigt', route: '/teacher-dashboard' },
      { name: 'Nuværende aktive spørgeskemaer', route: '/show-active-questionnaires' }

    ],
    [Role.Admin]: [
      { name: 'spørgeskemaer skabelon Manager', route: '/templates' },
      { name: 'Aktive spørgeskemaer manager', route: '/active-questionnaire' }
    ]
  };

  readonly navLinksForUser= computed(() => {
    const role = this.userRole();
    return role != null ? this.navLinks[role] : [];
  });
}
