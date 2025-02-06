import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { QuestionnaireSession } from '../models/active.models';
import { User } from '../../../shared/models/user.model';
import { Template } from '../../template-manager/models/template.model';

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
    { id: 't101', title: 'Math Quiz', description: 'A basic math quiz', questions: [] },
    { id: 't102', title: 'History Quiz', description: 'A history knowledge test', questions: [] }
  ];

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
    {
      id: 'q3',
      templateId: 't103',
      templateName: 'Science Exam',
      createdAt: new Date('2024-02-11T09:15:00'),
      updatedAt: new Date(),
  
      student: {
        user: { id: 's3', userName: 'markS99', fullName: 'Mark Spencer', role: 'test' },
        answered: false,
        answeredWhen: null,
      },
  
      teacher: {
        user: { id: 't3', userName: 'leeT', fullName: 'Dr. Lee', role: 'test' },
        answered: false,
        answeredWhen: null,
      },
    },
    {
      id: 'q4',
      templateId: 't104',
      templateName: 'English Grammar Test',
      createdAt: new Date('2024-02-12T14:00:00'),
      updatedAt: new Date(),
  
      student: {
        user: { id: 's4', userName: 'emilyG', fullName: 'Emily Green', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-12T14:45:00'),
      },
  
      teacher: {
        user: { id: 't4', userName: 'wilsonT', fullName: 'Ms. Wilson', role: 'test' },
        answered: false,
        answeredWhen: null,
      },
    },
    {
      id: 'q5',
      templateId: 't105',
      templateName: 'Geography Quiz',
      createdAt: new Date('2024-02-13T08:30:00'),
      updatedAt: new Date(),
  
      student: {
        user: { id: 's5', userName: 'lucasB', fullName: 'Lucas Brown', role: 'test' },
        answered: false,
        answeredWhen: null,
      },
  
      teacher: {
        user: { id: 't5', userName: 'hallT', fullName: 'Mr. Hall', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-13T10:00:00'),
      },
    },
    {
      id: 'q6',
      templateId: 't106',
      templateName: 'Physics Assessment',
      createdAt: new Date('2024-02-14T11:10:00'),
      updatedAt: new Date(),
  
      student: {
        user: { id: 's6', userName: 'sophiaW', fullName: 'Sophia White', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-14T11:55:00'),
      },
  
      teacher: {
        user: { id: 't6', userName: 'jonesT', fullName: 'Dr. Jones', role: 'test' },
        answered: true,
        answeredWhen: new Date('2024-02-14T12:15:00'),
      },
    },
    {
      id: 'q7',
      templateId: 't107',
      templateName: 'Chemistry Lab Test',
      createdAt: new Date('2024-02-15T16:45:00'),
      updatedAt: new Date(),
  
      student: {
        user: { id: 's7', userName: 'oliverR', fullName: 'Oliver Reynolds', role: 'test' },
        answered: false,
        answeredWhen: null,
      },
  
      teacher: {
        user: { id: 't7', userName: 'smithJ', fullName: 'Mr. Smith', role: 'test' },
        answered: false,
        answeredWhen: null,
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
    }).pipe(delay(2000));
  }
  getActiveQuestionnaireById(id: string): Observable<QuestionnaireSession | undefined> {
    const questionnaire = this.activeQuestionnaires.find(q => q.id === id);
    return of(questionnaire);
  }

  createActiveQuestionnaire(data: { studentId: string; teacherId: string; templateId: string }): Observable<QuestionnaireSession | null> {
    // Find the existing student, teacher, and template
    const student = this.mockUsers.find(user => user.id === data.studentId && user.role === 'student');
    const teacher = this.mockUsers.find(user => user.id === data.teacherId && user.role === 'teacher');
    const template = this.mockTemplates.find(t => t.id === data.templateId);

    // Ensure all required entities exist
    if (!student || !teacher || !template) {
      console.error('Invalid student, teacher, or template ID');
      return of(null);
    }

    const newQuestionnaire: QuestionnaireSession = {
      id: `q${this.activeQuestionnaires.length + 1}`,
      templateId: template.id,
      templateName: template.title,
      createdAt: new Date(),
      updatedAt: new Date(),
      student: {
        user: student,
        answered: false,
        answeredWhen: null,
      },
      teacher: {
        user: teacher,
        answered: false,
        answeredWhen: null,
      },
    };

    this.activeQuestionnaires.push(newQuestionnaire);
    return of(newQuestionnaire);
  }

  updateActiveQuestionnaire(id: string, data: Partial<QuestionnaireSession>): Observable<QuestionnaireSession | undefined> {
    const index = this.activeQuestionnaires.findIndex(q => q.id === id);
    if (index !== -1) {
      this.activeQuestionnaires[index] = { ...this.activeQuestionnaires[index], ...data, updatedAt: new Date() };
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
    let filteredUsers = this.mockUsers.filter(user =>
      user.role === role && (user.fullName.toLowerCase().includes(term.toLowerCase()) || user.userName.toLowerCase().includes(term.toLowerCase()))
    );
    const totalItems = filteredUsers.length;
    const startIndex = (page - 1) * 10;
    const paginatedUsers = filteredUsers.slice(startIndex, startIndex + 10);
    return of({ items: paginatedUsers, totalItems, currentPage: page, pageSize: 10, totalPages: Math.ceil(totalItems / 10) }).pipe(delay(2000));;
  }

  searchTemplates(term: string, page: number): Observable<PaginationResponse<Template>> {
    let filteredTemplates = this.mockTemplates.filter(template =>
      template.title.toLowerCase().includes(term.toLowerCase()) || template.description.toLowerCase().includes(term.toLowerCase())
    );
    const totalItems = filteredTemplates.length;
    const startIndex = (page - 1) * 10;
    const paginatedTemplates = filteredTemplates.slice(startIndex, startIndex + 10);
    return of({ items: paginatedTemplates, totalItems, currentPage: page, pageSize: 10, totalPages: Math.ceil(totalItems / 10) });
  }
}