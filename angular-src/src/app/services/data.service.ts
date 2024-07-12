import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActiveQuestionnaire, Question, User } from '../models/questionare';
import { environment } from '../../environments/environment';

/**
 * Service for handling data operations.
 */
@Injectable({
  providedIn: 'root'
})
export class DataService {
  private baseUrl: string = `${environment.apiUrl}`;
  private httpClient = inject(HttpClient);

  /**
   * Fetches the dashboard data, including students, students yet to finish, and active questionnaires.
   * @returns An observable containing the dashboard data.
   */
  getDashboardData(): Observable<{
    students: User[],
    activeQuestionnaires: ActiveQuestionnaire[]
  }> {
    return this.httpClient.get<{
      students: User[],
      activeQuestionnaires: ActiveQuestionnaire[]
    }>(`${this.baseUrl}/dashboard-data`);
  }

  /**
   * Fetches an active questionnaire by its ID.
   * @param id The ID of the active questionnaire.
   * @returns An observable containing the active questionnaire or null.
   */
  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    return this.httpClient.get<ActiveQuestionnaire | null>(`${this.baseUrl}/active-questionnaires/${id}`);
  }

  /**
   * Fetches the list of students.
   * @returns An observable containing the list of students.
   */
  getStudents(): Observable<User[]> {
    return this.httpClient.get<User[]>(`${this.baseUrl}/students`);
  }

  /**
   * Adds a student to a questionnaire.
   * @param studentId The ID of the student to add.
   * @returns An observable.
   */
  addStudentToQuestionnaire(studentId: number): Observable<void> {
    return this.httpClient.post<void>(`${this.baseUrl}/questionnaires/add-student`, { studentId });
  }

  /**
   * Fetches the questions for the user.
   * @returns An observable containing the list of questions.
   */
  getQuestionsForUser(): Observable<Question[]> {
    return this.httpClient.get<Question[]>(`${this.baseUrl}/questions`);
  }

  /**
   * Checks if a student is currently part of an active questionnaire.
   * @param studentId The ID of the student.
   * @returns An observable containing a boolean indicating if the student is in a questionnaire.
   */
  isStudentInQuestionnaire(studentId: number): Observable<boolean> {
    return this.httpClient.get<boolean>(`${this.baseUrl}/questionnaires/student/${studentId}/exists`);
  }

  /**
   * Submits data for a user.
   * @param userId The ID of the user.
   * @param role The role of the user (student or teacher).
   * @param questionnaireId The ID of the questionnaire.
   * @returns An observable.
   */
  submitData(userId: number, role: string, questionnaireId: string): Observable<void> {
    return this.httpClient.post<void>(`${this.baseUrl}/questionnaires/${questionnaireId}/submit`, { userId, role });
  }

  /**  
   * Creates a new active questionnaire.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   * @returns An observable containing the created active questionnaire.
   */
  createActiveQuestionnaire(studentId: number, teacherId: number): Observable<ActiveQuestionnaire> {
    return this.httpClient.post<ActiveQuestionnaire>(`${this.baseUrl}/active-questionnaires`, { studentId, teacherId });
  }

  /**
   * Fetches the list of active questionnaires.
   * @returns An observable containing the list of active questionnaires.
   */
  getActiveQuestionnaires(): Observable<ActiveQuestionnaire[]> {
    return this.httpClient.get<ActiveQuestionnaire[]>(`${this.baseUrl}/active-questionnaires`);
  }

  /**
   * Deletes an active questionnaire by its ID.
   * @param id The ID of the active questionnaire to delete.
   * @returns An observable.
   */
  deleteActiveQuestionnaire(id: string): Observable<void> {
    return this.httpClient.delete<void>(`${this.baseUrl}/active-questionnaires/${id}`);
  }
}