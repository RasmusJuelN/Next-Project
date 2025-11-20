import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { ActiveQuestionnaire, ActiveQuestionnaireBase, ResponseActiveQuestionnaireBase, TemplateBaseResponse, UserPaginationResult } from '../models/active.models';
import { User, Role } from '../../../shared/models/user.model';
import { Template, TemplateBase, TemplateStatus } from '../../../shared/models/template.model';


@Injectable({
  providedIn: 'root',
})
export class MockActiveService {
private sessionOffsets = new Map<string, number>();
private mockUsers: User[] = [
  { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: Role.Student },
  { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: Role.Student },
  { id: 's3', userName: 'jack789', fullName: 'Jack Brown', role: Role.Student},
  { id: 's4', userName: 'jessC', fullName: 'Jessica Carter', role: Role.Student },
  { id: 's5', userName: 'juliaS', fullName: 'Julia Sanchez', role: Role.Student },
  { id: 's6', userName: 'joelM', fullName: 'Joel Martinez', role: Role.Student},
  { id: 's7', userName: 'jacobR', fullName: 'Jacob Robinson', role: Role.Student },
  { id: 's8', userName: 'jordanD', fullName: 'Jordan Davis', role: Role.Student },
  { id: 's9', userName: 'jimB', fullName: 'Jim Brooks', role: Role.Student },
  { id: 's10', userName: 'jasmineW', fullName: 'Jasmine White', role: Role.Student },

  { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: Role.Teacher },
  { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: Role.Teacher },
  { id: 't3', userName: 'jacksonT', fullName: 'Ms. Jackson', role: Role.Teacher },
  { id: 't4', userName: 'jonesT', fullName: 'Dr. Jones', role: Role.Teacher },
  { id: 't5', userName: 'jamesT', fullName: 'Mr. James', role: Role.Teacher },
  { id: 't6', userName: 'jacobsT', fullName: 'Mrs. Jacobs', role: Role.Teacher },
  { id: 't7', userName: 'juliaT', fullName: 'Professor Julia', role: Role.Teacher },
  { id: 't8', userName: 'joanneT', fullName: 'Ms. Joanne', role: Role.Teacher },
  { id: 't9', userName: 'jaredT', fullName: 'Mr. Jared', role: Role.Teacher},
  { id: 't10', userName: 'julianT', fullName: 'Dr. Julian', role: Role.Teacher },
];


  private mockTemplates: Template[] = [
    { id: 't101', title: 'Math Quiz', description: 'A basic math quiz', questions: [], templateStatus:TemplateStatus.Finalized },
    { id: 't102', title: 'History Quiz', description: 'A history knowledge test', questions: [], templateStatus:TemplateStatus.Finalized},
    { id: 't103', title: 'Nature Quiz', description: 'A Nature knowledge test', questions: [],templateStatus:TemplateStatus.Draft}
  ];

  private activeQuestionnaires: ActiveQuestionnaireBase[] = [
    {
      id: 'q1',
      title: 'Math Quiz',
      activatedAt: new Date('2024-02-10T12:00:00'),
      student: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: Role.Student },
      teacher: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: Role.Teacher },
      studentCompletedAt: null,
      teacherCompletedAt: new Date('2024-02-10T14:30:00')
    },
    {
      id: 'q2',
      title: 'History Quiz',
      activatedAt: new Date('2024-02-10T12:30:00'),
      student: { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: Role.Student },
      teacher: { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: Role.Teacher },
      studentCompletedAt: new Date('2024-02-10T13:00:00'),
      teacherCompletedAt: new Date('2024-02-10T13:15:00')
    },
    {
      id: 'q3',
      title: 'Science Exam',
      activatedAt: new Date('2024-02-11T09:15:00'),
      student: { id: 's3', userName: 'markS99', fullName: 'Mark Spencer', role: Role.Student },
      teacher: { id: 't3', userName: 'leeT', fullName: 'Dr. Lee', role: Role.Teacher },
      studentCompletedAt: null,
      teacherCompletedAt: null
    },
    {
      id: 'q4',
      title: 'English Grammar Test',
      activatedAt: new Date('2024-02-12T14:00:00'),
      student: { id: 's4', userName: 'emilyG', fullName: 'Emily Green', role: Role.Student },
      teacher: { id: 't4', userName: 'wilsonT', fullName: 'Ms. Wilson', role: Role.Teacher },
      studentCompletedAt: new Date('2024-02-12T14:45:00'),
      teacherCompletedAt: null
    },
    {
      id: 'q5',
      title: 'Geography Quiz',
      activatedAt: new Date('2024-02-13T08:30:00'),
      student: { id: 's5', userName: 'lucasB', fullName: 'Lucas Brown', role: Role.Student },
      teacher: { id: 't5', userName: 'hallT', fullName: 'Mr. Hall', role: Role.Teacher },
      studentCompletedAt: null,
      teacherCompletedAt: new Date('2024-02-13T10:00:00')
    },
    {
      id: 'q6',
      title: 'Physics Assessment',
      activatedAt: new Date('2024-02-14T11:10:00'),
      student: { id: 's6', userName: 'sophiaW', fullName: 'Sophia White', role: Role.Student },
      teacher: { id: 't6', userName: 'jonesT', fullName: 'Dr. Jones', role: Role.Teacher },
      studentCompletedAt: new Date('2024-02-14T11:55:00'),
      teacherCompletedAt: new Date('2024-02-14T12:15:00')
    },
    {
      id: 'q7',
      title: 'Chemistry Lab Test',
      activatedAt: new Date('2024-02-15T16:45:00'),
      student: { id: 's7', userName: 'oliverR', fullName: 'Oliver Reynolds', role: Role.Student },
      teacher: { id: 't7', userName: 'smithJ', fullName: 'Mr. Smith', role: Role.Teacher },
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
      activeQuestionnaireBases: paginatedData,
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
      title: template?.title ?? 'unknown',
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
searchUsers(
  term: string,
  role: 'student' | 'teacher',
  pageSize: number,
  sessionId?: string
): Observable<UserPaginationResult> {

  const query = term.trim().toLowerCase();
  const filtered = this.mockUsers.filter(u =>
    (u.userName.toLowerCase().includes(query) ||
     u.fullName.toLowerCase().includes(query)) &&
    u.role.toLowerCase() === role                     // 'student' | 'teacher'
  );

  // Track pagination by sessionId
  let id = sessionId;
  let offset = 0;

  if (id && this.sessionOffsets.has(id)) {
    offset = this.sessionOffsets.get(id)!;
  } else {
    id = crypto.randomUUID();
  }

  const slice = filtered.slice(offset, offset + pageSize);
  const hasMore = offset + slice.length < filtered.length;

  this.sessionOffsets.set(id, offset + slice.length); // advance pointer

  return of({
    userBases: slice,
    sessionId: id,
    hasMore
  }).pipe(delay(300));
}
  
searchTemplates(term: string, queryCursor: string = ''): Observable<TemplateBaseResponse> {
  const pageSize = 5;
  const q = term.trim().toLowerCase();

  // 1️⃣ filter
  let filtered = this.mockTemplates.filter(t =>
    t.templateStatus === TemplateStatus.Finalized && (
      t.title.toLowerCase().includes(q) ||
      (t.description ?? '').toLowerCase().includes(q)
    )
  );

  // 2️⃣ deterministic order
  filtered = filtered.sort((a, b) => a.title.localeCompare(b.title));

  // 3️⃣ cursor → offset
  let start = 0;
  if (queryCursor) {
    const [cursorTitle, cursorId] = queryCursor.split('_');
    const idx = filtered.findIndex(t => t.title === cursorTitle && t.id === cursorId);
    if (idx !== -1) start = idx + 1;
  }

  const slice = filtered.slice(start, start + pageSize);

  const templateBases: TemplateBase[] = slice.map(t => ({
    id: t.id!,
    title: t.title,
    createdAt: (t as any).createdAt ?? new Date().toISOString(),
    lastUpdated: (t as any).lastUpdated ?? new Date().toISOString(),
    isLocked: (t as any).isLocked ?? false,
    templateStatus: t.templateStatus
  }));

  const hasMore = start + slice.length < filtered.length;
  const nextCursor =
    hasMore && slice.length
      ? `${slice[slice.length - 1].title}_${slice[slice.length - 1].id}`
      : '';

  return of({
    templateBases,
    queryCursor: nextCursor,
    totalCount: filtered.length
  }).pipe(delay(300));
}
}