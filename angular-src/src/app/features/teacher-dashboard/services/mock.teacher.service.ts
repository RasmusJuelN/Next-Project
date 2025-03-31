import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { ActiveQuestionnaireBase, ActiveQuestionnaireResponse } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root',
})
export class MockTeacherService {
  // Generate mock data as ActiveQuestionnaireBase objects.
  private readonly MOCK_DATA: ActiveQuestionnaireBase[] = Array.from({ length: 40 }, (_, i) => {
    const now = new Date();
    if (i === 0) {
      return {
        id: 'active1',
        title: 'Questionnaire 1',
        description: 'Description for Questionnaire 1',
        activatedAt: now,
        studentCompletedAt: new Date(now.getTime() - 3600 * 1000), // Completed 1 hour ago
        teacherCompletedAt: null,
      };
    } else {
      return {
        id: `${i + 1}`,
        title: `Questionnaire ${i + 1}`,
        description: `Description for Questionnaire ${i + 1}`,
        activatedAt: new Date(now.getTime() - i * 3600 * 1000),
        studentCompletedAt: Math.random() > 0.5 ? new Date(now.getTime() - i * 3600 * 1000) : null,
        teacherCompletedAt: Math.random() > 0.5 ? new Date(now.getTime() - i * 3600 * 1000) : null,
      };
    }
  });

  /**
   * Mimics the real service's NEWgetQuestionnaires method with keyset pagination.
   * Uses queryCursor as a stringified start index.
   */
  getQuestionnaires(
    searchTerm: string,
    searchType: 'name' | 'id',
    queryCursor: string | null,
    pageSize: number,
    filterStudentCompleted: boolean,
    filterTeacherCompleted: boolean
  ): Observable<ActiveQuestionnaireResponse> {
    // Clone the data.
    let filtered = [...this.MOCK_DATA];

    // Apply search filter based on search type.
    if (searchTerm) {
      const lowerTerm = searchTerm.toLowerCase();
      if (searchType === 'name') {
        filtered = filtered.filter(q => q.title.toLowerCase().includes(lowerTerm));
      } else {
        filtered = filtered.filter(q => q.id.toLowerCase().includes(lowerTerm));
      }
    }

    // Apply completion filters.
    if (filterStudentCompleted) {
      filtered = filtered.filter(q => q.studentCompletedAt !== null);
    }
    if (filterTeacherCompleted) {
      filtered = filtered.filter(q => q.teacherCompletedAt !== null);
    }

    const totalCount = filtered.length;
    // For keyset pagination, treat queryCursor as a stringified start index.
    const startIndex = queryCursor ? parseInt(queryCursor, 10) : 0;
    const items = filtered.slice(startIndex, startIndex + pageSize);
    // Set nextCursor if there are more items.
    const nextCursor = (startIndex + pageSize < totalCount) ? (startIndex + pageSize).toString() : null;

    const response: ActiveQuestionnaireResponse = {
      activeQuestionnaireBases: items,
      queryCursor: nextCursor,
      totalCount: totalCount,
    };

    // Return the response with a delay to simulate an HTTP call.
    return of(response).pipe(delay(2000));
  }
}
