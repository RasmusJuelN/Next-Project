import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { QuestionnaireSession } from '../models/active.models';

@Injectable({
  providedIn: 'root',
})
export class MockActiveService {
  private activeQuestionnaires: QuestionnaireSession[] = [
    {
      id: 'q1',
      templateId: 't101',
      templateName: 'Math Quiz',
      createdAt: new Date('2024-02-10T12:00:00'),
      updatedAt: new Date(),

      student: {
        user: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: 'test' },
        answered: false,
        answeredWhen: null,
      },

      teacher: {
        user: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-10T14:30:00'),
      },
    },
    {
      id: 'q2',
      templateId: 't102',
      templateName: 'History Quiz',
      createdAt: new Date('2024-02-10T12:30:00'),
      updatedAt: new Date(),

      student: {
        user: { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-10T13:00:00'),
      },

      teacher: {
        user: { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-10T13:15:00'),
      },
    },
  ];

  constructor() {}

  getActiveQuestionnaires(
    page: number,
    pageSize: number,
    searchStudent: string = '',
    searchStudentType: 'fullName' | 'userName' | 'both' = 'both',
    searchTeacher: string = '',
    searchTeacherType: 'fullName' | 'userName' | 'both' = 'both'
  ): Observable<PaginationResponse<QuestionnaireSession>> {
    let filteredData = [...this.activeQuestionnaires];

    const filterByType = (value: string, search: string, type: 'fullName' | 'userName' | 'both') => {
      if (type === 'fullName') return value.toLowerCase().includes(search.toLowerCase());
      if (type === 'userName') return value.toLowerCase().includes(search.toLowerCase());
      return (
        value.toLowerCase().includes(search.toLowerCase()) ||
        value.toLowerCase().includes(search.toLowerCase())
      );
    };

    // Apply search filters
    if (searchStudent.trim()) {
      filteredData = filteredData.filter(q =>
        filterByType(q.student.user.fullName, searchStudent, searchStudentType) ||
        filterByType(q.student.user.userName, searchStudent, searchStudentType)
      );
    }

    if (searchTeacher.trim()) {
      filteredData = filteredData.filter(q =>
        filterByType(q.teacher.user.fullName, searchTeacher, searchTeacherType) ||
        filterByType(q.teacher.user.userName, searchTeacher, searchTeacherType)
      );
    }

    // Pagination Logic
    const totalItems = filteredData.length;
    const startIndex = (page - 1) * pageSize;
    const paginatedData = filteredData.slice(startIndex, startIndex + pageSize);

    // Return paginated response
    return of({
      items: paginatedData,
      totalItems: totalItems,
      currentPage: page,
      pageSize: pageSize,
      totalPages: Math.ceil(totalItems / pageSize),
    });
  }
}