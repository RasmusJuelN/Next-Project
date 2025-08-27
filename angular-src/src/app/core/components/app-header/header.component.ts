import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { MenuSvgComponent } from '../../../shared/components/menu-svg/menu-svg.component';
import { Role } from '../../../shared/models/user.model';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
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
  // for language translation
    private translate = inject(TranslateService);


  isAuthenticated = false;
  // Now userRole can be one of the Role enum values or null
  userRole: Role | null = null;
  isMenuOpen = false; // For toggling the mobile menu

  // Global navigation links
  globalNavLinks: { name: string; route: string }[] = [
    { name: '', route: '/' },
  ];

  // Role-specific navigation links
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

  ngOnInit() {
    // Subscribe to authentication state
    this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });

    // Subscribe to role changes and cast the value to the Role enum if necessary.
    this.authService.userRole$.subscribe((role) => {
      // Assuming your AuthService returns a string that matches Role values:
      if (role === Role.Teacher || role === Role.Admin || role === Role.Student) {
        this.userRole = role as Role;
      } else {
        this.userRole = null;
      }
    });
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']); // Redirect to login on logout
  }
//    setLanguage(lang: string) {
//   this.translate.use(lang);
// }
}
