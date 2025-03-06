import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Result } from '../models/result.model';

@Injectable({
  providedIn: 'root'
})
export class MockResultService {

  private mockResults: Result[] = [
    {
      id: 'ActiveQuest23',
      templateName: 'Math Quiz',
      student: {
        user: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role:"test" },
        answeredWhen: new Date('2025-01-15'),
      },
      teacher: {
        user: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role:"test" },
        answeredWhen: new Date('2025-01-16'),
      },
      answers: [
        {
          question: 'What is Angular?',
          studentAnswer: 'A JavaScript framework for building applications.',
          isStudentCustomAnswer: true, // Student typed a custom response
          teacherAnswer: 'A framework developed by Google for creating SPAs.',
          isTeacherCustomAnswer: false, // Teacher selected from predefined options
        },
        {
          question: 'What is 2 + 2?',
          studentAnswer: 'Four',
          isStudentCustomAnswer: true, // Student manually typed "Four"
          teacherAnswer: '4',
          isTeacherCustomAnswer: false, // Teacher picked from predefined options
        },
      ],
    },
    {
      id: 'ActiveQuest24',
      templateName: 'Science Quiz',
      student: {
        user: { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role:"test" },
        answeredWhen: new Date('2025-02-01'),
      },
      teacher: {
        user: { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role:"test" },
        answeredWhen: new Date('2025-02-02'),
      },
      answers: [
        {
          question: 'What is the chemical symbol for water?',
          studentAnswer: 'H2O',
          isStudentCustomAnswer: false, // Student selected from predefined options
          teacherAnswer: 'H2O',
          isTeacherCustomAnswer: false, // Teacher picked from predefined options
        },
        {
          question: 'What planet is known as the Red Planet?',
          studentAnswer: 'Mars',
          isStudentCustomAnswer: false, // Student selected from predefined options
          teacherAnswer: 'Mars',
          isTeacherCustomAnswer: false, // Teacher picked from predefined options
        },
      ],
    }
  ];

  getResultById(id: string): Observable<Result | null> {
    const result = this.mockResults.find(r => r.id === id);
    return of(result ?? null); // Returns null if result not found
  }
}
