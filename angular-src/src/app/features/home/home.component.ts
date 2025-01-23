import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [LoginComponent, CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  loggedInAlready$ = this.authService.isAuthenticated$;
  activeQuestionnaireString = '';
  errorMessage = '';

  toActiveQuestionnaire() {
    // WIP
    this.router.navigate(['/questionnaire', "activeQuestionnaireId"]);
  }

  toDashboard() {
    this.router.navigate(['/dashboard']);
  }

  logout() {
    this.authService.logout();
  }
}
