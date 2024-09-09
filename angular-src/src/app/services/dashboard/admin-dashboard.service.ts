import { inject, Injectable } from '@angular/core';
import { ActiveQuestionnaire, QuestionTemplate, User } from '../../models/questionare';
import { catchError, map, Observable } from 'rxjs';
import { ErrorHandlingService } from '../error-handling.service';
import { DataService } from '../data/data.service';

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {
  constructor(private dataService: DataService) {}
  // Active questionare Management
  createActiveQuestionnaire(student: User, teacher: User, templateId: string): Observable<ActiveQuestionnaire>{
    return this.dataService.createActiveQuestionnaire(student,teacher,templateId)
  }
  getUsersFromSearch(role: string, nameString: string, page: number = 1, limit: number = 10){
    return this.dataService.getUsersFromSearch(role,nameString,page,limit);
  }
  getActiveQuestionnairePage(filter: any, page: number, limit: number){
    return this.dataService.getActiveQuestionnairePage(filter, page, limit)
  }

  getTemplatesFromSearch(titleString: string, page: number = 1, limit: number = 10){
    return this.dataService.getTemplatesFromSearch(titleString,page,limit);
  }

  // Template Management
  getTemplates(): Observable<QuestionTemplate[]> {
    return this.dataService.getTemplates();
  }

  createTemplate(template:QuestionTemplate): Observable<void> {
    return this.dataService.createTemplate(template);
  }

  updateTemplate(template:QuestionTemplate): Observable<void> {
    return this.dataService.updateTemplate(template);
  }

  deleteTemplate(templateId: string): Observable<void> {
    return this.dataService.deleteTemplate(templateId);
  }
}
