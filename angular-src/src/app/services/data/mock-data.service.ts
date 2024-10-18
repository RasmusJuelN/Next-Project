import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, delay, map, of, throwError } from 'rxjs';
import { User, Question, ActiveQuestionnaire, QuestionTemplate, AnswerSession, Answer, AnswerDetails } from '../../models/questionare';
import { Option } from '../../models/questionare';
import { ErrorHandlingService } from '../error-handling.service';
import { JWTTokenService } from '../auth/jwt-token.service';
import { MockDbService } from '../mock/mock-db.service';
import { LogFileType } from '../../models/log-models';
import { AuthService } from '../auth/auth.service';
import { LogEntry } from '../../models/log-models';



@Injectable({
  providedIn: 'root'
})
  //MockDataService is a fake dataservice that takes 
export class MockDataService {
  private mockDbService = inject(MockDbService);
  private jwtTokenService = inject(JWTTokenService);
  private errorHandlingService = inject(ErrorHandlingService)
  private authService = inject(AuthService)
  constructor() {
    this.mockDbService.loadInitialMockData();
  }

  checkForActiveQuestionnaires(userId: string): Observable<string | null> {
    // Check if userId is provided
    if (!userId) {
      return of(null);
    }
  
    const mockActiveQuestionnaires = this.mockDbService.mockData.mockActiveQuestionnaire;
  
    // Find an active questionnaire for the user, regardless of role (student or teacher)
    const activeQuestionnaire = mockActiveQuestionnaires.find((questionnaire: ActiveQuestionnaire) => {
      return (
        (questionnaire.student.id === userId && questionnaire.studentFinishedAt === null) ||
        (questionnaire.teacher.id === userId && questionnaire.teacherFinishedAt === null)
      );
    });
    
  
    // Return the ID of the active questionnaire if found, or null otherwise
    if (activeQuestionnaire) {
      return of(activeQuestionnaire.id).pipe(delay(250)); // Simulate delay for mock data
    }
  
    // Return null if no active questionnaire is found
    return of(null).pipe(delay(250));
  }
  
  
  
  

 // Updated getLogFileTypes() method
 getLogFileTypes(): Observable<LogFileType> {
  const logFileTypes: LogFileType = {};
  const mockLogs = this.mockDbService.mockData.mockLogs;

  for (const logName in mockLogs) {
    if (mockLogs.hasOwnProperty(logName)) {
      logFileTypes[logName] = { amount: mockLogs[logName].length };
    }
  }

  return of(logFileTypes).pipe(delay(250)); // Simulate delay
}

// Updated getLogs() method
getLogs(
  logSeverity: string,
  logFileType: string,
  lineCount: number | null,
  reverse: boolean
): Observable<LogEntry[]> {
  const logs: LogEntry[] = this.mockDbService.mockData.mockLogs[logFileType];

  if (!logs) {
    console.error(`Log file type '${logFileType}' not found`);
    return of([]);
  }

  // Filter logs by severity if provided
  let filteredLogs = logs;

  if (logSeverity) {
    const severityLevels = ['DEBUG', 'INFO', 'WARNING', 'ERROR', 'CRITICAL'];
    const severityIndex = severityLevels.indexOf(logSeverity.toUpperCase());

    if (severityIndex === -1) {
      console.error(`Invalid severity level '${logSeverity}'`);
      return of([]);
    }

    filteredLogs = logs.filter((log) => {
      const logLevelIndex = severityLevels.indexOf(log.severity);
      return logLevelIndex >= severityIndex;
    });
  }

  // Reverse the logs if needed
  if (reverse) {
    filteredLogs = filteredLogs.slice().reverse();
  }

  // Handle lineCount being null or zero (load all lines)
  let selectedLogs = filteredLogs;
  if (lineCount && lineCount > 0) {
    selectedLogs = filteredLogs.slice(0, lineCount);
  }
  return of(selectedLogs).pipe(
    delay(250), // Simulate delay
    catchError(this.handleError('getLogs'))
  );
}

  getSettings(): Observable<any>  {
    const settings = this.mockDbService.mockData.mockAppSettings.settings
    const metaData = this.mockDbService.mockData.mockAppSettings.metadata
    return of({
      settings: settings,
      metadata: metaData
    });
  }

  updateSettings(updatedSettings: any): Observable<any>{
    this.mockDbService.mockData.mockAppSettings.settings = updatedSettings;
    this.saveData();
    return of({ success: true });
  }

  submitUserAnswers(userId: string, answers: Answer[], questionnaireId: string): Observable<void> {
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(aq => aq.id === questionnaireId);
    if (!activeQuestionnaire) {
      return throwError(() => new Error('Active questionnaire not found.'));
    }
  
    let answerSession = this.mockDbService.mockData.mockAnswerSessions.find(as => as.questionnaireId === questionnaireId);
  
    if (!answerSession) {
      const studentUser = this.getUserById(activeQuestionnaire.student.id);
      const teacherUser = this.getUserById(activeQuestionnaire.teacher.id);
      if (!studentUser || !teacherUser) {
        return throwError(() => new Error('Student or teacher user not found.'));
      }
  
      // Create new AnswerSession
      answerSession = {
        questionnaireId: questionnaireId!,
        users: { student: studentUser, teacher: teacherUser },
        answers: [],
        studentAnsweredAt: undefined,
        teacherAnsweredAt: undefined,
      };
      this.mockDbService.mockData.mockAnswerSessions.push(answerSession);
    }
  
    answers.forEach(newAnswer => {
      const templateId = activeQuestionnaire.template.id;
      const existingAnswer = answerSession!.answers.find(a => a.questionId === newAnswer.questionId);
      const answerContent = newAnswer.customAnswer || this.getOptionLabelById(newAnswer.questionId, newAnswer.selectedOptionId!, templateId);
  
      if (existingAnswer) {
        if (userId === activeQuestionnaire.student.id) {
          existingAnswer.studentAnswer = answerContent;
          answerSession!.studentAnsweredAt = new Date();
        } else if (userId === activeQuestionnaire.teacher.id) {
          existingAnswer.teacherAnswer = answerContent;
          answerSession!.teacherAnsweredAt = new Date();
        } else {
          throw new Error('Unknown user ID.');
        }
      } else {
        answerSession!.answers.push({
          questionId: newAnswer.questionId,
          questionTitle: this.getQuestionTitleById(newAnswer.questionId, templateId),
          studentAnswer: userId === activeQuestionnaire.student.id ? answerContent : '',
          teacherAnswer: userId === activeQuestionnaire.teacher.id ? answerContent : '',
        });
      }
    });
    // Set the completion status for the user
    if (userId === activeQuestionnaire.student.id) {
      activeQuestionnaire.studentFinishedAt = new Date(); // Mark student as finished
      answerSession!.studentAnsweredAt = new Date();
    } else if (userId === activeQuestionnaire.teacher.id) {
      activeQuestionnaire.teacherFinishedAt = new Date(); // Mark teacher as finished
      answerSession!.teacherAnsweredAt = new Date();
    }
  
    this.saveData();
    return of(undefined).pipe(delay(250));
  }
  
  
  private getQuestionTitleById(questionId: number, templateId: string): string {
    // Find the template by templateId first to ensure the search is local to that template
    const template = this.mockDbService.mockData.mockQuestionTemplates.find(t => t.id === templateId);
  
    if (!template) {
      throw new Error(`Template with ID ${templateId} not found`);
    }
  
    // Now, search for the question in that specific template
    const question = template.questions.find(q => q.id === questionId);
  
    if (!question) {
      throw new Error(`Question with ID ${questionId} not found in template ${templateId}`);
    }
  
    return question.title; // Return the title of the question
  }
  

  private getOptionLabelById(questionId: number, selectedOptionId: number, templateId: string): string {
    // Find the relevant template to ensure the search is scoped correctly
    const template = this.mockDbService.mockData.mockQuestionTemplates.find(t => t.id === templateId);
  
    if (!template) {
      throw new Error(`Template with ID ${templateId} not found`);
    }
  
    // Find the question in that specific template
    const question = template.questions.find(q => q.id === questionId);
  
    if (!question) {
      throw new Error(`Question with ID ${questionId} not found in template ${templateId}`);
    }
  
    // Find the option by selectedOptionId within the question's options array
    const option = question.options.find(opt => opt.id === selectedOptionId);
  
    if (!option) {
      throw new Error(`Option with ID ${selectedOptionId} not found for question ID ${questionId}`);
    }
  
    return option.label || String(option.value);  // Return the label, or the value as a fallback
  }
  


  // Handle pagination for users or templates
  private paginate<T>(items: T[], page: number, limit: number): T[] {
    const startIndex = (page - 1) * limit;
    return items.slice(startIndex, startIndex + limit);
  }

  private getUserById(userId: string | null) {
    if (!userId) {
      return null;
    }
    const user = this.mockDbService.mockData.mockUsers.find(u => u.id === userId);
    return user ? user : null;
  }

  getResults(activeQuestionnaireId: string): Observable<AnswerSession> {
    // Step 1: Find the active questionnaire
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(
      (aq: ActiveQuestionnaire) => aq.id === activeQuestionnaireId
    );
    if (!activeQuestionnaire) {
      return throwError(() => new Error('Active questionnaire not found'));
    }
  
    // Step 2: Find the answer session for this questionnaire
    const answerSession = this.mockDbService.mockData.mockAnswerSessions?.find(
      (session: AnswerSession) => session.questionnaireId === activeQuestionnaireId
    );
    if (!answerSession) {
      return throwError(() => new Error('Answer session not found'));
    }
  
    // Step 3: Return the answer session from the mock data as is
    return of(answerSession).pipe(
      delay(500), // Simulate async behavior with a delay
      catchError((error) => throwError(() => new Error(error.message)))
    );
  }
  
  
  
  

  
  createActiveQuestionnaire(student: User, teacher: User, id: string): Observable<ActiveQuestionnaire>{
    const userRole = this.getRoleFromToken();
  
    // Only allow creation if the user is a teacher or admin
    if (userRole !== 'admin') {
      return throwError(() => new Error('Unauthorized: Only teachers or admins can create questionnaires.'));
    }

    const template = this.mockDbService.mockData.mockQuestionTemplates.find(t => t.id === id);

    if (!template) {
      throw new Error(`Template with ID ${id} not found.`);
    }

    const newActiveQuestionnaire: ActiveQuestionnaire = {
      id: this.generateId(), // Generate a unique ID based on the current timestamp
      student,
      teacher,
      template: {
        id: template.id,
        title: template.title,
        description: template.description
      },
      studentFinishedAt: null,
      teacherFinishedAt: null,
      createdAt: new Date()
    };

    // Add the new ActiveQuestionnaire to the mock database
    this.mockDbService.mockData.mockActiveQuestionnaire.push(newActiveQuestionnaire);
    this.mockDbService.saveData()

    // Return the new ActiveQuestionnaire as an observable
    return of(newActiveQuestionnaire).pipe(delay(300)); // Simulate a delay
  }

  getUsersFromSearch(role: string, nameString: string, page: number = 1, limit: number = 10, cacheCookie?: string): Observable<{ users: User[] }> {
    // Filter users by role (student or teacher)
    let users = this.mockDbService.mockData.mockUsers.filter(u => u.role === role);
    
    // Filter users by the name string (if provided)
    if (nameString) {
      users = users.filter(u => u.fullName.toLowerCase().includes(nameString.toLowerCase()));
    }

    // Paginate the results
    const paginatedUsers = this.paginate(users, page, limit);

    // Return the users and cacheCookie in the UserSearchResponse structure
    return of({ users: paginatedUsers }).pipe(delay(300));  // Simulate a network delay
  }

// Combined mock version for Search and Pagination for Templates
getTemplates(page: number = 1, limit: number = 10, titleString?: string): Observable<QuestionTemplate[]> {
  let templates = this.mockDbService.mockData.mockQuestionTemplates;

  // If a titleString is provided, filter the templates based on it
  if (titleString) {
    templates = templates.filter(t => t.title.toLowerCase().includes(titleString.toLowerCase()));
  }

  // Paginate the templates
  const paginatedTemplates = this.paginate(templates, page, limit);

  // Return the paginated result with a simulated delay
  return of(paginatedTemplates).pipe(delay(300)); 
}

  // Create a new template
  createTemplate(template: QuestionTemplate): Observable<void> {
    
    // Add the new template to mock data
    this.mockDbService.mockData.mockQuestionTemplates.push(template);
    this.saveData();
    
    return of(undefined).pipe(delay(300)); // Simulate delay
  }

  // Update an existing template
  updateTemplate(updatedTemplate: QuestionTemplate): Observable<void> {
    const existingTemplateIndex = this.mockDbService.mockData.mockQuestionTemplates.findIndex(t => t.id === updatedTemplate.id);
    
    if (existingTemplateIndex !== -1) {
      const existingTemplate = this.mockDbService.mockData.mockQuestionTemplates[existingTemplateIndex];

  
      // Now, update the existing template with the new title, description, and questions
      existingTemplate.title = updatedTemplate.title; // Update title
      existingTemplate.description = updatedTemplate.description; // Update description
      existingTemplate.questions = updatedTemplate.questions; // Update questions with any new ones
  
      // Save the updated template in mock data
      this.mockDbService.mockData.mockQuestionTemplates[existingTemplateIndex] = existingTemplate;
      this.saveData(); // Persist changes
    }
  
    return of(undefined).pipe(delay(300)); // Simulate delay
  }
  
  

  // Delete a template by ID
  deleteTemplate(templateId: string): Observable<void> {
    this.mockDbService.mockData.mockQuestionTemplates = this.mockDbService.mockData.mockQuestionTemplates.filter(t => t.id !== templateId);
    this.saveData();
    
    return of(undefined).pipe(delay(300)); // Simulate delay
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
  
    if (filter.studentIsFinished !== undefined) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        filter.studentIsFinished
          ? q.studentFinishedAt !== null // Student has finished
          : q.studentFinishedAt === null  // Student has not finished
      );
    }
    
    if (filter.teacherIsFinished !== undefined) {
      filteredQuestionnaires = filteredQuestionnaires.filter(q =>
        filter.teacherIsFinished
          ? q.teacherFinishedAt !== null // Teacher has finished
          : q.teacherFinishedAt === null  // Teacher has not finished
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
          (aq.student.id === userId && aq.studentFinishedAt === null) || 
          (aq.teacher.id === userId && aq.teacherFinishedAt === null)
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
    // Check if the provided ID is valid
    if (!id) {
      return of(null);
    }
  
    // Search for the active questionnaire by its ID in the mock database
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(
      (questionnaire: ActiveQuestionnaire) => questionnaire.id === id
    );
  
    // Return the active questionnaire if found, or null otherwise
    if (activeQuestionnaire) {
      return of(activeQuestionnaire).pipe(delay(250)); // Simulate delay for mock data
    } else {
      // Return null if no active questionnaire is found
      return of(null).pipe(delay(250));
    }
  }


  getActiveQuestionnaireByUserId(id: string): Observable<ActiveQuestionnaire | null> {
    // Check if the active questionnaire contains either a student or teacher with the specified id
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(
      aq => aq.student.id === id || aq.teacher.id === id
    ) || null;
  
    return of(activeQuestionnaire).pipe(
      delay(250), // Simulate network latency
      catchError(this.handleError('getActiveQuestionnaireByUserId'))
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
    const template = this.mockDbService.mockData.mockQuestionTemplates.find(t => t.id === templateId);
  
    // If template is found, return its questions; otherwise, return an empty array
    const questions = template ? template.questions : [];
    
    return of(questions).pipe(delay(250)); // Simulate delay for mock data
  }
  /**
   * Checks if a student is currently part of an active questionnaire.
   * @param studentId The ID of the student.
   * @returns True if the student is in an active questionnaire, false otherwise.
   */
  isStudentInQuestionnaire(studentId: string): boolean {
    return this.mockDbService.mockData.mockActiveQuestionnaire.some(aq => 
      aq.student.id === studentId && aq.studentFinishedAt === null
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
    
      if (role === 'student' && activeQuestionnaire.student.id == userId && activeQuestionnaire.studentFinishedAt === null) {
        return of(true);
      }
      
      if (role === 'teacher' && activeQuestionnaire.teacher.id == userId && activeQuestionnaire.teacherFinishedAt === null) {
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
