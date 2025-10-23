import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Result } from '../../result/models/result.model';

export interface StudentResultHistory {
  results: Result[];
  studentId: string;
  templateId: string;
  studentName: string;
  templateTitle: string;
}

@Injectable({
  providedIn: 'root'
})
export class ResultHistoryService {
  private apiUrl = `${environment.apiUrl}/active-questionnaire`;
  private apiService = inject(ApiService);

  /**
   * Get result history for a specific student and template combination
   * @param studentId - The ID of the student
   * @param templateId - The ID of the questionnaire template
   * @returns Observable of StudentResultHistory
   */
  getStudentResultHistory(studentId: string, templateId: string): Observable<StudentResultHistory> {
    // TODO: Replace with actual API call when backend endpoint is ready
    // return this.apiService.get<StudentResultHistory>(`${this.apiUrl}/student-history/${studentId}/template/${templateId}`);
    
    // For now, return mock data
    return of({
      results: [],
      studentId,
      templateId,
      studentName: 'Mock Student',
      templateTitle: 'Mock Template'
    });
  }

  /**
   * Get all results for a specific student across all templates
   * @param studentId - The ID of the student
   * @returns Observable of Result array
   */
  getAllStudentResults(studentId: string): Observable<Result[]> {
    // TODO: Replace with actual API call when backend endpoint is ready
    // return this.apiService.get<Result[]>(`${this.apiUrl}/student/${studentId}/all-results`);
    
    // For now, return empty array
    return of([]);
  }

  /**
   * Get result by ID - delegates to the existing result service
   * @param resultId - The ID of the result
   * @returns Observable of Result
   */
  getResultById(resultId: string): Observable<Result> {
    return this.apiService.get<Result>(`${this.apiUrl}/${resultId}/getresponse`);
  }
}