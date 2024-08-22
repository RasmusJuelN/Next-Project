import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, delay, map, of, throwError } from 'rxjs';
import { User, Question, StudentTeacherAnswer, ActiveQuestionnaire } from '../../models/questionare';
import { LocalStorageService } from '../misc/local-storage.service';

import { ErrorHandlingService } from '../error-handling.service';
import { JWTTokenService } from '../auth/jwt-token.service';
import { AppAuthService } from '../auth/app-auth.service';

type DashboardSection = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher' | 'searchResults';


@Injectable({
  providedIn: 'root'
})
export class MockDataService {
  private localStorageKey = 'mockData';
  private localStorageService = inject(LocalStorageService)
  private jwtTokenService = inject(JWTTokenService);
  private errorHandlingService = inject(ErrorHandlingService)
  private authService = inject(AppAuthService)
  private mockData: {
    mockStudents: User[],
    mockTeachers: User[],
    mockQuestions: Question[],
    mockStudentTeacherAnswers: StudentTeacherAnswer[],
    mockActiveQuestionnaire: ActiveQuestionnaire[]
  } = {
    mockStudents: [],
    mockTeachers: [],
    mockQuestions: [],
    mockStudentTeacherAnswers: [],
    mockActiveQuestionnaire: []
  }; 

  constructor(private http: HttpClient) {
    this.loadInitialMockData();
  }

  /**
  * Fetches paginated dashboard data for a specified section.
  * @param section - The section for which data is requested.
  * @param offset - The offset used for pagination.
  * @param limit - The maximum number of items to load (default is 5).
  * @returns An observable containing the paginated data for the requested section.
  */
  getPaginatedDashboardData(
    section: string,
    offset: number,
    limit: number = 5,
    searchQuery?: string
  ): Observable<ActiveQuestionnaire[]> {
    const teacherId = parseInt(this.authService.getUserId()!, 10);
  
    // Determine if this is a search scenario or a section-based scenario
    let filteredQuestionnaires = this.mockData.mockActiveQuestionnaire.filter(q => {
      if (section === 'searchResults' && searchQuery) {
        return (
          q.teacher.id === teacherId &&
          q.student.username.toLowerCase().includes(searchQuery.toLowerCase())
        );
      }
  
      // Handle section-based filtering
      switch (section) {
        case 'finishedByStudents':
          return q.isStudentFinished && !q.isTeacherFinished;
        case 'notAnsweredByStudents':
          return !q.isStudentFinished;
        case 'notAnsweredByTeacher':
          return !q.isTeacherFinished;
        default:
          return false;
      }
    });

  
    // Simulate pagination by slicing the array
    const paginatedData = filteredQuestionnaires.slice(offset, offset + limit);
    return of(paginatedData).pipe(
      delay(250), // Simulate delay
      catchError(this.handleError('getPaginatedDashboardData'))
    );
  }
  

  
  /**
   * Loads initial data either from local storage or from a mock data file.
   * If data is present in local storage, it initializes the service with that data.
   * Otherwise, it fetches data from a JSON file and saves it to local storage.
   */
  private loadInitialMockData(): void {
    const savedData = this.localStorageService.getData(this.localStorageKey);
    if (savedData) {
      this.mockData = JSON.parse(savedData);
    } else {
      this.http.get('/assets/mock-data.json').subscribe(
        (data: any) => {
          this.mockData = {
            mockStudents: data.mockStudents,
            mockTeachers: data.mockTeachers,
            mockQuestions: data.mockQuestions,
            mockStudentTeacherAnswers: data.mockStudentTeacherAnswers,
            mockActiveQuestionnaire: data.mockActiveQuestionnaire
          };
          this.saveData();
        },
        error => this.errorHandlingService.handleError(error, 'Failed to load mock data')
      );
    }
  }


  getFirstActiveQuestionnaireId(): string | null {
    const token = this.jwtTokenService.tokenExists();
    if (token) {
      try {
        const decodedToken = this.jwtTokenService.getDecodeToken();
        const userId = decodedToken ? decodedToken['sub'] : null;
  
        const activeQuestionnaire = this.mockData.mockActiveQuestionnaire.find(aq => 
          (aq.student.id === userId && !aq.isStudentFinished) || 
          (aq.teacher.id === userId && !aq.isTeacherFinished)
        );
  
        return activeQuestionnaire ? activeQuestionnaire.id : null;
      } catch (error) {
        this.errorHandlingService.handleError(error, 'Invalid token');
        return null;
      }
    }
    return null;
  }

  private handleError(operation = 'operation') {
    return (error: any): Observable<never> => {
      return this.errorHandlingService.handleError(error, `${operation} failed`);
    };
  }
  /**
   * Saves the current state of mock data to local storage.
   */
  private saveData(): void {
    this.localStorageService.saveData(this.localStorageKey, JSON.stringify(this.mockData));
  }

  getDashboardData(): Observable<{
    students: User[],
    activeQuestionnaires: ActiveQuestionnaire[]
  }> {
    return of({
      students: this.mockData.mockStudents,
      activeQuestionnaires: this.mockData.mockActiveQuestionnaire
    }).pipe(
      delay(250),
      catchError(this.handleError('getDashboardData'))
    );
  }



  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    const activeQuestionnaire = this.mockData.mockActiveQuestionnaire.find(aq => aq.id === id) || null;
    return of(activeQuestionnaire).pipe(
      delay(250),
      catchError(this.handleError('getActiveQuestionnaireById'))
    );
  }

  /**
   * Retrieves the list of mock students.
   * @returns An observable that emits the list of mock students.
   */
  getStudents(): Observable<User[]> {
    return of(this.mockData.mockStudents).pipe(
      delay(250),
      catchError(this.handleError('getStudents'))
    );
  }

  /**
   * Adds a student to the questionnaire if they exist and are not already in an active questionnaire.
   * @param studentId The ID of the student to add.
   */
  addStudentToQuestionnaire(studentId: number, teacherId: number = 1): Observable<void> {
    const student = this.mockData.mockStudents.find(s => s.id === studentId);
    const teacher = this.mockData.mockTeachers.find(t => t.id === teacherId);

    if (!student || !teacher) {
      return this.errorHandlingService.handleError(new Error('Student or Teacher not found'), 'addStudentToQuestionnaire');
    }

    const studentAvailableForQuestionnaire = !this.mockData.mockActiveQuestionnaire.some(aq => aq.student.id === studentId && !aq.isStudentFinished);

    if (studentAvailableForQuestionnaire) {
      return this.createActiveQuestionnaire(studentId, teacherId).pipe(
        map(() => {}),
        catchError(this.handleError('addStudentToQuestionnaire'))
      );
    } else {
      return this.errorHandlingService.handleError(new Error('Student is already in an active questionnaire or has finished it'), 'addStudentToQuestionnaire');
    }
  }
  

  /**
   * Retrieves the list of questions for a specific user if they exist and are part of an active questionnaire.
   * @param userId The ID of the user.
   * @returns An observable that emits the list of questions.
   */
  getQuestionsForUser(): Observable<Question[]> {
    return of(this.mockData.mockQuestions).pipe(delay(250));
  }
  /**
   * Checks if a student is currently part of an active questionnaire.
   * @param studentId The ID of the student.
   * @returns True if the student is in an active questionnaire, false otherwise.
   */
  isStudentInQuestionnaire(studentId: number): boolean {
    return this.mockData.mockActiveQuestionnaire.some(aq => aq.student.id === studentId && !aq.isStudentFinished);
  }




  /**
   * Marks a user's questionnaire as finished based on their role and saves the answers.
   * @param userId The ID of the user.
   * @param role The role of the user (e.g., 'student', 'teacher').
   * @param questionnaireId The ID of the questionnaire.
   * @param answers The answers to submit.
   */
  submitData(userId: any, role: string, questionnaireId: string, answers: Question[]): Observable<void> {
    const activeQuestionnaire = this.mockData.mockActiveQuestionnaire.find(aq => aq.id === questionnaireId);
    
    if (!activeQuestionnaire) {
      return this.errorHandlingService.handleError(new Error('Active questionnaire not found or user role mismatch'), 'submitData');
    }

    answers.forEach(answer => {
      // Instead of saving the results of the answer here, lets just log it.
      console.log(`Question ID: ${answer.id}, Selected Option: ${answer.selectedOption}`);
    });


    // Mark the questionnaire as finished for the user
    if (role === 'student' && activeQuestionnaire.student.id == userId) {
      activeQuestionnaire.isStudentFinished = true;
    } else if (role === 'teacher' && activeQuestionnaire.teacher.id == userId) {
      activeQuestionnaire.isTeacherFinished = true;
    }

    this.saveData();
    return of(undefined).pipe(
      delay(250),
      catchError(this.handleError('submitData'))
    );
  }

   /**
   * Creates a new active questionnaire.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   * @returns The created active questionnaire.
   */
   createActiveQuestionnaire(studentId: number, teacherId: number): Observable<ActiveQuestionnaire> {
    const student = this.mockData.mockStudents.find(s => s.id === studentId);
    const teacher = this.mockData.mockTeachers.find(t => t.id === teacherId);
  
    if (!student || !teacher) {
      return this.errorHandlingService.handleError(new Error('Student or Teacher not found'), 'createActiveQuestionnaire');
    }
  
    const newActiveQuestionnaire: ActiveQuestionnaire = {
      id: this.generateId(),
      student: student,
      teacher: teacher,
      isStudentFinished: false,
      isTeacherFinished: false
    };
  
    this.mockData.mockActiveQuestionnaire.push(newActiveQuestionnaire);
    this.saveData();
    return of(newActiveQuestionnaire).pipe(
      delay(250),
      catchError(this.handleError('createActiveQuestionnaire'))
    );
  }

  /**
   * Deletes an active questionnaire by its ID.
   * @param id The ID of the active questionnaire to delete.
   */
  deleteActiveQuestionnaire(id: string): Observable<void> {
    this.mockData.mockActiveQuestionnaire = this.mockData.mockActiveQuestionnaire.filter(aq => aq.id !== id);
    this.saveData();
    return of(undefined).pipe(delay(250));
  }

  /**
   * Generates a random ID for purpose of testing creating new questionare on the frontend.
   * @returns The generated ID.
   */
  generateId(): string {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_';
    const idLength = 4;
    let result = '';
    for (let i = 0; i < idLength; i++) {
      result += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    console.log('Generated ID:', result);
    return result;
  }

    /**
   * Validates if a user has access to a specific questionnaire.
   * @param userId The ID of the user.
   * @param role The role of the user (e.g., student, teacher).
   * @param questionnaireId The ID of the questionnaire.
   * @returns An observable that emits true if the user has access, false otherwise.
   */
    validateUserAccess(userId: any, role: string, questionnaireId: string): Observable<boolean> {
      const activeQuestionnaire = this.mockData.mockActiveQuestionnaire.find(aq => aq.id === questionnaireId);
  
      if (!activeQuestionnaire) {
        return of(false); // Questionnaire not found
      }
  
      if (role === 'student' && activeQuestionnaire.student.id == userId && !activeQuestionnaire.isStudentFinished) {
        return of(true);
      }
  
      if (role === 'teacher' && activeQuestionnaire.teacher.id == userId && !activeQuestionnaire.isTeacherFinished) {
        return of(true);
      }
  
      return of(false); // User does not have access
    }
}
