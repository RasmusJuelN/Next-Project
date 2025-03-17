import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { ActiveQuestionnaire, ActiveQuestionnaireBase, ResponseActiveQuestionnaireBase, Template } from '../models/active.models';
import { User } from '../../../shared/models/user.model';


@Injectable({
  providedIn: 'root',
})
export class MockActiveService {
private mockUsers: User[] = [
  { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: 'student' },
  { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: 'student' },
  { id: 's3', userName: 'jack789', fullName: 'Jack Brown', role: 'student' },
  { id: 's4', userName: 'jessC', fullName: 'Jessica Carter', role: 'student' },
  { id: 's5', userName: 'juliaS', fullName: 'Julia Sanchez', role: 'student' },
  { id: 's6', userName: 'joelM', fullName: 'Joel Martinez', role: 'student' },
  { id: 's7', userName: 'jacobR', fullName: 'Jacob Robinson', role: 'student' },
  { id: 's8', userName: 'jordanD', fullName: 'Jordan Davis', role: 'student' },
  { id: 's9', userName: 'jimB', fullName: 'Jim Brooks', role: 'student' },
  { id: 's10', userName: 'jasmineW', fullName: 'Jasmine White', role: 'student' },

  { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: 'teacher' },
  { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: 'teacher' },
  { id: 't3', userName: 'jacksonT', fullName: 'Ms. Jackson', role: 'teacher' },
  { id: 't4', userName: 'jonesT', fullName: 'Dr. Jones', role: 'teacher' },
  { id: 't5', userName: 'jamesT', fullName: 'Mr. James', role: 'teacher' },
  { id: 't6', userName: 'jacobsT', fullName: 'Mrs. Jacobs', role: 'teacher' },
  { id: 't7', userName: 'juliaT', fullName: 'Professor Julia', role: 'teacher' },
  { id: 't8', userName: 'joanneT', fullName: 'Ms. Joanne', role: 'teacher' },
  { id: 't9', userName: 'jaredT', fullName: 'Mr. Jared', role: 'teacher' },
  { id: 't10', userName: 'julianT', fullName: 'Dr. Julian', role: 'teacher' },
];


  private mockTemplates: Template[] = [
    { id: 't101', templateTitle: 'Math Quiz', description: 'A basic math quiz', questions: [] },
    { id: 't102', templateTitle: 'History Quiz', description: 'A history knowledge test', questions: [] }
  ];

  private activeQuestionnaires: ActiveQuestionnaireBase[] = [
    {
      id: 'q1',
      title: 'Math Quiz',
      activatedAt: new Date('2024-02-10T12:00:00'),
      student: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: 'test' },
      teacher: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: 'test' },
      studentCompletedAt: null,
      teacherCompletedAt: new Date('2024-02-10T14:30:00')
    },
    {
      id: 'q2',
      title: 'History Quiz',
      activatedAt: new Date('2024-02-10T12:30:00'),
      student: { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: 'test' },
      teacher: { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: 'test' },
      studentCompletedAt: new Date('2024-02-10T13:00:00'),
      teacherCompletedAt: new Date('2024-02-10T13:15:00')
    },
    {
      id: 'q3',
      title: 'Science Exam',
      activatedAt: new Date('2024-02-11T09:15:00'),
      student: { id: 's3', userName: 'markS99', fullName: 'Mark Spencer', role: 'test' },
      teacher: { id: 't3', userName: 'leeT', fullName: 'Dr. Lee', role: 'test' },
      studentCompletedAt: null,
      teacherCompletedAt: null
    },
    {
      id: 'q4',
      title: 'English Grammar Test',
      activatedAt: new Date('2024-02-12T14:00:00'),
      student: { id: 's4', userName: 'emilyG', fullName: 'Emily Green', role: 'test' },
      teacher: { id: 't4', userName: 'wilsonT', fullName: 'Ms. Wilson', role: 'test' },
      studentCompletedAt: new Date('2024-02-12T14:45:00'),
      teacherCompletedAt: null
    },
    {
      id: 'q5',
      title: 'Geography Quiz',
      activatedAt: new Date('2024-02-13T08:30:00'),
      student: { id: 's5', userName: 'lucasB', fullName: 'Lucas Brown', role: 'test' },
      teacher: { id: 't5', userName: 'hallT', fullName: 'Mr. Hall', role: 'test' },
      studentCompletedAt: null,
      teacherCompletedAt: new Date('2024-02-13T10:00:00')
    },
    {
      id: 'q6',
      title: 'Physics Assessment',
      activatedAt: new Date('2024-02-14T11:10:00'),
      student: { id: 's6', userName: 'sophiaW', fullName: 'Sophia White', role: 'test' },
      teacher: { id: 't6', userName: 'jonesT', fullName: 'Dr. Jones', role: 'test' },
      studentCompletedAt: new Date('2024-02-14T11:55:00'),
      teacherCompletedAt: new Date('2024-02-14T12:15:00')
    },
    {
      id: 'q7',
      title: 'Chemistry Lab Test',
      activatedAt: new Date('2024-02-15T16:45:00'),
      student: { id: 's7', userName: 'oliverR', fullName: 'Oliver Reynolds', role: 'test' },
      teacher: { id: 't7', userName: 'smithJ', fullName: 'Mr. Smith', role: 'test' },
      studentCompletedAt: null,
      teacherCompletedAt: null
    }
  ];
  

  constructor() {}

  getActiveQuestionnaires(
    pageSize: number,
    queryCursor: string = '',
    studentSearch: string = '',
    studentSearchType: 'fullName' | 'userName' | 'both' = 'both',
    teacherSearch: string = '',
    teacherSearchType: 'fullName' | 'userName' | 'both' = 'both'
  ): Observable<ResponseActiveQuestionnaireBase> {
    // Start with a copy of all active questionnaires.
    let filteredData = [...this.activeQuestionnaires];

    // Apply student search filter if provided.
    if (studentSearch.trim()) {
      filteredData = filteredData.filter(q =>
        this.filterByType(q.student.fullName, studentSearch, studentSearchType) ||
        this.filterByType(q.student.userName, studentSearch, studentSearchType)
      );
    }

    // Apply teacher search filter if provided.
    if (teacherSearch.trim()) {
      filteredData = filteredData.filter(q =>
        this.filterByType(q.teacher.fullName, teacherSearch, teacherSearchType) ||
        this.filterByType(q.teacher.userName, teacherSearch, teacherSearchType)
      );
    }

    // Sort data by activatedAt descending.
    filteredData.sort((a, b) => b.activatedAt.getTime() - a.activatedAt.getTime());

    // Implement keyset pagination:
    // If a queryCursor is provided, it should be in the format "activatedAt_ISO_{id}"
    let startIndex = 0;
    if (queryCursor.trim()) {
      // Attempt to find the index of the record that matches the cursor.
      const [cursorDate, cursorId] = queryCursor.split('_');
      const cursorTime = new Date(cursorDate).getTime();
      startIndex = filteredData.findIndex(q => {
        // We match if activatedAt equals cursorTime and id matches.
        return q.activatedAt.getTime() === cursorTime && q.id === cursorId;
      });
      // If found, start with the next record.
      if (startIndex !== -1) {
        startIndex = startIndex + 1;
      } else {
        startIndex = 0;
      }
    }

    // Slice the data for the given pageSize.
    const paginatedData = filteredData.slice(startIndex, startIndex + pageSize);

    // Create a new query cursor from the last record (if any).
    let newQueryCursor = '';
    if (paginatedData.length > 0) {
      const lastRecord = paginatedData[paginatedData.length - 1];
      newQueryCursor = `${lastRecord.activatedAt.toISOString()}_${lastRecord.id}`;
    }

    // Construct the response object.
    const response: ResponseActiveQuestionnaireBase = {
      activeQuestionnaireBase: paginatedData,
      queryCursor: newQueryCursor,
      totalCount: filteredData.length,
    };

    return of(response).pipe(delay(2000));
  }

  private filterByType(value: string, search: string, type: 'fullName' | 'userName' | 'both'): boolean {
    // For this mock, both cases simply do a case-insensitive search.
    return value.toLowerCase().includes(search.toLowerCase());
  }


  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaireBase | undefined> {
    const questionnaire = this.activeQuestionnaires.find(q => q.id === id);
    return of(questionnaire);
  }

  createActiveQuestionnaire(data: { studentId: string; teacherId: string; templateId: string }): Observable<ActiveQuestionnaire | null> {
    // Find the existing student, teacher, and template
    const student = this.mockUsers.find(user => user.id === data.studentId && user.role === 'student');
    const teacher = this.mockUsers.find(user => user.id === data.teacherId && user.role === 'teacher');
    const template = this.mockTemplates.find(t => t.id === data.templateId);

    // Ensure all required entities exist
    if (!student || !teacher || !template) {
      console.error('Invalid student, teacher, or template ID');
      return of(null);
    }

    const newQuestionnaire: ActiveQuestionnaire = {
      id: `q${this.activeQuestionnaires.length + 1}`,
      title: template?.templateTitle ?? 'unknown',
      activatedAt: new Date(),
      student: student,
      teacher: teacher,
      studentCompletedAt: null,
      teacherCompletedAt: null,
    };
    

    this.activeQuestionnaires.push(newQuestionnaire);
    return of(newQuestionnaire);
  }

  updateActiveQuestionnaire(id: string, data: Partial<ActiveQuestionnaire>): Observable<ActiveQuestionnaireBase | undefined> {
    const index = this.activeQuestionnaires.findIndex(q => q.id === id);
    if (index !== -1) {
      this.activeQuestionnaires[index] = { ...this.activeQuestionnaires[index], ...data };
      return of(this.activeQuestionnaires[index]);
    }
    return of(undefined);
  }
  

  deleteActiveQuestionnaire(id: string): Observable<boolean> {
    const index = this.activeQuestionnaires.findIndex(q => q.id === id);
    if (index !== -1) {
      this.activeQuestionnaires.splice(index, 1);
      return of(true);
    }
    return of(false);
  }
  searchUsers(term: string, role: 'student' | 'teacher', page: number): Observable<PaginationResponse<User>> {
    // Normalize search term
    const normalizedTerm = term.trim().toLowerCase();
  
    // Filter users by role and search term (checking both fullName & userName)
    let filteredUsers = this.mockUsers.filter(user =>
      user.role === role &&
      (user.fullName.toLowerCase().includes(normalizedTerm) || user.userName?.toLowerCase().includes(normalizedTerm))
    );
  
    const totalItems = filteredUsers.length;
    const startIndex = Math.max(0, (page - 1) * 10);
    const paginatedUsers = filteredUsers.slice(startIndex, startIndex + 10);
  
    return of({
      items: paginatedUsers,
      totalItems,
      currentPage: page,
      pageSize: 10,
      totalPages: Math.ceil(totalItems / 10),
    }).pipe(delay(2000));
  }
  
  searchTemplates(term: string, page: number): Observable<PaginationResponse<Template>> {
    // Normalize search term
    const normalizedTerm = term.trim().toLowerCase();
  
    // Filter templates based on title or description
    let filteredTemplates = this.mockTemplates.filter(template =>
      template.templateTitle.toLowerCase().includes(normalizedTerm) || template.description?.toLowerCase().includes(normalizedTerm)
    );
  
    const totalItems = filteredTemplates.length;
    const startIndex = Math.max(0, (page - 1) * 10);
    const paginatedTemplates = filteredTemplates.slice(startIndex, startIndex + 10);
  
    return of({
      items: paginatedTemplates,
      totalItems,
      currentPage: page,
      pageSize: 10,
      totalPages: Math.ceil(totalItems / 10),
    }).pipe(delay(500)); // Optional delay for a smoother UI effect
  }
}