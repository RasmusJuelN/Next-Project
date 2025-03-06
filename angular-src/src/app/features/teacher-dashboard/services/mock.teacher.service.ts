// mock-teacher.service.ts
import { Injectable } from '@angular/core';
import { Dashboard } from '../models/dashboard.model';
import { delay, Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';

@Injectable({
  providedIn: 'root',
})
export class MockTeacherService {

  private readonly MOCK_DATA: Dashboard[] = Array.from({ length: 40 }, (_, i) => {
    // For the first item, set activeQuestionaireUrlLink to 'active1'
    if (i === 0) {
      return {
        activeQuestionaireUrlLink: 'active1',
        studentName: 'Student 1',
        studentCompleted: true,
        teacherCompleted: false,
      };
    } else {
      // For the remaining items, keep the original logic
      return {
        activeQuestionaireUrlLink: `${i + 1}`,
        studentName: `Student ${i + 1}`,
        studentCompleted: Math.random() > 0.5,
        teacherCompleted: Math.random() > 0.5,
      };
    }
  });

  getQuestionnaires(
    searchTerm: string,
    searchType: string,
    currentPage: number,
    pageSize: number,
    filterStudentCompleted: boolean,
    filterTeacherCompleted: boolean
  ): Observable<PaginationResponse<Dashboard>> {
    let filtered = [...this.MOCK_DATA];

    if (searchTerm) {
      const lowerTerm = searchTerm.toLowerCase();
      if (searchType === 'name') {
        filtered = filtered.filter(q => q.studentName.toLowerCase().includes(lowerTerm));
      } else {
        filtered = filtered.filter(q => q.activeQuestionaireUrlLink.toLowerCase().includes(lowerTerm));
      }
    }

    if (filterStudentCompleted) {
      filtered = filtered.filter(q => q.studentCompleted === true);
    }
    if (filterTeacherCompleted) {
      filtered = filtered.filter(q => q.teacherCompleted === true);
    }

    const totalItems = filtered.length;
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const items = filtered.slice(startIndex, endIndex);

    const totalPages = Math.ceil(totalItems / pageSize);

    const response: PaginationResponse<Dashboard> = {
      items,
      totalItems,
      currentPage,
      pageSize,
      totalPages
    };
    return of(response).pipe(delay(2000));;
  }
}
