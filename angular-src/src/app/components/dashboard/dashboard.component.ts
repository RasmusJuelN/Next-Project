import { Component, inject, OnInit } from '@angular/core';
import { MockDataService } from '../../services/mock-data.service';
import { Router } from '@angular/router';
import { User } from '../../models/questionare';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
/**
 * Represents the dashboard component which is used display specfic data for instructors and admins.
 */
export class DashboardComponent implements OnInit {
  authService = inject(AuthService);
  dataService = inject(MockDataService);
  router = inject(Router);

  /**
   * Represents the list of students.
   */
  studentList: User[] = [];

  /**
   * Represents the list of students who have not finished yet.
   */
  studentsYetToFinish: User[] = [];

  /**
   * Initializes the component.
   */
  ngOnInit(): void {
    const role = this.authService.getRole();
    console.log('User Role:', role);

    if (role === 'teacher') {
      console.log(`welcome ${role}`);
      this.loadStudentsData();
    } else if (role === 'admin') {
      console.log(`welcome ${role}`);
    } else if (role === 'student') {
      console.log(`welcome ${role}`);
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
}