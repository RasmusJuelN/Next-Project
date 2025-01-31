import { Component } from '@angular/core';
import { QuestionnaireSession } from './models/active.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveListComponent } from './components/active-list/active-list.component';

@Component({
  selector: 'app-active-questionnaire-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, ActiveListComponent],
  templateUrl: './active-questionnaire-manager.component.html',
  styleUrl: './active-questionnaire-manager.component.css'
})
export class ActiveQuestionnaireManagerComponent {
  searchStudent: string = '';
  searchTeacher: string = '';
  pageSize: number = 5;

  activeQuestionnaires: QuestionnaireSession[] = [
    {
      id: 'q1',
      templateId: 't101',
      templateName: 'Math Quiz',
      createdAt: new Date('2024-02-10T12:00:00'),
      updatedAt: new Date(),

      student: {
        user: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: 'test' },
        answered: false,
        answeredWhen: null,
      },

      teacher: {
        user: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-10T14:30:00'),
      },
    },
    {
      id: 'q2',
      templateId: 't102',
      templateName: 'History Quiz',
      createdAt: new Date('2024-02-10T12:30:00'),
      updatedAt: new Date(),

      student: {
        user: { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-10T13:00:00'),
      },

      teacher: {
        user: { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-10T13:15:00'),
      },
    },
  ];

  onSearchStudentChange(value: string) {
    this.searchStudent = value;
  }

  onSearchTeacherChange(value: string) {
    this.searchTeacher = value;
  }

  onPageSizeChange(value: number) {
    this.pageSize = value;
  }
}
