import { Component, inject, OnInit } from '@angular/core';
import { MockDataService } from '../../services/mock-data.service';
import { Router } from '@angular/router';
import { User, ActiveQuestionnaire } from '../../models/questionare';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

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
  authService = inject(AuthService);
  dataService = inject(MockDataService);
  router = inject(Router);
  studentList: User[] = [];
  studentsYetToFinish: User[] = [];
  activeQuestionnaires: ActiveQuestionnaire[] = [];

  ngOnInit(): void {
    const role = this.authService.getRole();
    console.log('User Role:', role);

    if (role === 'teacher') {
      console.log(`Welcome ${role}`);
      this.loadStudentsData();
      this.loadActiveQuestionnaires();
    } else if (role === 'admin') {
      console.log(`Welcome ${role}`);
      this.loadStudentsData();
      this.loadActiveQuestionnaires();
    } else if (role === 'student') {
      console.log(`Welcome ${role}`);
      const userId = this.authService.getUserId();
      console.log('User ID:', userId);
      if (userId !== null) {
        this.router.navigate(['answer', userId]);
      } else {
        console.error('User ID is null, cannot navigate to answer page');
      }
    } else {
      console.log('Unknown role');
      this.router.navigate(['/']);
    }
  }

  /**
   * Loads the students' data.
   */
  loadStudentsData(): void {
    this.dataService.getStudents().subscribe((students) => {
      this.studentList = students;
    });
    this.dataService.getStudentsYetToFinish().subscribe((students) => {
      this.studentsYetToFinish = students;
    });
  }

  /**
   * Loads the active questionnaires.
   */
  loadActiveQuestionnaires(): void {
    this.dataService.getActiveQuestionnaires().subscribe((questionnaires) => {
      this.activeQuestionnaires = questionnaires;
    });
  }

  /**
   * Adds a student to the questionnaire.
   * @param studentId The ID of the student to add.
   */
  addStudentToQuestionnaire(studentId: number): void {
    this.dataService.addStudentToQuestionnaire(studentId);
    this.loadStudentsData();
  }

  /**
   * Checks if a student is in the questionnaire.
   * @param studentId The ID of the student to check.
   * @returns True if the student is in the questionnaire, false otherwise.
   */
  isStudentInQuestionnaire(studentId: number): boolean {
    return this.dataService.isStudentInQuestionnaire(studentId);
  }

  /**
   * Creates a new active questionnaire.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   */
  createActiveQuestionnaire(studentId: number, teacherId: number): void {
    console.log('Creating new active questionnaire...');
    const newQuestionnaire = this.dataService.createActiveQuestionnaire(studentId, teacherId);
    console.log('New Active Questionnaire Created:', newQuestionnaire);
    this.loadActiveQuestionnaires();
  }

  /**
   * Deletes an active questionnaire.
   * @param questionnaireId The ID of the questionnaire to delete.
   */
  deleteActiveQuestionnaire(questionnaireId: string): void {
    if (questionnaireId) {
      this.dataService.deleteActiveQuestionnaire(questionnaireId);
      console.log(`Questionnaire with ID ${questionnaireId} deleted.`);
      this.loadActiveQuestionnaires();
    } else {
      console.error('Invalid questionnaire ID');
    }
  }
}
