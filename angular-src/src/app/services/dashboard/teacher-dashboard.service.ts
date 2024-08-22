import { Injectable } from '@angular/core';
import { ActiveQuestionnaire } from '../../models/questionare';
import { catchError, map, Observable, of } from 'rxjs';
import { AppDataService } from '../data/app-data.service';
import { ErrorHandlingService } from '../error-handling.service';

type DashboardSection = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher' | 'searchResults';

@Injectable({
  providedIn: 'root'
})
export class TeacherDashboardService {
  private readonly loadLimit = 5;

  private sectionStates: { [key in DashboardSection]: { collapsed: boolean; noMoreData: boolean } } = {
    finishedByStudents: { collapsed: true, noMoreData: false },
    notAnsweredByStudents: { collapsed: true, noMoreData: false },
    notAnsweredByTeacher: { collapsed: true, noMoreData: false },
    searchResults: { collapsed: false, noMoreData: false }
  };

  private sectionData: { [key in DashboardSection]: ActiveQuestionnaire[] } = {
    finishedByStudents: [],
    notAnsweredByStudents: [],
    notAnsweredByTeacher: [],
    searchResults: []
  };

  constructor(
    private appDataService: AppDataService,
    private errorHandlingService: ErrorHandlingService
  ) {}

  loadActiveQuestionnaires(
    section: DashboardSection,
    offset: number = 0,
    searchQuery?: string
  ): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getPaginatedDashboardData(section, offset, this.loadLimit, searchQuery).pipe(
      map((data) => {
        this.sectionStates[section].noMoreData = data.length < this.loadLimit;
        this.sectionData[section] = offset === 0 ? data : [...this.sectionData[section], ...data];
        return this.sectionData[section];
      }),
      catchError((error) =>
        this.errorHandlingService.handleError(error, `Failed to load data for section: ${section}`)
      )
    );
  }

  toggleSection(section: DashboardSection): Observable<ActiveQuestionnaire[]> {
    this.sectionStates[section].collapsed = !this.sectionStates[section].collapsed;
    if (!this.sectionStates[section].collapsed && this.sectionData[section].length === 0) {
      return this.loadActiveQuestionnaires(section, 0);
    }
    return of(this.sectionData[section]);
  }

  getCollapsedState(section: DashboardSection): boolean {
    return this.sectionStates[section].collapsed;
  }

  hasNoMoreData(section: DashboardSection): boolean {
    return this.sectionStates[section].noMoreData;
  }
}
