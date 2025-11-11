import { Component, computed, effect, inject } from '@angular/core';
import { take, switchMap } from 'rxjs/operators';
import { ShowActiveService } from './services/show-active.service';
import { ActiveQuestionnaireResponse, ActiveQuestionnaireBase } from './models/show-active.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { Role } from '../../shared/models/user.model';
import { TranslateModule } from '@ngx-translate/core';

@Component({
    selector: 'app-show-active-questionnaire',
    imports: [CommonModule, RouterLink, TranslateModule],
    templateUrl: './show-active-questionnaire.component.html',
    styleUrls: ['./show-active-questionnaire.component.css']
})
export class ShowActiveQuestionnaireComponent {
  showActiveService = inject(ShowActiveService);
  authService = inject(AuthService);

  activeQuestionnaires: ActiveQuestionnaireBase[] = [];
  readonly user = this.authService.user;
  public Role = Role;

  constructor() {
    effect(() => {
      const role = this.user()?.role;
      if (role === Role.Student || role === Role.Teacher) {
        this.fetch();
      }
    });
  }

shouldShowAnswerButton(q: ActiveQuestionnaireBase): boolean {
  const role = this.user()?.role ?? null
  if (!role) return false;

  if (role === Role.Student && !q.studentCompletedAt) return true;
  if (role === Role.Teacher && !q.teacherCompletedAt) return true;

  return false;
}



  fetch(): void {
    this.showActiveService.fetchActiveQuestionnaires().subscribe({
      next: (response: ActiveQuestionnaireBase[]) => {
        this.activeQuestionnaires = response;
      },
      error: (error) => {
        console.error('Error fetching active questionnaires:', error);
      }
    });
  }
}
