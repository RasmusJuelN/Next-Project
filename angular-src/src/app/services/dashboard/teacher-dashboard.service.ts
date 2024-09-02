import { Injectable } from '@angular/core';
import { ActiveQuestionnaire } from '../../models/questionare';
import { catchError, map, Observable } from 'rxjs';
import { AppDataService } from '../data/app-data.service';
import { ErrorHandlingService } from '../error-handling.service';
import { QuestionnaireFilter } from '../../models/dashboard';

@Injectable({
  providedIn: 'root'
})
export class TeacherDashboardService {
  private readonly loadLimit = 5;

  constructor(private appDataService: AppDataService, private errorHandlingService: ErrorHandlingService) {}

  getLoadLimit(): number {
    return this.loadLimit;
  }
  
  loadActiveQuestionnaires(filter: QuestionnaireFilter, page: number = 1): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getActiveQuestionnairePage(filter, page, this.loadLimit).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to load active questionnaires'))
    );
  }
}