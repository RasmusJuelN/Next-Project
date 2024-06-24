import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { User, Question, StudentTeacherAnswer, ActiveQuestionnaire } from '../models/questionare';

@Injectable({
  providedIn: 'root'
})
/**
 * Service for managing mock data.
 * This service provides methods to retrieve and manipulate mock data related to students and questionnaires.
 */
/**
 * Service for managing mock data.
 */
export class MockDataService {
  private localStorageKey = 'mockData';

  private mockStudents: User[] = [];
  private mockQuestions: Question[] = [];
  private mockStudentTeacherAnswers: StudentTeacherAnswer[] = [];
  private mockActiveQuestionnaire: ActiveQuestionnaire[] = [];

  constructor(private http: HttpClient) {
    const savedData = localStorage.getItem(this.localStorageKey);
    if (savedData) {
      const parsedData = JSON.parse(savedData);
      this.mockStudents = parsedData.mockStudents;
      this.mockQuestions = parsedData.mockQuestions;
      this.mockStudentTeacherAnswers = parsedData.mockStudentTeacherAnswers;
      this.mockActiveQuestionnaire = parsedData.mockActiveQuestionnaire;
    } else {
      this.loadInitialData();
    }
  }

  /**
   * Loads the initial data from the mock-data.json file.
   * Saves the loaded data to the local storage.
   */
  private loadInitialData(): void {
    this.http.get('/assets/mock-data.json').subscribe((data: any) => {
      this.mockStudents = data.mockStudents;
      this.mockQuestions = data.mockQuestions;
      this.mockStudentTeacherAnswers = data.mockStudentTeacherAnswers;
      this.mockActiveQuestionnaire = data.mockActiveQuestionnaire;
      this.saveData();
    });
  }

  /**
   * Saves the mock data to the local storage.
   */
  private saveData(): void {
    const dataToSave = {
      mockStudents: this.mockStudents,
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
   * Adds a student to the questionnaire in local memory.
   * @param studentId The ID of the student to add.
   */
  addStudentToQuestionnaire(_studentId: number): void {
    const studentExists = this.mockStudents.some(student => student.id === _studentId);
    const studentAvailableForQuestionnaire = this.mockActiveQuestionnaire.some(student => student.studentId === _studentId && !student.isStudentFinished);

    if (studentExists && !studentAvailableForQuestionnaire) {
      this.mockActiveQuestionnaire.push({studentId:_studentId, TeacherId: 1, isStudentFinished: false, isTeacherFinished: false});
      this.saveData();
      console.log(`Student with ID ${_studentId} added to questionnaire.`);
    } else {
      console.error(`Error: Student with ID ${_studentId} not found or already in questionnaire.`);
    }
  }

  /**
   * Retrieves the list of questions for a user using mock data.
   * @param userId The ID of the user.
   * @returns An observable that emits the list of questions.
   * @throws An error if the user is not found or the questionnaire is finished.
   */
  getQuestionsForUser(userId: number): Observable<Question[]> {
    const userExists = this.mockStudents.some(student => student.id === userId);
    const studentAvailableForQuestionnaire = this.mockActiveQuestionnaire.some(student => student.studentId === userId && !student.isStudentFinished);

    if (userExists && studentAvailableForQuestionnaire) {
      return of(this.mockQuestions);
    } else {
      return throwError(() => new Error(`Error: User with ID ${userId} not found or questionnaire is finished.`));
    }
  }

  /**
   * Checks if a student is in the mock questionnaire.
   * @param studentId The ID of the student.
   * @returns True if the student is in the questionnaire, false otherwise.
   */
  isStudentInQuestionnaire(studentId: number): boolean {
    return this.mockActiveQuestionnaire.some(student => student.studentId === studentId && !student.isStudentFinished);
  }

  /**
   * Retrieves the list of students who have not finished the questionnaire in mock data.
   * @returns An observable that emits the list of students.
   */
  getStudentsYetToFinish(): Observable<User[]> {
    const studentsYetToFinish = this.mockStudents.filter(student => {
      const studentInQ = this.mockActiveQuestionnaire.find(siq => siq.studentId === student.id);
      return studentInQ && !studentInQ.isStudentFinished;
    });
    return of(studentsYetToFinish);
  }

  generateId(): string {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_';
    const idLength = 4;
    let result = '';
    const charactersLength = characters.length;
    for (let i = 0; i < idLength; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    console.log('Generated ID:', result);
    return result;
  }

}