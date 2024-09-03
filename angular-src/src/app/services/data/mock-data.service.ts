import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, delay, map, of, throwError } from 'rxjs';
import { User, Question, StudentTeacherAnswer, ActiveQuestionnaire, QuestionTemplate } from '../../models/questionare';

import { ErrorHandlingService } from '../error-handling.service';
import { JWTTokenService } from '../auth/jwt-token.service';
import { AppAuthService } from '../auth/app-auth.service';
import { MockDbService } from '../mock/mock-db.service';


@Injectable({
  providedIn: 'root'
})
export class MockDataService {
  private mockDbService = inject(MockDbService);
  private jwtTokenService = inject(JWTTokenService);
  private errorHandlingService = inject(ErrorHandlingService)

  constructor(private http: HttpClient) {
    this.mockDbService.loadInitialMockData();
  }


  // Get all templates
  getTemplates(): Observable<QuestionTemplate[]> {
    console.log(this.mockDbService.mockData.mockQuestionTemplates)
    return of(this.mockDbService.mockData.mockQuestionTemplates);
  }

  // Create a new template
  createTemplate(template: QuestionTemplate): Observable<void> {
    // Add a new template to the mock database
    this.mockDbService.mockData.mockQuestionTemplates.push(template);
    this.mockDbService.saveData(); // Save the updated state to local storage
    return of();
  }

  // Update an existing template
  updateTemplate(updatedTemplate: QuestionTemplate): Observable<void> {
    // Find and update the existing template in the mock database
    const templateIndex = this.mockDbService.mockData.mockQuestionTemplates.findIndex(t => t.templateId === updatedTemplate.templateId);
    if (templateIndex !== -1) {
      this.mockDbService.mockData.mockQuestionTemplates[templateIndex] = updatedTemplate;
      this.mockDbService.saveData(); // Save the updated state to local storage
    }
    return of();
  }

  // Delete a template by ID
  deleteTemplate(templateId: string): Observable<void> {
    // Remove the template from the mock database
    this.mockDbService.mockData.mockQuestionTemplates = this.mockDbService.mockData.mockQuestionTemplates.filter(t => t.templateId !== templateId);
    this.mockDbService.saveData(); // Save the updated state to local storage
    return of();
  }






  getActiveQuestionnairePage(
    filter: any = {}, // General filter object
    page: number = 1,
    limit: number = 5,
  ) {
    let filteredQuestionnaires: ActiveQuestionnaire[] = this.mockDbService.mockData.mockActiveQuestionnaire;
  
    // Filter by teacherId if provided
    if (filter.teacherId) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        q.teacher.id === filter.teacherId
      );
    }
  
    // Filter by studentId if provided
    if (filter.studentId) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        q.student.id === filter.studentId
      );
    }
  
    // Filter by student's username (case insensitive)
    if (filter.searchStudent) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        q.student.fullName.toLowerCase().includes(filter.searchStudent.toLowerCase())
      );
    }
  
    // Filter by whether the student has finished (handling boolean values)
    if (filter.studentIsFinished !== undefined) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        q.isStudentFinished === filter.studentIsFinished
      );
    }
  
    // Filter by whether the teacher has finished (handling boolean values)
    if (filter.teacherIsFinished !== undefined) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        q.isTeacherFinished === filter.teacherIsFinished
      );
    }
  
    // Calculate the offset for pagination
    const offset = (page - 1) * limit;
  
    // Slice the filtered results to get the current page
    const pageData = filteredQuestionnaires.slice(offset, offset + limit);
  
    // Return the paginated and filtered data as an Observable with simulated delay
    return of(pageData).pipe(
      delay(250), // Simulate delay for mock data
      catchError(this.handleError('getActiveQuestionnairePage'))
    );
  }


  getFirstActiveQuestionnaireId(): string | null {
    const token = this.jwtTokenService.tokenExists();
    if (token) {
      try {
        const decodedToken = this.jwtTokenService.getDecodeToken();
        const userId = decodedToken ? decodedToken['sub'] : null;
  
        const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(aq => 
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
    this.mockDbService.saveData();
  }

  getDashboardData(role: string): Observable<{
    students: User[],
    activeQuestionnaires: ActiveQuestionnaire[]
  }> {
    // Filter users based on role
    const students = this.mockDbService.mockData.mockUsers.filter(user => user.role === "student");
    return of({
      students,
      activeQuestionnaires: this.mockDbService.mockData.mockActiveQuestionnaire
    }).pipe(
      delay(250),
      catchError(this.handleError('getDashboardData'))
    );
  }



  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(aq => aq.id === id) || null;
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
    const students = this.mockDbService.mockData.mockUsers.filter(user => user.role === "students");
    return of(students).pipe(
      delay(250),
      catchError(this.handleError('getStudents'))
    );
  }

  /**
   * Adds a student to the questionnaire if they exist and are not already in an active questionnaire.
   * @param studentId The ID of the student to add.
   */
  addStudentToQuestionnaire(studentId: number, teacherId: number = 1): Observable<void> {
    const student = this.mockDbService.mockData.mockUsers.find(u => u.id === studentId && u.role === 'student');
    const teacher = this.mockDbService.mockData.mockUsers.find(u => u.id === teacherId && u.role === 'teacher');

    if (!student || !teacher) {
      return this.errorHandlingService.handleError(new Error('Student or Teacher not found'), 'addStudentToQuestionnaire');
    }

    const studentAvailableForQuestionnaire = !this.mockDbService.mockData.mockActiveQuestionnaire.some(aq => aq.student.id === studentId && !aq.isStudentFinished);

    if (studentAvailableForQuestionnaire) {
      return this.createActiveQuestionnaire(studentId, teacherId).pipe(
        map(() => {}),
        catchError(this.handleError('addStudentToQuestionnaire'))
      );
    } else {
      return this.errorHandlingService.handleError(new Error('Student is already in an active questionnaire or has finished it'), 'addStudentToQuestionnaire');
    }
  }
  

  getQuestionsForUser(templateId: string): Observable<Question[]> {
    // Find the template by templateId
    const template = this.mockDbService.mockData.mockQuestionTemplates.find(t => t.templateId === templateId);
  
    // If template is found, return its questions; otherwise, return an empty array
    const questions = template ? template.questions : [];
    
    return of(questions).pipe(delay(250)); // Simulate delay for mock data
  }
  /**
   * Checks if a student is currently part of an active questionnaire.
   * @param studentId The ID of the student.
   * @returns True if the student is in an active questionnaire, false otherwise.
   */
  isStudentInQuestionnaire(studentId: number): boolean {
    return this.mockDbService.mockData.mockActiveQuestionnaire.some(aq => aq.student.id === studentId && !aq.isStudentFinished);
  }




  /**
   * Marks a user's questionnaire as finished based on their role and saves the answers.
   * @param userId The ID of the user.
   * @param role The role of the user (e.g., 'student', 'teacher').
   * @param questionnaireId The ID of the questionnaire.
   * @param answers The answers to submit.
   */
  submitData(userId: any, role: string, questionnaireId: string, answers: Question[]): Observable<void> {
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(aq => aq.id === questionnaireId);
    
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
   * Creates a new active questionnaire, whoever it uses a default template.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   * @returns The created active questionnaire.
   */
   createActiveQuestionnaire(studentId: number, teacherId: number): Observable<ActiveQuestionnaire> {
    const student = this.mockDbService.mockData.mockUsers.find(u => u.id === studentId && u.role === 'student');
    const teacher = this.mockDbService.mockData.mockUsers.find(u => u.id === teacherId && u.role === 'teacher');
  
    if (!student || !teacher) {
      return this.errorHandlingService.handleError(new Error('Student or Teacher not found'), 'createActiveQuestionnaire');
    }
  
    // Use 'template1' as the default questionnaire template
    const defaultTemplate = this.mockDbService.mockData.mockQuestionTemplates.find(t => t.templateId === 'template1');
    
    if (!defaultTemplate) {
      return this.errorHandlingService.handleError(new Error('Default template not found'), 'createActiveQuestionnaire');
    }
  
    const newActiveQuestionnaire: ActiveQuestionnaire = {
      id: this.generateId(),
      student: student,
      teacher: teacher,
      isStudentFinished: false,
      isTeacherFinished: false,
      questionnaireTemplate: {
        templateId: defaultTemplate.templateId,
        title: defaultTemplate.title,
        description: defaultTemplate.description
      }
    };
  
    this.mockDbService.mockData.mockActiveQuestionnaire.push(newActiveQuestionnaire);
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
    this.mockDbService.mockData.mockActiveQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.filter(aq => aq.id !== id);
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
      const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(aq => aq.id === questionnaireId);
  
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
