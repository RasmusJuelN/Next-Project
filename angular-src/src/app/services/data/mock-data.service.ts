import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, delay, map, of, throwError } from 'rxjs';
import { User, Question, ActiveQuestionnaire, QuestionTemplate, AnswerSession, Answer } from '../../models/questionare';
import { Option } from '../../models/questionare';
import { ErrorHandlingService } from '../error-handling.service';
import { JWTTokenService } from '../auth/jwt-token.service';
import { MockDbService } from '../mock/mock-db.service';
import { AuthService } from '../auth/auth.service';


@Injectable({
  providedIn: 'root'
})
  //MockDataService is a fake dataservice that takes 
export class MockDataService {
  private mockDbService = inject(MockDbService);
  private jwtTokenService = inject(JWTTokenService);
  private errorHandlingService = inject(ErrorHandlingService)
  private authService = inject(AuthService)
  constructor(private http: HttpClient) {
    this.mockDbService.loadInitialMockData();
  }

  submitUserAnswers(role: string, answers: Answer[], questionnaireId: string | null): Observable<void> {
    // Step 1: Check if the user's role matches the role passed as an argument
    const userRole = this.getRoleFromToken();
    if (role !== userRole) {
      return throwError(() => new Error('Unauthorized: Role mismatch.'));
    }
  
    // Step 2: Find the active questionnaire by its ID
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(aq => aq.id === questionnaireId);
    if (!activeQuestionnaire) {
      return this.errorHandlingService.handleError(new Error('Active questionnaire not found'), 'submitData');
    }
  
    // Step 3: Find or create an answer session for the questionnaire
    let answerSession = this.mockDbService.mockData.mockAnswers.find(as => as.questionnaireId === questionnaireId);
  
    if (!answerSession) {
      // Fetch user data by ID to be used in the new session
      const studentUser = this.getUserById(activeQuestionnaire.student.id);
      const teacherUser = this.getUserById(activeQuestionnaire.teacher.id);
  
      if (!studentUser || !teacherUser) {
        return throwError(() => new Error('Student or teacher user not found.'));
      }
  
      // Create a new AnswerSession if none exists
      const newAnswerSession: AnswerSession = {
        questionnaireId: questionnaireId!,
        studentAnswers: { user: studentUser, answers: [], answeredAt: new Date() },
        teacherAnswers: { user: teacherUser, answers: [], answeredAt: new Date() },
      };
      this.mockDbService.mockData.mockAnswers.push(newAnswerSession);
      answerSession = newAnswerSession; // Assign the newly created session
    }
  
    // Step 4: Save the answers based on the user's role
    if (role === 'student') {
      // Update student answers
      answerSession.studentAnswers.answers = answers;
      answerSession.studentAnswers.answeredAt = new Date();
    } else if (role === 'teacher') {
      // Update teacher answers
      answerSession.teacherAnswers.answers = answers;
      answerSession.teacherAnswers.answeredAt = new Date();
    } else {
      return throwError(() => new Error('Unknown role.'));
    }

    if (role === 'student') {
      activeQuestionnaire.isStudentFinished = true;
    } else if (role === 'teacher') {
      activeQuestionnaire.isTeacherFinished = true;
    }
  
    this.saveData();
    return of(undefined).pipe(delay(250), catchError(this.handleError('submitUserAnswers')));

  }

  // Handle pagination for users or templates
  private paginate<T>(items: T[], page: number, limit: number): T[] {
    const startIndex = (page - 1) * limit;
    return items.slice(startIndex, startIndex + limit);
  }

  private getUserById(userId: number | null) {
    if (!userId) {
      return null;
    }
    const user = this.mockDbService.mockData.mockUsers.find(u => u.id === userId);
    return user ? user : null;
  }

  // can only be used by teachers for now
  getResults(activeQuestionnaireId: string): Observable<{ answerSession: AnswerSession, questionDetails: { questionId: string, title: string, studentOptionLabel: string, teacherOptionLabel: string }[] }> {
    // Helper function to process answer labels
    const getOptionLabel = (answer: Answer | undefined, question: Question): string => {
      if (!answer) return 'No answer provided';
      const selectedOptionId = answer.selectedOptionId;
      if (selectedOptionId !== undefined && selectedOptionId !== null) {
        const optionType = question.options.find((opt: any) => opt.id === selectedOptionId);
        return optionType ? optionType.label : 'No label found';
      } else if (answer.customAnswer) {
        return answer.customAnswer;
      }
      return 'No answer provided';
    };
  
    // Step 1: Find the active questionnaire and answer session
    const activeQuestionnaire = this.mockDbService.mockData.mockActiveQuestionnaire.find(
      (aq: ActiveQuestionnaire) => aq.id === activeQuestionnaireId
    );
    if (!activeQuestionnaire) return throwError(() => new Error('Active questionnaire not found'));
  
    const answerSession = this.mockDbService.mockData.mockAnswers?.find(
      (session: AnswerSession) => session.questionnaireId === activeQuestionnaireId
    );
    if (!answerSession) return throwError(() => new Error('Answer session not found'));
  
    // Step 2: Ensure the user is a teacher
    const userId = this.authService.getUserId();
    const user = this.mockDbService.mockData.mockUsers.find(u => u.id === userId);
    if (!user || user.role !== 'teacher') return throwError(() => new Error('User is not authorized as a teacher'));
  
    // Step 3: Find the corresponding question template
    const template = this.mockDbService.mockData.mockQuestionTemplates.find(
      qt => qt.templateId === activeQuestionnaire.questionnaireTemplate.templateId
    );
    if (!template) return throwError(() => new Error('Question template not found'));
  
    // Step 4: Process student and teacher answers
    const questionDetails = answerSession.studentAnswers.answers.map((studentAnswer, index) => {
      const questionId = studentAnswer.questionId;
      const question = template.questions.find(q => q.id === questionId);
  
      if (!question) {
        return {
          questionId: String(questionId),
          title: 'Question not found',
          studentOptionLabel: 'Question not found',
          teacherOptionLabel: 'Question not found',
        };
      }
  
      const teacherAnswer = answerSession.teacherAnswers.answers[index];
      return {
        questionId: String(questionId),
        title: question.title,
        studentOptionLabel: getOptionLabel(studentAnswer, question), // Process student answer
        teacherOptionLabel: getOptionLabel(teacherAnswer, question), // Process teacher answer
      };
    });
  
    // Step 5: Return the answer session and question details
    return of({ answerSession, questionDetails }).pipe(
      delay(500), // Simulate async behavior with a delay
      catchError((error) => throwError(() => new Error(error.message)))
    );
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
  getUsersFromSearch(role: string, nameString: string, page: number = 1, limit: number = 10): Observable<User[]> {
    // Filter users by role (student or teacher)
    let users = this.mockDbService.mockData.mockUsers.filter(u => u.role === role);
    if(nameString){
      users = users.filter(u => u.fullName.toLowerCase().includes(nameString.toLowerCase()))
    }
    // Paginate the results
    return of(this.paginate(users, page, limit)).pipe(delay(300));
  }

  getTemplatesFromSearch(titleString: string, page: number = 1, limit: number = 10): Observable<QuestionTemplate[]> {
    let templates = this.mockDbService.mockData.mockQuestionTemplates.filter(t => t.title.toLowerCase().includes(titleString.toLowerCase()));
    return of(this.paginate(templates, page, limit)).pipe(delay(300));
  }
  // Get all templates
  getTemplates(): Observable<QuestionTemplate[]> {
    return of(this.mockDbService.mockData.mockQuestionTemplates);
  }

  // Create a new template
  createTemplate(template: QuestionTemplate): Observable<void> {
    const newTemplateId = 'template' + this.generateUniqueId('template');
    const newTemplate = { ...template, templateId: newTemplateId };
    
    // Add the new template to mock data
    this.mockDbService.mockData.mockQuestionTemplates.push(newTemplate);
    this.saveData();
    
    return of(undefined).pipe(delay(300)); // Simulate delay
  }

  private generateUniqueId(type: 'template' | 'question' | 'option'): number {
    const mockData = this.mockDbService.mockData;
    
    let maxId = 0;
    if (type === 'template') {
      maxId = Math.max(...mockData.mockQuestionTemplates.map(t => Number(t.templateId.replace('template', ''))), 0);
    } else if (type === 'question') {
      maxId = Math.max(...mockData.mockQuestionTemplates.flatMap(t => t.questions.map(q => q.id)), 0);
    } else if (type === 'option') {
      maxId = Math.max(...mockData.mockQuestionTemplates.flatMap(t => t.questions.flatMap(q => q.options.map(o => o.id))), 0);
    }
    return maxId + 1;
  }

  // Update an existing template
  updateTemplate(updatedTemplate: QuestionTemplate): Observable<void> {
    const existingTemplateIndex = this.mockDbService.mockData.mockQuestionTemplates.findIndex(t => t.templateId === updatedTemplate.templateId);
    
    if (existingTemplateIndex !== -1) {
      const existingTemplate = this.mockDbService.mockData.mockQuestionTemplates[existingTemplateIndex];
  
      // Update questions and assign unique IDs where necessary
      const updatedQuestions = updatedTemplate.questions.map(question => {
        // Assign unique ID if the question is new
        if (!question.id || question.id === 0) {
          question.id = this.generateUniqueId('question');
        }
  
        // Update options for the question and assign IDs for new options
        question.options = question.options.map(option => {
          if (!option.id || option.id === 0) {
            option.id = this.generateUniqueId('option');
          }
          return option;
        });
  
        return question;
      });
  
      // Now, update the existing template with the new title, description, and questions
      existingTemplate.title = updatedTemplate.title; // Update title
      existingTemplate.description = updatedTemplate.description; // Update description
      existingTemplate.questions = updatedQuestions; // Update questions with any new ones
  
      // Save the updated template in mock data
      this.mockDbService.mockData.mockQuestionTemplates[existingTemplateIndex] = existingTemplate;
      this.saveData(); // Persist changes
    }
  
    return of(undefined).pipe(delay(300)); // Simulate delay
  }
  
  

  // Delete a template by ID
  deleteTemplate(templateId: string): Observable<void> {
    this.mockDbService.mockData.mockQuestionTemplates = this.mockDbService.mockData.mockQuestionTemplates.filter(t => t.templateId !== templateId);
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
