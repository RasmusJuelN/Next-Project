import { Injectable } from '@angular/core';
import { MockDataService } from './mock-data.service';
import { Observable } from 'rxjs';
import { ActiveQuestionnaire, Question, User } from '../../models/questionare';

@Injectable({
  providedIn: 'root'
})
export class AppDataService {

  constructor(
    private dataService: MockDataService
  ) {}

  getDashboardDataTeacher(teacherId:number){
    return this.dataService.getDashboardDataTeacher(teacherId);
  }

  // Dashboard
  getDashboardData(): Observable<{ students: User[], activeQuestionnaires: ActiveQuestionnaire[] }> {
    return this.dataService.getDashboardData();
  }

  addStudentToQuestionnaire(studentId: number): Observable<void> {
    return this.dataService.addStudentToQuestionnaire(studentId);
  }

  createActiveQuestionnaire(studentId: number, teacherId: number): Observable<ActiveQuestionnaire> {
    return this.dataService.createActiveQuestionnaire(studentId, teacherId);
  }

  deleteActiveQuestionnaire(questionnaireId: string): Observable<void> {
    return this.dataService.deleteActiveQuestionnaire(questionnaireId);
  }

  // Questionnaire Methods
  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    return this.dataService.getActiveQuestionnaireById(id);
  }

  getQuestionsForUser(): Observable<Question[]> {
    return this.dataService.getQuestionsForUser();
  }

  submitUserAnswers(userId: string | null, role: string, answers: Question[], questionnaireId: string | null): Observable<void> {
    return this.dataService.submitData(userId, role, questionnaireId!, answers);
  }

  validateUserAccess(userId: any, role: string, questionnaireId: string): Observable<boolean> {
    return this.dataService.validateUserAccess(userId, role, questionnaireId);
  }

}
