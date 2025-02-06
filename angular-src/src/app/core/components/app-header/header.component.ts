import { Component, inject } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { MenuSvgComponent } from '../../../shared/components/menu-svg/menu-svg.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, CommonModule, MenuSvgComponent],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  isAuthenticated = false;
  userRole: 'teacher' | 'admin' | null = null;
  isMenuOpen = false; // For toggling the mobile menu

  // Global navigation links (always shown)
  globalNavLinks: { name: string; route: string }[] = [
    { name: 'Home', route: '/' },
  ];

  // Role-specific navigation links
  navLinks: Record<'teacher' | 'admin', { name: string; route: string }[]> = {
    teacher: [
      { name: 'Overview', route: '/hub' },
      {name: 'Teacher dashboard', route:'/teacher-dashboard'}
    ],
    admin: [
      { name: 'Overview', route: '/hub' },
      { name: 'Templates', route: '/templates' },
      { name: 'Settings', route: '/settings' },
    ],
  };

  ngOnInit() {
    // Subscribe to authentication state
    this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
      this.isAuthenticated = isAuthenticated;
    });

    // Subscribe to role changes
    this.authService.userRole$.subscribe((role) => {
      this.userRole = role as 'teacher' | 'admin' | null;
    });
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']); // Redirect to login on logout
  }
}
