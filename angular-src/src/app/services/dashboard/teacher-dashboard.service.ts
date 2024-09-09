import { Injectable } from '@angular/core';
import { ActiveQuestionnaire } from '../../models/questionare';
import { catchError, map, Observable } from 'rxjs';
import { ErrorHandlingService } from '../error-handling.service';
import { QuestionnaireFilter } from '../../models/dashboard';
import { DataService } from '../data/data.service';

@Injectable({
  providedIn: 'root'
})
export class TeacherDashboardService {
  private readonly loadLimit = 5;

  constructor(private dataService: DataService, private errorHandlingService: ErrorHandlingService) {}

  getLoadLimit(): number {
    return this.loadLimit;
  }
  
  loadActiveQuestionnaires(filter: QuestionnaireFilter, page: number = 1): Observable<ActiveQuestionnaire[]> {
    return this.dataService.getActiveQuestionnairePage(filter, page, this.loadLimit).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to load active questionnaires'))
    );
  }
}