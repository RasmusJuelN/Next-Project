import { Injectable } from '@angular/core';
import { ActiveQuestionnaire, User } from '../models/questionare';
import { Observable } from 'rxjs';
import { AppDataService } from './data/app-data.service';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private appDataService: AppDataService) {}

  /**
   * Fetches the data required for the dashboard, including students and active questionnaires.
   * @returns An observable of the dashboard data.
   */
  getDashboardData(): Observable<{ students: User[], activeQuestionnaires: ActiveQuestionnaire[] }> {
    return this.appDataService.getDashboardData();
  }

  /**
   * Adds a student to an active questionnaire.
   * @param studentId The ID of the student to add.
   * @returns An observable for the operation.
   */
  addStudentToQuestionnaire(studentId: number): Observable<void> {
    return this.appDataService.addStudentToQuestionnaire(studentId);
  }

  /**
   * Creates a new active questionnaire for a student and teacher.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   * @returns An observable of the newly created questionnaire.
   */
  createActiveQuestionnaire(studentId: number, teacherId: number): Observable<ActiveQuestionnaire> {
    return this.appDataService.createActiveQuestionnaire(studentId, teacherId);
  }

  /**
   * Deletes an active questionnaire by its ID.
   * @param questionnaireId The ID of the questionnaire to delete.
   * @returns An observable for the operation.
   */
  deleteActiveQuestionnaire(questionnaireId: string): Observable<void> {
    return this.appDataService.deleteActiveQuestionnaire(questionnaireId);
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
