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
  // Hold the current cursor returned from the API
  queryCursor: string | null = null;
  // Optional: store a history of cursors to enable "back" navigation
  previousCursors: string[] = [];
  // Store the current user role (e.g. Role.Student or Role.Teacher)
  currentUserRole!: Role;
  public Role = Role;

  ngOnInit(): void {
    // Get the current user role and then fetch the questionnaires.
    this.authService.userRole$.pipe(take(1)).subscribe((role) => {
      if (role) {
        this.currentUserRole = role as Role;
        this.fetch();
      }
    });
  }
  
  fetch(cursor?: string): void {
    this.showActiveService.fetchActiveQuestionnaires(cursor).subscribe({
      next: (response: ActiveQuestionnaireResponse) => {
        if (cursor) {
          if (this.queryCursor) {
            this.previousCursors.push(this.queryCursor);
          }
          this.activeQuestionnaires = [...this.activeQuestionnaires, ...response.activeQuestionnaireBases];
        } else {
          this.activeQuestionnaires = response.activeQuestionnaireBases;
        }
        this.queryCursor = response.queryCursor;
      },
      error: (error) => {
        console.error('Error fetching active questionnaires:', error);
      }
    });
  }

  loadMore(): void {
    if (this.queryCursor) {
      this.fetch(this.queryCursor);
    }
  }

  loadPrevious(): void {
    if (this.previousCursors.length) {
      const prevCursor = this.previousCursors.pop()!;
      this.fetch(prevCursor);
    }
  }
}
