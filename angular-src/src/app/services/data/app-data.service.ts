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

  // Dashboard

  getActiveQuestionnairePage(filter: any, page: number, limit: number): Observable<ActiveQuestionnaire[]> {
    return this.dataService.getActiveQuestionnairePage(filter, page, limit);
  }
  
  getDashboardData(role:string): Observable<{ students: User[], activeQuestionnaires: ActiveQuestionnaire[] }> {
    return this.dataService.getDashboardData(role);
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

  getQuestionsForUser(templateId: string): Observable<Question[]> {
    return this.dataService.getQuestionsForUser(templateId);
  }

  submitUserAnswers(userId: number | null, role: string, answers: Question[], questionnaireId: string | null): Observable<void> {
    return this.dataService.submitData(userId, role, questionnaireId!, answers);
  }

  validateUserAccess(userId: any, role: string, questionnaireId: string): Observable<boolean> {
    return this.dataService.validateUserAccess(userId, role, questionnaireId);
  }

}
