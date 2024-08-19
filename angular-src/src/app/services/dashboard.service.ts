import { inject, Injectable } from '@angular/core';
import { ActiveQuestionnaire, User } from '../models/questionare';
import { catchError, map, Observable } from 'rxjs';
import { AppDataService } from './data/app-data.service';
import { ErrorHandlingService } from './error-handling.service';
import { AppAuthService } from './auth/app-auth.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private errorHandlingService = inject(ErrorHandlingService);
  private appDataService = inject(AppDataService);
  private authService = inject(AppAuthService)
  private router = inject(Router);

  // NEW
  getDashboardDataTeacher(): Observable<{
    students: User[],
    activeQuestionnaires: ActiveQuestionnaire[]
  }> {
    // Get the current teacher ID
    const teacherId = parseInt(this.authService.getUserId()!, 10);

    // Fetch and filter the dashboard data
    return this.appDataService.getDashboardData().pipe(
      map(data => ({
        students: data.students,
        activeQuestionnaires: data.activeQuestionnaires.filter(
          questionnaire => questionnaire.teacher.id === teacherId
        )
      })),
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to get dashboard data for teacher'))
    );
  }

  createNewQuestionnaire(studentId:number, teacherId:number){
    return this.appDataService.createActiveQuestionnaire(studentId, teacherId).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to create active questionnaire'))
    );
  }




  // OLD, for admin


  /**
   * Fetches the data required for the dashboard, including students and active questionnaires.
   * @returns An observable of the dashboard data.
   */
  getDashboardData(): Observable<{ students: User[], activeQuestionnaires: ActiveQuestionnaire[] }> {
    return this.appDataService.getDashboardData().pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to get dashboard data'))
    );
  }

  
  
  /**
   * Adds a student to an active questionnaire.
   * @param studentId The ID of the student to add.
   * @returns An observable for the operation.
   */
  addStudentToQuestionnaire(studentId: number): Observable<void> {
    return this.appDataService.addStudentToQuestionnaire(studentId).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to add student to questionnaire'))
    );
  }

  /**
   * Creates a new active questionnaire for a student and teacher.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   * @returns An observable of the newly created questionnaire.
   */
  createActiveQuestionnaire(studentId: number, teacherId: number): Observable<ActiveQuestionnaire> {
    return this.appDataService.createActiveQuestionnaire(studentId, teacherId).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to create active questionnaire'))
    );
  }
  /**
   * Deletes an active questionnaire by its ID.
   * @param questionnaireId The ID of the questionnaire to delete.
   * @returns An observable for the operation.
   */
  deleteActiveQuestionnaire(questionnaireId: string): Observable<void> {
    return this.appDataService.deleteActiveQuestionnaire(questionnaireId).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to delete active questionnaire'))
    );
  }
  /**
   * Checks if a student is in an active questionnaire.
   * @param studentId The ID of the student.
   * @param activeQuestionnaires The list of active questionnaires.
   * @returns True if the student is in an active questionnaire, false otherwise.
   */
  isStudentInQuestionnaire(studentId: number, activeQuestionnaires: ActiveQuestionnaire[]): boolean {
    return activeQuestionnaires.some(q => q.student.id === studentId);
  }
}
