import { Component, inject } from '@angular/core';
import { ShowActiveService } from './services/show-active.service';
import { UserSpecificActiveQuestionnaireBase } from './models/show-active.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-show-active-questionnaire',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './show-active-questionnaire.component.html',
  styleUrl: './show-active-questionnaire.component.css'
})
export class ShowActiveQuestionnaireComponent {

  showActiveService = inject(ShowActiveService)
  activeQuestionnaires:UserSpecificActiveQuestionnaireBase[] = []

  ngOnInit(): void {
    this.fetch();
  }
  
  fetch(): void {
    this.showActiveService.fetchActiveQuestionnaires().subscribe({
      next: (data) => {
        this.activeQuestionnaires = data;
      },
      error: (error) => {
        console.error('Error fetching active questionnaires:', error);
      }
    });
  }
}
