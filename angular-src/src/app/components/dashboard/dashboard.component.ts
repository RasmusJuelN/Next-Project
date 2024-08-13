import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User, ActiveQuestionnaire } from '../../models/questionare';
import { CommonModule } from '@angular/common';
import { AppDataService } from '../../services/data/app-data.service';
import { AppAuthService } from '../../services/auth/app-auth.service'; // Use AppAuthService

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
/**
 * Represents the dashboard component which is used to display specific data for instructors and admins.
 */
export class DashboardComponent implements OnInit {
  private authService = inject(AppAuthService); // Use AppAuthService
  private dataService = inject(AppDataService);
  private router = inject(Router);
  
  studentList: User[] = [];
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  studentsInQuestionnaire: Set<number> = new Set<number>();

  ngOnInit(): void {
    if (this.authService.hasRole('admin')) {
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
    this.dataService.createActiveQuestionnaire(studentId, teacherId).subscribe((newQuestionnaire) => {
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
        this.loadDashboardData();
      });
    } else {
      console.error('Invalid questionnaire ID');
    }
  }
}