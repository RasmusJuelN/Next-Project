import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Question, User } from '../../models/questionare';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private baseUrl: string = `${environment.apiUrl}`;

  httpClient = inject(HttpClient )

  // Note: this has not been tested as backend isn't ready yet
  getStudents(): Observable<User[]> {
    return this.httpClient.get<User[]>(`${this.baseUrl}/students`);
  }

  // Note: this has not been tested as backend isn't ready yet
  getQuestionsForUser(userId: number): Observable<Question[]> {
    return this.httpClient.get<Question[]>(`${this.baseUrl}/questions?userId=${userId}`);
  }
  getQuestionFromId(questionId: number): Observable<Question> {
    return this.httpClient.get<Question>(`${this.baseUrl}/questions/${questionId}`);
  }
}