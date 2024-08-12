import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User, ActiveQuestionnaire } from '../../models/questionare';
import { CommonModule } from '@angular/common';
import { AppDataService } from '../../services/data/app-data.service';
import { MockAuthService } from '../../services/auth/mock-auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
/**
 * Represents the dashboard component which is used display specific data for instructors and admins.
 */
export class DashboardComponent implements OnInit {
  authService = inject(MockAuthService);
  dataService = inject(AppDataService);
  router = inject(Router);
  studentList: User[] = [];
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  studentsInQuestionnaire: Set<number> = new Set<number>();

  ngOnInit(): void {
    const role = this.authService.getRole();

    if (role === 'admin') {
      this.loadDashboardData();
    } else {
      this.router.navigate(['/']);
    }
  }

  loadDashboardData(): void {
    this.dataService.getDashboardData().subscribe((data) => {
      this.studentList = data.students;
      this.activeQuestionnaires = data.activeQuestionnaires;
      this.studentsInQuestionnaire = new Set(data.activeQuestionnaires.map(q => q.student.id));
    });
  }

  /**
   * Adds a student to the questionnaire.
   * @param studentId The ID of the student to add.
   */
  addStudentToQuestionnaire(studentId: number): void {
    this.dataService.addStudentToQuestionnaire(studentId).subscribe(() => {
      this.loadDashboardData();
    });
  }

   /**
   * Checks if a student is in the questionnaire.
   * @param studentId The ID of the student to check.
   * @returns True if the student is in the questionnaire, false otherwise.
   */
    isStudentInQuestionnaire(studentId: number): boolean {
      return this.studentsInQuestionnaire.has(studentId);
    }

   /**
   * Creates a new active questionnaire.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   */
   createActiveQuestionnaire(studentId: number, teacherId: number): void {
    console.log('Creating new active questionnaire...');
    this.dataService.createActiveQuestionnaire(studentId, teacherId).subscribe((newQuestionnaire) => {
      console.log('New Active Questionnaire Created:', newQuestionnaire);
      this.loadDashboardData();
    });
  }

  /**
   * Deletes an active questionnaire.
   * @param questionnaireId The ID of the questionnaire to delete.
   */
  deleteActiveQuestionnaire(questionnaireId: string): void {
    if (questionnaireId) {
      this.dataService.deleteActiveQuestionnaire(questionnaireId).subscribe(() => {
        console.log(`Questionnaire with ID ${questionnaireId} deleted.`);
        this.loadDashboardData();
      });
    } else {
      console.error('Invalid questionnaire ID');
    }
  }
}
