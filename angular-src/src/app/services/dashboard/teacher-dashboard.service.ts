import { Injectable } from '@angular/core';
import { ActiveQuestionnaire } from '../../models/questionare';
import { catchError, map, Observable } from 'rxjs';
import { AppDataService } from '../data/app-data.service';
import { ErrorHandlingService } from '../error-handling.service';
import { DashboardFilter, DashboardSection } from '../../models/dashboard';

@Injectable({
  providedIn: 'root'
})
export class TeacherDashboardService {
  private readonly loadLimit = 5;

  constructor(private appDataService: AppDataService, private errorHandlingService: ErrorHandlingService) {}

  getLoadLimit(): number {
    return this.loadLimit;
  }
  
  loadFilteredData(filter: DashboardFilter, offset: number = 0): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getPaginatedDashboardData(DashboardSection.generalResults, filter, offset).pipe(
      catchError(error => this.errorHandlingService.handleError(error, `Failed to load data for filter: ${filter}`))
    );
  }

    // NEW Function: Load search results based on query
    searchQuestionnaires(searchQuery: string, offset: number = 0): Observable<ActiveQuestionnaire[]> {
      return this.appDataService.getPaginatedDashboardData(DashboardSection.SearchResults, null, offset, this.loadLimit, searchQuery).pipe(
        catchError(error => this.errorHandlingService.handleError(error, `Failed to load search results for query: ${searchQuery}`))
      );
    }
}