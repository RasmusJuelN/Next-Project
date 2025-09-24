import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { MenuSvgComponent } from '../../../shared/components/menu-svg/menu-svg.component';
import { Role } from '../../../shared/models/user.model';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

/**
 * Header component responsible for:
 * - Displaying global and role-based navigation links.
 * - Handling authentication state and role-based visibility.
 * - Managing mobile menu toggling.
 * - Supporting multi-language navigation labels via `@ngx-translate/core`.
 */
@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule, MenuSvgComponent, TranslateModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  /** Translation service for switching navigation labels by language. */
  private translate = inject(TranslateService);


  isAuthenticated = false;
  userRole: Role | null = null;
  isMenuOpen = false;

  /** Global navigation links visible to all users. */
  globalNavLinks: { name: string; route: string }[] = [
    { name: '', route: '/' },
  ];

  /** Role-specific navigation links (labels use translation keys). */
  navLinks: Record<Role, { name: string; route: string }[]> = {
    [Role.Student]: [
      { name: 'NAV_ACTIVE_QUESTIONNAIRES', route: '/show-active-questionnaires' }
    ],
    [Role.Teacher]: [
      //{ name: 'Overview', route: '/hub' },
      { name: 'NAV_OVERVIEW', route: '/teacher-dashboard' },
      { name: 'NAV_ACTIVE_QUESTIONNAIRES', route: '/show-active-questionnaires' },
      { name: 'NAV_DATA_COMPARE', route: '/data-compare' }
    ],
    [Role.Admin]: [
      //{ name: 'Overview', route: '/hub' },
      { name: 'NAV_TEMPLATES', route: '/templates' },
      { name: 'NAV_ACTIVE_QUESTIONNAIRES', route: '/active-questionnaire' }
    ],
  };

  /**
   * Lifecycle hook: subscribes to authentication and role streams
   * from `AuthService` to update UI state.
   */
  ngOnInit() {
    this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });

    this.authService.userRole$.subscribe((role) => {
      if (role === Role.Teacher || role === Role.Admin || role === Role.Student) {
        this.userRole = role as Role;
      } else {
        this.userRole = null;
      }
    });
  }

  /** Toggles the mobile navigation menu open/closed. */
  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }
  
  /**
   * Logs the user out:
   * - Calls `AuthService.logout()`.
   * - Redirects to the home route (`'/'`).
   */
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
//    setLanguage(lang: string) {
//   this.translate.use(lang);
// }
}
