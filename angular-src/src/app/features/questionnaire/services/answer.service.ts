import { Injectable } from '@angular/core';
import { AnswerSubmission, Questionnaire } from '../models/answer.model';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';


/**
 * Answer service.
 *
 * Provides operations for answering active questionnaires.
 *
 * Handles:
 * - Submitting questionnaire answers.
 * - Checking if the current user has already submitted.
 * - Fetching an active questionnaire by its instance ID.
 */
@Injectable({
  providedIn: 'root',
})
export class AnswerService {
  private apiUrl = environment.apiUrl; // Replace with your actual API URL

  constructor(private http: HttpClient) {}

  /**
   * Submits a completed questionnaire.
   * @param id The active questionnaire ID.
   * @param answers The answers payload to submit.
   */
  submitAnswers(id: string, answers: AnswerSubmission): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/active-questionnaire/${id}/submitanswer`, answers);
  }

  /**
   * Checks whether the current user has already submitted
   * a questionnaire with the given ID.
   * @param id The active questionnaire ID.
   */
  hasUserSubmited(id:string){
    return this.http.get<boolean>(`${this.apiUrl}/active-questionnaire/${id}/isAnswered`);
  }

  /**
   * Fetches the active questionnaire template by instance ID.
   * @param instanceId The questionnaire instance ID.
   */
  getActiveQuestionnaireById(instanceId: string): Observable<Questionnaire> {
    return this.http.get<Questionnaire>(`${this.apiUrl}/active-questionnaire/${instanceId}`);
  }
}
