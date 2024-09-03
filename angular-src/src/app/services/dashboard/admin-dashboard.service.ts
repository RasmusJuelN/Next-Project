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
