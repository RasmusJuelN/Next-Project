import { Component, computed, DestroyRef, inject, OnInit, signal, untracked } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginComponent } from '../login/login.component';
import { HomeService } from './services/home.service';
import { catchError, map, of, switchMap, take } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Role } from '../../shared/models/user.model';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';


/**
 * HomeComponent
 * 
 * The entry point for the application. 
 * Responsible for:
 * - Checking authentication state.
 * - Handling navigation to questionnaires.
 * - Managing login/logout flows and error states.
 */
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [LoginComponent, CommonModule, TranslateModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  private authService = inject(AuthService);
  private homeService = inject(HomeService);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);
  // private translate = inject(TranslateService);

  readonly user = this.authService.user;
  readonly userRole = computed(() => this.user()?.role ?? null);
  readonly username = computed(() => this.user()?.userName ?? '');

  activeQuestionnaireId: string = '';
  errorMessage: string | null = null;

  constructor() {
    toObservable(this.user).pipe(
      switchMap(u => {
        // reset on no user/admin
        if (!u || u.role === Role.Admin) {
          this.activeQuestionnaireId = '';
          this.errorMessage = null;
          return of<string | null>(null); // nothing to load
        }
        // load ID for non-admin users
        return this.homeService.checkForExistingActiveQuestionnaires().pipe(
          map(({ id }) => id ?? ''),
          catchError(() => {
            this.errorMessage = 'Could not load active questionnaire.';
            return of(''); // keep UI stable
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(id => {
      if (id !== null) this.activeQuestionnaireId = id; // null means we already reset above
    });
  }

  

  // Redirects based on user role
  toHub() {
    this.router.navigate(['/hub']);
  }

  // Navigate to the active questionnaire
  toActiveQuestionnaire() {
    if (this.activeQuestionnaireId) {
      this.router.navigate(['/answer', this.activeQuestionnaireId]);
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  onLoginError(error: string) {
    this.errorMessage ='Login failed. Please try again.';
    console.error('Login error:', error);
  }

//   setLanguage(lang: string) {
//   this.translate.use(lang);
// }

}
