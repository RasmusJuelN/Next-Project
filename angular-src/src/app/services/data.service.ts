import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Question, User } from '../models/questionare';
import { environment } from '../../environments/environment';

/**
 * Service for handling data operations.
 */
@Injectable({
  providedIn: 'root'
})
export class DataService {

  private baseUrl: string = `${environment.apiUrl}`;

  httpClient = inject(HttpClient )

  /**
   * Retrieves a list of students.
   * Note: this has not been tested as the backend isn't ready yet.
   * @returns An Observable of User[] representing the list of students.
   */
  getStudents(): Observable<User[]> {
    return this.httpClient.get<User[]>(`${this.baseUrl}/students`);
  }

  /**
   * Retrieves a list of questions for a specific user.
   * Note: this has not been tested as the backend isn't ready yet.
   * @param userId - The ID of the user.
   * @returns An Observable of Question[] representing the list of questions.
   */
  getQuestionsForUser(userId: number): Observable<Question[]> {
    return this.httpClient.get<Question[]>(`${this.baseUrl}/questions?userId=${userId}`);
  }

  /**
   * Retrieves a question based on its ID.
   * @param questionId - The ID of the question.
   * @returns An Observable of Question representing the question.
   */
  getQuestionFromId(questionId: number): Observable<Question> {
    return this.httpClient.get<Question>(`${this.baseUrl}/questions/?id=${questionId}`);
  }
}