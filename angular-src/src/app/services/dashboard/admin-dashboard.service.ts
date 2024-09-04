import { inject, Injectable } from '@angular/core';
import { ActiveQuestionnaire, QuestionTemplate, User } from '../../models/questionare';
import { catchError, map, Observable } from 'rxjs';
import { AppDataService } from '../data/app-data.service';
import { ErrorHandlingService } from '../error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {
  constructor(private appDataService: AppDataService) {}
  // Active questionare Management
  createActiveQuestionnaire(student: User, teacher: User, templateId: string): Observable<ActiveQuestionnaire>{
    return this.appDataService.createActiveQuestionnaire(student,teacher,templateId)
  }
  getUsersFromSearch(role: string, nameString: string, page: number = 1, limit: number = 10){
    return this.appDataService.getUsersFromSearch(role,nameString,page,limit);
  }
  getActiveQuestionnairePage(filter: any, page: number, limit: number){
    return this.appDataService.getActiveQuestionnairePage(filter, page, limit)
  }

  getTemplatesFromSearch(titleString: string, page: number = 1, limit: number = 10){
    return this.appDataService.getTemplatesFromSearch(titleString,page,limit);
  }

  // Template Management
  getTemplates(): Observable<QuestionTemplate[]> {
    return this.appDataService.getTemplates();
  }

  createTemplate(template:QuestionTemplate): Observable<void> {
    return this.appDataService.createTemplate(template);
  }

  updateTemplate(template:QuestionTemplate): Observable<void> {
    return this.appDataService.updateTemplate(template);
  }

  deleteTemplate(templateId: string): Observable<void> {
    return this.appDataService.deleteTemplate(templateId);
  }
}
