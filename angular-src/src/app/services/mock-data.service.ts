import { Injectable } from '@angular/core';
import { Question, User } from '../../models/questionare';
import { delay, of, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MockDataService {
  private mockStudents: User[] = [
    { id: 2, username: 'Nicklas', role: 'student' },
    { id: 3, username: 'Alexander', role: 'student' },
    { id: 4, username: 'Johan', role: 'student'}
  ];

  private mockQuestions: Question[] = [
    {
      id: 1,
      text: 'How well does the student understand the material?',
      options: [
        { id: 1, questionId: 1, value: -2, label: 'Does not understand at all' },
        { id: 2, questionId: 1, value: -1, label: 'Understands poorly' },
        { id: 3, questionId: 1, value: 0, label: 'Understands somewhat' },
        { id: 4, questionId: 1, value: 1, label: 'Understands well' },
        { id: 5, questionId: 1, value: 2, label: 'Understands very well' },
      ]
    },
    {
      id: 2,
      text: 'How engaged is the student during class?',
      options: [
        { id: 6, questionId: 2, value: -2, label: 'Not engaged at all' },
        { id: 7, questionId: 2, value: -1, label: 'Rarely engaged' },
        { id: 8, questionId: 2, value: 0, label: 'Sometimes engaged' },
        { id: 9, questionId: 2, value: 1, label: 'Mostly engaged' },
        { id: 10, questionId: 2, value: 2, label: 'Always engaged' },
      ]
    }
  ];

  private studentInQuestionare = [{studentId: 2, isFinished: false}, {studentId: 3, isFinished: false}];
  
  getStudents() {
    // Use the mockStudents array.
    return of(this.mockStudents);
  }

  addStudentToQuestionnaire(studentId: number) {
    const studentExists = this.mockStudents.some(student => student.id === studentId);
    const studentAvailableForQuestionnaire = this.studentInQuestionare.some(student => student.studentId === studentId && !student.isFinished);

    if (studentExists && !studentAvailableForQuestionnaire) {
      // If the student exists and is not already in the questionnaire, add the student.
      this.studentInQuestionare.push({studentId, isFinished: false});
      console.log(`Student with ID ${studentId} added to questionnaire.`);
    } else {
      // If the student does not exist or is already in the questionnaire, log an error.
      console.error(`Error: Student with ID ${studentId} not found or already in questionnaire.`);
    }
  }

  getQuestionsForUser(userId: number) {
    const userExists = this.mockStudents.some(student => student.id === userId);
    const studentAvailableForQuestionnaire = this.studentInQuestionare.some(student => student.studentId === userId && !student.isFinished);

    if (userExists && studentAvailableForQuestionnaire) {
      // If the user exists and the questionnaire is unfinished, return the questions.
      return of(this.mockQuestions);
    } else {
      // If the user does not exist or the questionnaire is finished, return an error.
      return throwError(() => new Error(`Error: User with ID ${userId} not found or questionnaire is finished.`));
    }
  }

  isStudentInQuestionnaire(studentId: number) {
    return this.studentInQuestionare.some(student => student.studentId === studentId && !student.isFinished);
  }

  getStudentsYetToFinish() {
    const studentsYetToFinish = this.mockStudents.filter(student => {
      const studentInQ = this.studentInQuestionare.find(siq => siq.studentId === student.id);
      return studentInQ && !studentInQ.isFinished;
    });
    return of(studentsYetToFinish);
  }

  
}