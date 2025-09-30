import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Result } from '../models/result.model';
import { Role } from '../../../shared/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class MockResultService {

  private mockResults: Result[] = [
    {
      id: 'ActiveQuest23',
      title: 'Math Quiz',
      description: null,
      student: {
        user: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: Role.Student },
        completedAt: new Date('2025-03-24T10:00:00')
      },
      teacher: {
        user: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: Role.Teacher },
        completedAt: new Date('2025-03-24T10:05:00')
      },
      answers: [
        {
          question: 'What is Angular?',
          studentResponse: 'A JavaScript framework for building applications.',
          isStudentResponseCustom: true,
          teacherResponse: 'A framework developed by Google for creating SPAs.',
          isTeacherResponseCustom: false,
        },
        {
          question: 'What is 2 + 2?',
          studentResponse: 'Four',
          isStudentResponseCustom: true,
          teacherResponse: '4',
          isTeacherResponseCustom: false,
        },
      ],
    },
    {
      id: 'ActiveQuest24',
      title: 'Science Quiz',
      description: null,
      student: {
        user: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: Role.Student },
        completedAt: new Date('2025-03-23T09:00:00')
      },
      teacher: {
        user: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: Role.Teacher },
        completedAt: new Date('2025-03-23T09:05:00')
      },
      answers: [
        {
          question: 'What is the chemical symbol for water?',
          studentResponse: 'H2O',
          isStudentResponseCustom: false,
          teacherResponse: 'H2O',
          isTeacherResponseCustom: false,
        },
        {
          question: 'What planet is known as the Red Planet?',
          studentResponse: 'Mars',
          isStudentResponseCustom: false,
          teacherResponse: 'Mars',
          isTeacherResponseCustom: false,
        },
      ],
    }
  ];

  getResultById(id: string): Observable<Result | null> {
    const result = this.mockResults.find(r => r.id === id);
    return of(result ?? null); // Returns null if result not found
  }
}
