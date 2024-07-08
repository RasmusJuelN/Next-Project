import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, delay, map, of, throwError } from 'rxjs';
import { User, Question, StudentTeacherAnswer, ActiveQuestionnaire } from '../models/questionare';
import {jwtDecode} from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class MockDataService {
  private localStorageKey = 'mockData';
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
   * Loads initial data either from local storage or from a mock data file.
   * If data is present in local storage, it initializes the service with that data.
   * Otherwise, it fetches data from a JSON file and saves it to local storage.
   */
  private loadInitialMockData(): void {
    const savedData = localStorage.getItem(this.localStorageKey);
    if (savedData) {
      this.mockData = JSON.parse(savedData);
    } else {
      this.http.get('/assets/mock-data.json').subscribe((data: any) => {
        this.mockData = {
          mockStudents: data.mockStudents,
          mockTeachers: data.mockTeachers,
          mockQuestions: data.mockQuestions,
          mockStudentTeacherAnswers: data.mockStudentTeacherAnswers,
          mockActiveQuestionnaire: data.mockActiveQuestionnaire
        };
        this.saveData();
      });
    }
  }

    /**
   * Saves the current state of mock data to local storage.
   */
  private saveData(): void {
    localStorage.setItem(this.localStorageKey, JSON.stringify(this.mockData));
  }

  getDashboardData(): Observable<{
    students: User[],
    studentsYetToFinish: User[],
    activeQuestionnaires: ActiveQuestionnaire[]
  }> {
    const studentsYetToFinish = this.mockData.mockStudents.filter(student => {
      const studentInQ = this.mockData.mockActiveQuestionnaire.find(aq => aq.student.id === student.id);
      return studentInQ && !studentInQ.isStudentFinished;
    });
    return of({
      students: this.mockData.mockStudents,
      studentsYetToFinish: studentsYetToFinish,
      activeQuestionnaires: this.mockData.mockActiveQuestionnaire
    }).pipe(delay(250));
  }



  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    const activeQuestionnaire = this.mockData.mockActiveQuestionnaire.find(aq => aq.id === id) || null;
    return of(activeQuestionnaire).pipe(delay(250));
  }

  /**
   * Retrieves the list of mock students.
   * @returns An observable that emits the list of mock students.
   */
  getStudents(): Observable<User[]> {
    return of(this.mockData.mockStudents).pipe(delay(250));
  }

  /**
   * Adds a student to the questionnaire if they exist and are not already in an active questionnaire.
   * @param studentId The ID of the student to add.
   */
  addStudentToQuestionnaire(studentId: number, teacherId: number = 1): Observable<void> {
    const student = this.mockData.mockStudents.find(s => s.id === studentId);
    const teacher = this.mockData.mockTeachers.find(t => t.id === teacherId);
  
    if (!student || !teacher) {
      return throwError(() => new Error('Student or Teacher not found'));
    }
  
    const studentAvailableForQuestionnaire = !this.mockData.mockActiveQuestionnaire.some(aq => aq.student.id === studentId && !aq.isStudentFinished);
  
    if (studentAvailableForQuestionnaire) {
      return this.createActiveQuestionnaire(studentId, teacherId).pipe(
        map(() => {
          // Just return void after creation
          return;
        })
      );
    } else {
      return throwError(() => new Error('Student is already in an active questionnaire or has finished it'));
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
   * Retrieves the list of students who have not yet finished their questionnaires.
   * @returns An observable that emits the list of students.
   */
  getStudentsYetToFinish(): Observable<User[]> {
    const studentsYetToFinish = this.mockData.mockStudents.filter(student => {
      const studentInQ = this.mockData.mockActiveQuestionnaire.find(aq => aq.student.id === student.id);
      return studentInQ && !studentInQ.isStudentFinished;
    });
    return of(studentsYetToFinish).pipe(delay(250));
  }

  /**
   * Marks a user's questionnaire as finished based on their role.
   * @param userId The ID of the user.
   */
  submitData(userId: number, role: string, questionnaireId: string): Observable<void> {
    for (let activeQuestionnaire of this.mockData.mockActiveQuestionnaire) {
      if (activeQuestionnaire.id === questionnaireId) {
        console.log('Submitting data...', userId, role, questionnaireId)
        console.log('Active Questionnaire:', activeQuestionnaire)
        if (role === 'student' && activeQuestionnaire.student.id == userId) {
          activeQuestionnaire.isStudentFinished = true;
          this.saveData();
          return of(undefined).pipe(delay(250));
        } else if (role === 'teacher' && activeQuestionnaire.teacher.id == userId) {
          activeQuestionnaire.isTeacherFinished = true;
          this.saveData();
          return of(undefined).pipe(delay(250));
        }
      }
    }
    return throwError(() => new Error('Active questionnaire not found or user role mismatch'));
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
      return throwError(() => new Error('Student or Teacher not found'));
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
    return of(newActiveQuestionnaire).pipe(delay(250));
  }
  
   /**
   * Retrieves the list of active questionnaires.
   * @returns An observable that emits the list of active questionnaires.
   */
   getActiveQuestionnaires(): Observable<ActiveQuestionnaire[]> {
    return of(this.mockData.mockActiveQuestionnaire).pipe(delay(250));
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
}
