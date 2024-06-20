import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { User, Question, StudentTeacherAnwser } from '../models/questionare';

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
  private mockStudentTeacherAnswers: StudentTeacherAnwser[] = [];
  private studentInQuestionare: { studentId: number, isFinished: boolean }[] = [];

  constructor(private http: HttpClient) {
    const savedData = localStorage.getItem(this.localStorageKey);
    if (savedData) {
      const parsedData = JSON.parse(savedData);
      this.mockStudents = parsedData.mockStudents;
      this.mockQuestions = parsedData.mockQuestions;
      this.mockStudentTeacherAnswers = parsedData.mockStudentTeacherAnswers;
      this.studentInQuestionare = parsedData.studentInQuestionare;
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
      this.studentInQuestionare = data.studentInQuestionare;
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
      studentInQuestionare: this.studentInQuestionare
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
  addStudentToQuestionnaire(studentId: number): void {
    const studentExists = this.mockStudents.some(student => student.id === studentId);
    const studentAvailableForQuestionnaire = this.studentInQuestionare.some(student => student.studentId === studentId && !student.isFinished);

    if (studentExists && !studentAvailableForQuestionnaire) {
      this.studentInQuestionare.push({ studentId, isFinished: false });
      this.saveData();
      console.log(`Student with ID ${studentId} added to questionnaire.`);
    } else {
      console.error(`Error: Student with ID ${studentId} not found or already in questionnaire.`);
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
    const studentAvailableForQuestionnaire = this.studentInQuestionare.some(student => student.studentId === userId && !student.isFinished);

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
    return this.studentInQuestionare.some(student => student.studentId === studentId && !student.isFinished);
  }

  /**
   * Retrieves the list of students who have not finished the questionnaire in mock data.
   * @returns An observable that emits the list of students.
   */
  getStudentsYetToFinish(): Observable<User[]> {
    const studentsYetToFinish = this.mockStudents.filter(student => {
      const studentInQ = this.studentInQuestionare.find(siq => siq.studentId === student.id);
      return studentInQ && !studentInQ.isFinished;
    });
    return of(studentsYetToFinish);
  }
}