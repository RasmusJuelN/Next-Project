import { Injectable } from '@angular/core';
import { ActiveQuestionnaire } from '../../models/questionare';
import { catchError, map, Observable, of } from 'rxjs';
import { AppDataService } from '../data/app-data.service';
import { ErrorHandlingService } from '../error-handling.service';
import { AppAuthService } from '../auth/app-auth.service';

// Define the types of sections available on the dashboard
type DashboardSection = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher';

@Injectable({
  providedIn: 'root'
})
export class TeacherDashboardService {
  // Configurable load limits for each section (e.g., load 5 items at a time)
  private loadLimits: { [key in DashboardSection]: number } = {
    finishedByStudents: 5,
    notAnsweredByStudents: 5,
    notAnsweredByTeacher: 5
  };

  // Tracks whether more data is available for each section
  private noMoreData: { [key in DashboardSection]: boolean } = {
    finishedByStudents: false,
    notAnsweredByStudents: false,
    notAnsweredByTeacher: false
  };

  // Tracks whether data has been loaded for each section
  private hasLoaded: { [key in DashboardSection]: boolean } = {
    finishedByStudents: false,
    notAnsweredByStudents: false,
    notAnsweredByTeacher: false
  };

  // Tracks the collapsed (visibility) state of each section
  private collapsedState: { [key in DashboardSection]: boolean } = {
    finishedByStudents: true,
    notAnsweredByStudents: true,
    notAnsweredByTeacher: true
  };

  // Caches the data for each section
  private sectionData: { [key in DashboardSection]: ActiveQuestionnaire[] } = {
    finishedByStudents: [],
    notAnsweredByStudents: [],
    notAnsweredByTeacher: []
  };

  constructor(
    private appDataService: AppDataService,
    private errorHandlingService: ErrorHandlingService,
    private appAuthService: AppAuthService
  ) {}

  searchActiveQuestionnaires(searchQuery: string): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getFilteredActiveQuestionnaires(searchQuery);
  }

  /**
   * Toggles the collapsed state of a section and loads data if it hasn't been loaded before.
   * @param section - The section to toggle (e.g., 'finishedByStudents', 'notAnsweredByStudents', 'notAnsweredByTeacher').
   * @returns An observable of the section data.
   */
  toggleSection(section: DashboardSection): Observable<ActiveQuestionnaire[]> {
    this.collapsedState[section] = !this.collapsedState[section];
    if (!this.collapsedState[section] && !this.hasLoaded[section]) {
      // Load data if the section is being expanded for the first time
      return this.loadInitialData(section);
    } else {
      // Return the already loaded data
      return of(this.sectionData[section]);
    }
  }

  /**
   * Retrieves the collapsed state of a section (whether it is expanded or collapsed).
   * @param section - The section to check.
   * @returns True if the section is collapsed, false otherwise.
   */
  getCollapsedState(section: DashboardSection): boolean {
    return this.collapsedState[section];
  }

  /**
   * Checks if there is no more data to be loaded for a section.
   * @param section - The section to check.
   * @returns True if there is no more data to load, false otherwise.
   */
  hasNoMoreData(section: DashboardSection): boolean {
    return this.noMoreData[section];
  }

  /**
   * Loads the initial set of data for a section (e.g., the first 5 items).
   * @param section - The section to load data for.
   * @returns An observable of the loaded data.
   */
  loadInitialData(section: DashboardSection): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getPaginatedDashboardData(section, 0, this.loadLimits[section]).pipe(
      map(data => {
        this.hasLoaded[section] = true;
        this.noMoreData[section] = data.length < this.loadLimits[section];
        this.sectionData[section] = data; // Cache the loaded data
        return data;
      }),
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to load initial data'))
    );
  }

  /**
   * Loads more data for a section and appends it to the existing data.
   * @param section - The section to load more data for.
   * @param currentDataLength - The current number of items loaded (used as the offset for pagination).
   * @returns An observable of the updated data.
   */
  loadMoreData(section: DashboardSection, currentDataLength: number): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getPaginatedDashboardData(section, currentDataLength, this.loadLimits[section]).pipe(
      map(data => {
        this.noMoreData[section] = data.length < this.loadLimits[section];
        this.sectionData[section] = [...this.sectionData[section], ...data]; // Append new data
        return this.sectionData[section]; // Return the updated array
      }),
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to load more data'))
    );
  }

  /**
   * Checks if the data for a section has already been loaded.
   * @param section - The section to check.
   * @returns True if the data has been loaded, false otherwise.
   */
  isDataLoaded(section: DashboardSection): boolean {
    return this.hasLoaded[section];
  }

  /**
   * Retrieves the cached data for a section.
   * @param section - The section to retrieve data for.
   * @returns An observable of the cached data.
   */
  getSectionData(section: DashboardSection): Observable<ActiveQuestionnaire[]> {
    return of(this.sectionData[section]);
  }
}
