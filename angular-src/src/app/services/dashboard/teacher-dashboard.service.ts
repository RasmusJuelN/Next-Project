import { Injectable } from '@angular/core';
import { ActiveQuestionnaire } from '../../models/questionare';
import { catchError, map, Observable } from 'rxjs';
import { AppDataService } from '../data/app-data.service';
import { ErrorHandlingService } from '../error-handling.service';

type DashboardSection = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher' | 'searchResults';
type DashboardFilter = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher'

@Injectable({
  providedIn: 'root'
})
export class TeacherDashboardService {
  private readonly loadLimit = 5;

  constructor(private appDataService: AppDataService, private errorHandlingService: ErrorHandlingService) {}

  getLoadLimit(): number {
    return this.loadLimit;
  }

  loadActiveQuestionnaires(section: DashboardSection, offset: number = 0, searchQuery?: string): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getPaginatedDashboardData(section, offset, this.loadLimit, searchQuery).pipe(
      catchError(error => this.errorHandlingService.handleError(error, `Failed to load data for section: ${section}`))
    );
  }
}
