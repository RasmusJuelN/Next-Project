import { Component, computed, DestroyRef, effect, inject, OnInit, signal, untracked } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginComponent } from '../login/login.component';
import { HomeService } from './services/home.service';
import { catchError, of, take } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Role } from '../../shared/models/user.model';


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

  // Local UI state as a signal
  readonly activeQuestionnaireString = signal('');
  readonly errorMessage = signal<string | null>(null);


  constructor() {
    effect(() => {
      const u = this.user();

      // logged out
      if (!u) {
        this.activeQuestionnaireString.set('');
        this.errorMessage.set(null);
        return;
      }

      // admin: skip fetch
      if (u.role === Role.Admin) {
        this.activeQuestionnaireString.set('');
        return;
      }
      // side effect without tracking local writes
      untracked(() => {
        this.homeService.checkForExistingActiveQuestionnaires()
          .pipe(
            // take(1) is optional for HttpClient
            catchError(() => {
              this.errorMessage.set('Could not load active questionnaire.');
              return of({ exists: false, id: '' as string });
            })
          )
          .subscribe(({ id }) => {
            this.activeQuestionnaireString.set(id ?? '');
          });
      });
    });
  }
  

  // Redirects based on user role
  toHub() {
    this.router.navigate(['/hub']);
  }

  // Navigate to the active questionnaire
  toActiveQuestionnaire() {
    if (this.activeQuestionnaireString) {
      this.router.navigate(['/answer', this.activeQuestionnaireString]);
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  onLoginError(error: string) {
    this.errorMessage.set('Login failed. Please try again.');
    console.error('Login error:', error);
  }

//   setLanguage(lang: string) {
//   this.translate.use(lang);
// }

}
