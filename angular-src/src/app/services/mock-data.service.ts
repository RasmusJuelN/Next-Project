import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { User, Question, StudentTeacherAnswer, ActiveQuestionnaire } from '../models/questionare';
import {jwtDecode} from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class MockDataService {
  private localStorageKey = 'mockData';
  private mockStudents: User[] = [];
  private mockTeachers: User[] = [];
  private mockQuestions: Question[] = [];
  private mockStudentTeacherAnswers: StudentTeacherAnswer[] = [];
  private mockActiveQuestionnaire: ActiveQuestionnaire[] = [];

  constructor(private http: HttpClient) {
    this.loadInitialData();
  }
  /**
   * Loads initial data either from local storage or from a mock data file.
   * If data is present in local storage, it initializes the service with that data.
   * Otherwise, it fetches data from a JSON file and saves it to local storage.
   */
  private loadInitialData(): void {
    const savedData = localStorage.getItem(this.localStorageKey);
    if (savedData) {
      const parsedData = JSON.parse(savedData);
      this.mockStudents = parsedData.mockStudents;
      this.mockTeachers = parsedData.mockTeachers;
      this.mockQuestions = parsedData.mockQuestions;
      this.mockStudentTeacherAnswers = parsedData.mockStudentTeacherAnswers;
      this.mockActiveQuestionnaire = parsedData.mockActiveQuestionnaire;
    } else {
      this.http.get('/assets/mock-data.json').subscribe((data: any) => {
        this.mockStudents = data.mockStudents;
        this.mockQuestions = data.mockQuestions;
        this.mockStudentTeacherAnswers = data.mockStudentTeacherAnswers;
        this.mockActiveQuestionnaire = data.mockActiveQuestionnaire;
        this.saveData();
      });
    }
  }
  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    const activeQuestionnaire = this.mockActiveQuestionnaire.find(aq => aq.id === id) || null;
    return of(activeQuestionnaire);
  }

  /**
   * Saves the current state of mock data to local storage.
   */
  private saveData(): void {
    const dataToSave = {
      mockStudents: this.mockStudents,
      mockTeachers: this.mockTeachers,
      mockQuestions: this.mockQuestions,
      mockStudentTeacherAnswers: this.mockStudentTeacherAnswers,
      mockActiveQuestionnaire: this.mockActiveQuestionnaire
    };
    localStorage.setItem(this.localStorageKey, JSON.stringify(dataToSave));
  }

  /**
   * Retrieves the list of mock students.
   * @returns An observable that emits the list of mock students.
   */
  getStudents(): Observable<User[]> {
    return of(this.mockStudents);
  }

  /**
   * Adds a student to the questionnaire if they exist and are not already in an active questionnaire.
   * @param studentId The ID of the student to add.
   */
  addStudentToQuestionnaire(studentId: number, teacherID = 1): void {
    const studentExists = this.mockStudents.some(student => student.id === studentId);
    const studentAvailableForQuestionnaire = this.mockActiveQuestionnaire.some(aq => aq.studentId === studentId && !aq.isStudentFinished);

    if (studentExists && !studentAvailableForQuestionnaire) {
      this.mockActiveQuestionnaire.push({ id: this.generateId(), studentId, teacherId: teacherID, isStudentFinished: false, isTeacherFinished: false });
      this.saveData();
      console.log(`Student with ID ${studentId} added to questionnaire.`);
    } else {
      console.error(`Error: Student with ID ${studentId} not found or already in questionnaire.`);
    }
  }

  /**
   * Retrieves the list of questions for a specific user if they exist and are part of an active questionnaire.
   * @param userId The ID of the user.
   * @returns An observable that emits the list of questions.
   */
  getQuestionsForUser(): Observable<Question[]> {
    return of(this.mockQuestions);
  }

  /**
   * Checks if a student is currently part of an active questionnaire.
   * @param studentId The ID of the student.
   * @returns True if the student is in an active questionnaire, false otherwise.
   */
  isStudentInQuestionnaire(studentId: number): boolean {
    return this.mockActiveQuestionnaire.some(aq => aq.studentId === studentId && !aq.isStudentFinished);
  }

  /**
   * Retrieves the list of students who have not yet finished their questionnaires.
   * @returns An observable that emits the list of students.
   */
  getStudentsYetToFinish(): Observable<User[]> {
    const studentsYetToFinish = this.mockStudents.filter(student => {
      const studentInQ = this.mockActiveQuestionnaire.find(aq => aq.studentId === student.id);
      return studentInQ && !studentInQ.isStudentFinished;
    });
    return of(studentsYetToFinish);
  }

  /**
   * Marks a user's questionnaire as finished based on their role.
   * @param userId The ID of the user.
   */
  submitData(userId: number, role: string, questionnaireId: string): void {
    // Update mockActiveQuestionnaire based on the role and questionnaireId
    for (let activeQuestionnaire of this.mockActiveQuestionnaire) {
      if (activeQuestionnaire.id == questionnaireId) {
        if (role == 'student' && activeQuestionnaire.studentId == userId) {
          activeQuestionnaire.isStudentFinished = true;
          console.log(`Student with ID ${userId} has finished the questionnaire.`);
          this.saveData();
          break;
        } else if (role == 'teacher' && activeQuestionnaire.teacherId == userId) {
          activeQuestionnaire.isTeacherFinished = true;
          console.log(`Teacher with ID ${userId} has finished the questionnaire.`);
          this.saveData();
          break;
        }
      }
    }
    console.log('Error: User not found in active questionnaire.');
    console.log("data",)
  }
   /**
   * Creates a new active questionnaire.
   * @param studentId The ID of the student.
   * @param teacherId The ID of the teacher.
   * @returns The created active questionnaire.
   */
  createActiveQuestionnaire(studentId: number, teacherId: number): ActiveQuestionnaire {
    const newActiveQuestionnaire: ActiveQuestionnaire = {
      id: this.generateId(),
      studentId: studentId,
      teacherId: teacherId,
      isStudentFinished: false,
      isTeacherFinished: false
    };

    this.mockActiveQuestionnaire.push(newActiveQuestionnaire);
    this.saveData();
    console.log('Created active questionnaire:', newActiveQuestionnaire);
    return newActiveQuestionnaire;
  }
   /**
   * Retrieves the list of active questionnaires.
   * @returns An observable that emits the list of active questionnaires.
   */
   getActiveQuestionnaires(): Observable<ActiveQuestionnaire[]> {
    return of(this.mockActiveQuestionnaire);
  }

  /**
   * Deletes an active questionnaire by its ID.
   * @param id The ID of the active questionnaire to delete.
   */
  deleteActiveQuestionnaire(id: string): void {
    this.mockActiveQuestionnaire = this.mockActiveQuestionnaire.filter(aq => aq.id !== id);
    this.saveData();
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
