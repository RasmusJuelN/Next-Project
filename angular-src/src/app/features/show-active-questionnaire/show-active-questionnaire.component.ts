import { Component, inject } from '@angular/core';
import { take, switchMap } from 'rxjs/operators';
import { ShowActiveService } from './services/show-active.service';
import { ActiveQuestionnaireResponse, ActiveQuestionnaireBase } from './models/show-active.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { Role } from '../../shared/models/user.model';

@Component({
  selector: 'app-show-active-questionnaire',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './show-active-questionnaire.component.html',
  styleUrls: ['./show-active-questionnaire.component.css']
})
export class ShowActiveQuestionnaireComponent {
  showActiveService = inject(ShowActiveService);
  authService = inject(AuthService);

  activeQuestionnaires: ActiveQuestionnaireBase[] = [];
  currentUserRole!: Role;
  public Role = Role;

  ngOnInit(): void {
    this.authService.userRole$.pipe(take(1)).subscribe((role) => {
      if (role) {
        this.currentUserRole = role as Role;
        this.fetch();
      }
    });
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
