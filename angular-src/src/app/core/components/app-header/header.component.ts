import {
  ChangeDetectorRef,
  Component,
  computed,
  inject,
  HostListener,
} from "@angular/core";
import { RouterLink, RouterLinkActive, Router } from "@angular/router";
import { AuthService } from "../../services/auth.service";

import { MenuSvgComponent } from "../../../shared/components/menu-svg/menu-svg.component";
import { Role } from "../../../shared/models/user.model";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { LanguageSwitcherComponent } from "../language-switcher/language-switcher.component";

/**
 * Header component responsible for:
 * - Displaying global and role-based navigation links.
 * - Handling authentication state and role-based visibility.
 * - Managing mobile menu toggling.
 * - Supporting multi-language navigation labels via `@ngx-translate/core`.
 */
@Component({
    selector: "app-header",
    imports: [
    RouterLink,
    RouterLinkActive,
    MenuSvgComponent,
    TranslateModule,
    LanguageSwitcherComponent
],
    templateUrl: "./header.component.html",
    styleUrls: ["./header.component.css"]
})
export class HeaderComponent {
  /**
   * Toggle profile dropdown
   */
  toggleProfileDropdown() {
    this.showProfileDropdown = !this.showProfileDropdown;
  }
  /**
   * Controls visibility of the profile dropdown in the navbar
   */
  showProfileDropdown = false;

  /**
   * Close profile dropdown when clicking outside
   */
  @HostListener("document:click", ["$event"])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const btn = document.querySelector('.profile-dropdown-trigger');
    const menu = document.querySelector('.profile-dropdown-menu');
    if (
      this.showProfileDropdown &&
      btn && menu &&
      !btn.contains(target) &&
      !menu.contains(target)
    ) {
      this.showProfileDropdown = false;
    }
  }
  private authService = inject(AuthService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private cdr = inject(ChangeDetectorRef);

  readonly isAuthenticated = this.authService.isAuthenticated; // already a computed in the service
  readonly userRole = computed<Role | null>( () => this.authService.user()?.role ?? null );
  readonly username = computed(() => this.authService.user()?.userName ?? '');

  isMenuOpen = false;

  /** Global navigation links visible to all users. */
  globalNavLinks: { name: string; route: string }[] = [
    { name: "NAV.HOME", route: "/" },
    { name: "NAV.USER_GUIDE", route: "/user-guide" },
  ];

  /** Role-specific navigation links (labels use translation keys). */
  navLinks: Record<Role, { name: string; route: string }[]> = {
    [Role.Student]: [
      {
        name: "NAV.ACTIVE_QUESTIONNAIRES",
        route: "/show-active-questionnaires",
      },
      { name: "NAV.USER_GUIDE", route: "/user-guide" }
    ],
    [Role.Teacher]: [
      //{ name: 'Overview', route: '/hub' },
      { name: "NAV.OVERVIEW", route: "/teacher-dashboard" },
      {
        name: "NAV.ACTIVE_QUESTIONNAIRES",
        route: "/show-active-questionnaires",
      },
      { name: "NAV.DATA_COMPARE", route: "/data-compare" },
      { name: "NAV.RESULT_HISTORY", route: "/result-history" },
      { name: "NAV.USER_GUIDE", route: "/user-guide" }
    ],
    [Role.Admin]: [
      //{ name: 'Overview', route: '/hub' },
      { name: "NAV.TEMPLATES", route: "/templates" },
      { name: "NAV.ACTIVE_QUESTIONNAIRES", route: "/active-questionnaire" },
      { name: "NAV.USER_GUIDE", route: "/user-guide" }
    ],
  };

  readonly navLinksForUser = computed(() => {
    const role = this.userRole();
    return role != null ? this.navLinks[role] : [];
  });
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
    this.cdr.markForCheck();
    this.router.navigate(["/"]);
  }
  //    setLanguage(lang: string) {
  //   this.translate.use(lang);
  // }
}
