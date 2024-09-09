import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, delay, map, of, throwError } from 'rxjs';
import { User, Question, StudentTeacherAnswer, ActiveQuestionnaire, QuestionTemplate } from '../../models/questionare';

import { ErrorHandlingService } from '../error-handling.service';
import { JWTTokenService } from '../auth/jwt-token.service';
import { MockDbService } from '../mock/mock-db.service';


@Injectable({
  providedIn: 'root'
})
  //MockDataService is a fake dataservice that takes 
export class MockDataService {
  private mockDbService = inject(MockDbService);
  private jwtTokenService = inject(JWTTokenService);
  private errorHandlingService = inject(ErrorHandlingService)

  constructor(private http: HttpClient) {
    this.mockDbService.loadInitialMockData();
  }

  
  createActiveQuestionnaire(student: User, teacher: User, templateId: string): Observable<ActiveQuestionnaire>{
    const userRole = this.getRoleFromToken();
  
    // Only allow creation if the user is a teacher or admin
    if (userRole !== 'admin') {
      return throwError(() => new Error('Unauthorized: Only teachers or admins can create questionnaires.'));
    }

    const template = this.mockDbService.mockData.mockQuestionTemplates.find(t => t.templateId === templateId);

    if (!template) {
      throw new Error(`Template with ID ${templateId} not found.`);
    }

    const newActiveQuestionnaire: ActiveQuestionnaire = {
      id: this.generateId(), // Generate a unique ID based on the current timestamp
      student,
      teacher,
      questionnaireTemplate: {
        templateId: template.templateId,
        title: template.title,
        description: template.description
      },
      isStudentFinished: false,
      isTeacherFinished: false,
      createdAt: new Date()
    };

    // Add the new ActiveQuestionnaire to the mock database
    this.mockDbService.mockData.mockActiveQuestionnaire.push(newActiveQuestionnaire);
    this.mockDbService.saveData()

    // Return the new ActiveQuestionnaire as an observable
    return of(newActiveQuestionnaire).pipe(delay(300)); // Simulate a delay
  }

  // Active questionare
  getUsersFromSearch(role: string, nameString: string, page: number = 1, limit: number = 10) {
    // Filter users by role (student or teacher)
    let users = this.mockDbService.mockData.mockUsers.filter(u => u.role === role);
  
    // Filter users by name string (case-insensitive)
    if (nameString) {
      users = users.filter(u => u.fullName.toLowerCase().includes(nameString.toLowerCase()));
    }
  
    // Implement pagination: calculate start and end indexes
    const startIndex = (page - 1) * limit;
    const endIndex = startIndex + limit;
  
    // Slice the array to return the correct page of results
    const paginatedUsers = users.slice(startIndex, endIndex);
  
    // Return the result as an Observable (simulate async operation)
    return of(paginatedUsers).pipe(delay(300)); // Optional delay to simulate network latency
  }

  getTemplatesFromSearch(titleString: string, page: number = 1, limit: number = 10){
    const templates = this.mockDbService.mockData.mockQuestionTemplates.filter(t => t.title.toLowerCase().includes(titleString.toLowerCase()))

    const startIndex = (page - 1) * limit;
    const endIndex = startIndex + limit;

    const pageinatedTemplates = templates.slice(startIndex, endIndex);
    return of(pageinatedTemplates).pipe(delay(300)); // Optional delay to simulate network latency
  }

  // Get all templates
  getTemplates(): Observable<QuestionTemplate[]> {
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

    // Filter by student's username (case insensitive)
    if (filter.searchTeacher) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        q.teacher.fullName.toLowerCase().includes(filter.searchTeacher.toLowerCase())
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
  submitUserAnswers(userId: number | null, role: string, answers: Question[], questionnaireId: string | null): Observable<void> {
    const userRole = this.getRoleFromToken();

    // Ensure the role matches before allowing submission
    if (role !== userRole) {
      return throwError(() => new Error('Unauthorized: Role mismatch.'));
    }

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
   * Deletes an active questionnaire by its ID.
   * @param id The ID of the active questionnaire to delete.
   */
  deleteActiveQuestionnaire(id: string): Observable<void> {
    this.mockDbService.mockData.mockActiveQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.filter(aq => aq.id !== id);
    this.saveData();
    return of(undefined).pipe(delay(250));
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
    
      // Ensure the role matches before granting access
      const userRole = this.getRoleFromToken();
      if (role !== userRole) {
        return of(false); // Role mismatch, deny access
      }
    
      if (role === 'student' && activeQuestionnaire.student.id == userId && !activeQuestionnaire.isStudentFinished) {
        return of(true);
      }
    
      if (role === 'teacher' && activeQuestionnaire.teacher.id == userId && !activeQuestionnaire.isTeacherFinished) {
        return of(true);
      }
    
      return of(false);
    }

    private getRoleFromToken(): string | null {
      const token = this.jwtTokenService.getDecodeToken();
      if (token) {
        const decodedToken: any = this.jwtTokenService.getDecodeToken();
        return decodedToken ? decodedToken['scope'] : null;
      }
      return null;
    }
   /**
   * Generates a random ID for purpose of testing creating new questionare on the frontend.
   * @returns The generated ID.
   */
    private generateId(): string {
      const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_';
      const idLength = 4;
      let result = '';
      for (let i = 0; i < idLength; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
      }
      console.log('Generated ID:', result);
      return result;
    }
}
