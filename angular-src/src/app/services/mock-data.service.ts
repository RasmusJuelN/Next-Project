import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { User, Question, StudentTeacherAnwser } from '../models/questionare';

@Injectable({
  providedIn: 'root'
})
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

  private loadInitialData(): void {
    this.http.get('/assets/mock-data.json').subscribe((data: any) => {
      this.mockStudents = data.mockStudents;
      this.mockQuestions = data.mockQuestions;
      this.mockStudentTeacherAnswers = data.mockStudentTeacherAnswers;
      this.studentInQuestionare = data.studentInQuestionare;
      this.saveData();
    });
  }

  private saveData(): void {
    const dataToSave = {
      mockStudents: this.mockStudents,
      mockQuestions: this.mockQuestions,
      mockStudentTeacherAnswers: this.mockStudentTeacherAnswers,
      studentInQuestionare: this.studentInQuestionare
    };
    localStorage.setItem(this.localStorageKey, JSON.stringify(dataToSave));
  }

  getStudents(): Observable<User[]> {
    return of(this.mockStudents);
  }

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

  getQuestionsForUser(userId: number): Observable<Question[]> {
    const userExists = this.mockStudents.some(student => student.id === userId);
    const studentAvailableForQuestionnaire = this.studentInQuestionare.some(student => student.studentId === userId && !student.isFinished);

    if (userExists && studentAvailableForQuestionnaire) {
      return of(this.mockQuestions);
    } else {
      return throwError(() => new Error(`Error: User with ID ${userId} not found or questionnaire is finished.`));
    }
  }

  isStudentInQuestionnaire(studentId: number): boolean {
    return this.studentInQuestionare.some(student => student.studentId === studentId && !student.isFinished);
  }

  getStudentsYetToFinish(): Observable<User[]> {
    const studentsYetToFinish = this.mockStudents.filter(student => {
      const studentInQ = this.studentInQuestionare.find(siq => siq.studentId === student.id);
      return studentInQ && !studentInQ.isFinished;
    });
    return of(studentsYetToFinish);
  }
}

/*import { Injectable } from '@angular/core';
import { Question, StudentTeacherAnwser, User } from '../../models/questionare';
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
      text: 'Indlæringsevne',
      options: [
        {
          value: 1,
          label: 'Viser lidt eller ingen forståelse for arbejdsopgaverne',
        },
        {
          value: 2,
          label: 'Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden',
        },
        {
          value: 3,
          label: 'Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.',
        },
        {
          value: 4,
          label: 'Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.',
        },
        {
          value: 5,
          label: 'Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.',
        },
      ],
    },
    {
      id: 2,
      text: 'Kreativitet og selvstændighed',
      options: [
        {
          value: 1,
          label: 'Viser intet initiativ. Er passiv, uinteresseret og uselvstændig',
        },
        {
          value: 2,
          label: 'Viser ringe initiativ. Kommer ikke med løsningsforslag. Viser ingen interesse i at tilægge eget arbejde.',
        },
        {
          value: 3,
          label: 'Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.',
        },
        {
          value: 4,
          label: 'Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.',
        },
        {
          value: 5,
          label: 'Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.',
        },
      ],
    },
    {
      id: 3,
      text: 'Arbejdsindsats',
      options: [
        { value: 1, label: 'Uacceptabel' },
        { value: 2, label: 'Under middel' },
        { value: 3, label: 'Middel' },
        { value: 4, label: 'Over middel' },
        { value: 5, label: 'Særdeles god' },
      ],
    },
    {
      id: 4,
      text: 'Orden og omhyggelighed',
      options: [
        {
          value: 1,
          label: 'Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.',
        },
        {
          value: 2,
          label: 'Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.',
        },
        {
          value: 3,
          label: 'Bruger maskiner, materialer og værktøj med påpasselighed og omhyggelighed middel. Rimelig god orden.',
        },
        {
          value: 4,
          label: 'Meget påpasselig både i praktik og teori. God orden.',
        },
        {
          value: 5,
          label: 'I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.',
        },
      ],
    },
  ];
  private mockStudentTeacherAnswers: StudentTeacherAnwser[] = [
    {anwserID:1 , student: { id: 2, studentId: 2, questionId: 1, rating: 5, answerId: 1, answerDate: new Date() }, 
    teacher: { id: 1, teacherId: 1, questionId: 1, rating: 5, answerId: 1, answerDate: new Date() } },
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
  */