import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginComponent } from '../login/login.component';
import { HomeService } from './services/home.service';
import { catchError, of } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [LoginComponent, CommonModule, TranslateModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  private authService = inject(AuthService);
  private homeService = inject(HomeService);
  private router = inject(Router);
  // private translate = inject(TranslateService);

  loggedInAlready$ = this.authService.isAuthenticated$;
  activeQuestionnaireString = '';
  userRole: string | null = null;
  errorMessage: string | null = null;
  username: string = '';

  ngOnInit(): void {
    // Check if an active questionnaire exists when logged in
    this.loggedInAlready$.subscribe((isLoggedIn) => {
      if (isLoggedIn) {
        this.userRole = this.authService.getUserRole();
        this.username = this.authService.getUser()?.userName || '';
        if (this.userRole !== 'admin') {
          this.homeService
            .checkForExistingActiveQuestionnaires()
            .pipe(catchError((error) => of({ exists: false, id: null })))
            .subscribe((response: any) => {
              this.activeQuestionnaireString = response?.id || '';
            });
        }
      } else {
        this.userRole = null;
        this.activeQuestionnaireString = '';
        this.username = '';
      }
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

  onLoginSuccess(){
    
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  onLoginError(error: string) {
    this.errorMessage = 'Login failed. Please try again.';
    console.error('Login error:', error);
  }

//   setLanguage(lang: string) {
//   this.translate.use(lang);
// }

}
