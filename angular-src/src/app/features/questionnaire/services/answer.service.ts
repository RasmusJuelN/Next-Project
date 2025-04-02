import { Injectable } from '@angular/core';
import { AnswerSubmission, Questionnaire } from '../models/answer.model';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AnswerService {
  private apiUrl = environment.apiUrl; // Replace with your actual API URL

  constructor(private http: HttpClient) {}

  submitAnswers(id: string, answers: AnswerSubmission): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/active-questionnaire/${id}/submitanswer`, answers);
  }

  hasUserSubmited(id:string){
    return this.http.get<boolean>(`${this.apiUrl}/active-questionnaire/${id}/isAnswered`);
  }

  // Get active questionnaire by instance ID
  getActiveQuestionnaireById(instanceId: string): Observable<Questionnaire> {
    return this.http.get<Questionnaire>(`${this.apiUrl}/active-questionnaire/${instanceId}`);
  }
}
