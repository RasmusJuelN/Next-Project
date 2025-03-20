import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ActiveQuestionnaireBase } from '../models/show-active.model';

@Injectable({
  providedIn: 'root'
})
export class ShowActiveService {
  constructor() { }

  fetchActiveQuestionnaires(): Observable<ActiveQuestionnaireBase[]> {
    // For demonstration, we simulate 10 sample active questionnaires:
    const sampleData: ActiveQuestionnaireBase[] = [
      {
        id: '844b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 1',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's1', fullName: 'John Doe', userName: 'jdoe', role: "student" },
        teacher: { id: 't1', fullName: 'Jane Smith', userName: 'jsmith', role: "teacher" },
        studentCompletedAt: null,
        teacherCompletedAt: null
      },
      {
        id: 'a644b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 2',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's2', fullName: 'Alice Johnson', userName: 'alicej', role: "student" },
        teacher: { id: 't2', fullName: 'Bob Brown', userName: 'bobb', role: "teacher" },
        studentCompletedAt: null,
        teacherCompletedAt: null
      },
      {
        id: 'b744b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 3',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's3', fullName: 'Charlie Davis', userName: 'charlied', role: "student" },
        teacher: { id: 't3', fullName: 'Dana Lee', userName: 'danal', role: "teacher" },
        studentCompletedAt: null,
        teacherCompletedAt: null
      },
      {
        id: 'c844b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 4',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's4', fullName: 'Emily White', userName: 'emilyw', role: "student" },
        teacher: { id: 't4', fullName: 'Frank Green', userName: 'frankg', role: "teacher" },
        studentCompletedAt: null,
        teacherCompletedAt: null
      },
      {
        id: 'd944b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 5',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's5', fullName: 'Grace Hall', userName: 'graceh', role: "student" },
        teacher: { id: 't5', fullName: 'Henry King', userName: 'henryk', role: "teacher" },
        studentCompletedAt: null,
        teacherCompletedAt: null
      },
      {
        id: 'e044b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 6',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's6', fullName: 'Ivy Moore', userName: 'ivym', role: "student" },
        teacher: { id: 't6', fullName: 'Jack Wilson', userName: 'jackw', role: "teacher" },
        studentCompletedAt: null,
        teacherCompletedAt: null
      },
      {
        id: 'f144b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 7',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's7', fullName: 'Kate Brown', userName: 'kateb', role: "student" },
        teacher: { id: 't7', fullName: 'Leo Smith', userName: 'leos', role: "teacher" },
        studentCompletedAt: null,
        teacherCompletedAt: null
      },
      {
        id: 'g244b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 8',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's8', fullName: 'Mia Turner', userName: 'miat', role: "student" },
        teacher: { id: 't8', fullName: 'Noah Evans', userName: 'noahe', role: "teacher" },
        studentCompletedAt: new Date(), // Testing date set
        teacherCompletedAt: null
      },
      {
        id: 'h344b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 9',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's9', fullName: 'Olivia Perez', userName: 'oliviap', role: "student" },
        teacher: { id: 't9', fullName: 'Paul Adams', userName: 'paula', role: "teacher" },
        studentCompletedAt: new Date(), // Testing date set
        teacherCompletedAt: null
      },
      {
        id: 'i444b8aa8-2f47-41e2-dbfb-08dd677d8866',
        title: 'Evaluering af SKP-elever 10',
        description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
        activatedAt: new Date(),
        student: { id: 's10', fullName: 'Quinn Scott', userName: 'quinns', role: "student" },
        teacher: { id: 't10', fullName: 'Rachel Lee', userName: 'rachell', role: "teacher" },
        studentCompletedAt: new Date(), // Testing date set
        teacherCompletedAt: null
      }
    ];

    return of(sampleData);
  }
}
