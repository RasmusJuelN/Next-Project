import { Injectable } from '@angular/core';
import { ActiveQuestionnaire } from '../../models/questionare';
import { catchError, map, Observable, of } from 'rxjs';
import { AppDataService } from '../data/app-data.service';
import { ErrorHandlingService } from '../error-handling.service';

// Define the types of sections available on the dashboard
type DashboardSection = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher' | 'searchResults';

@Injectable({
  providedIn: 'root'
})
export class TeacherDashboardService {
  private readonly loadLimit = 5;

  private noMoreData: { [key in DashboardSection]: boolean } = {
    finishedByStudents: false,
    notAnsweredByStudents: false,
    notAnsweredByTeacher: false,
    searchResults: false
  };

  private collapsedState: { [key in DashboardSection]: boolean } = {
    finishedByStudents: true,
    notAnsweredByStudents: true,
    notAnsweredByTeacher: true,
    searchResults: true
  };

  private sectionData: { [key in DashboardSection]: ActiveQuestionnaire[] } = {
    finishedByStudents: [],
    notAnsweredByStudents: [],
    notAnsweredByTeacher: [],
    searchResults: []
  };

  constructor(
    private appDataService: AppDataService,
    private errorHandlingService: ErrorHandlingService,
  ) {}

  /**
   * Handles the loading and pagination of active questionnaires for any dashboard section.
   * @param section The dashboard section (e.g., 'finishedByStudents', 'searchResults').
   * @param offset The starting point for fetching data.
   * @param limit The maximum number of results to fetch.
   * @param searchQuery Optional search query used when fetching search results.
   * @returns An observable of the fetched ActiveQuestionnaire data.
   */
  getActiveQuestionnaires(
    section: DashboardSection,
    offset: number = 0,
    searchQuery?: string
  ): Observable<ActiveQuestionnaire[]> {
    return this.appDataService.getPaginatedDashboardData(section, offset, this.loadLimit, searchQuery).pipe(
      map((data) => {
        this.noMoreData[section] = data.length < this.loadLimit;
        this.sectionData[section] = offset === 0 ? data : [...this.sectionData[section], ...data];
        return this.sectionData[section];
      }),
      catchError((error) =>
        this.errorHandlingService.handleError(error, `Failed to load data for section: ${section}`)
      )
    );
  }

  /**
   * Searches active questionnaires based on a query.
   * @param searchQuery The search query to filter results.
   * @returns An observable of the search results.
   */
  searchActiveQuestionnaires(searchQuery: string): Observable<ActiveQuestionnaire[]> {
    this.collapsedState['searchResults'] = false;
    return this.appDataService.getFilteredActiveQuestionnaires(searchQuery).pipe(
      map(data => {
        this.sectionData['searchResults'] = data.slice(0, this.loadLimit);
        this.noMoreData['searchResults'] = data.length < this.loadLimit;
        return this.sectionData['searchResults'];
      }),
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to search questionnaires'))
    );
  }

  /**
   * Loads additional search results when the user clicks "Load More".
   * @param searchQuery The search query to filter results.
   * @returns An observable of the updated search results.
   */
  loadMoreSearchResults(searchQuery: string): Observable<ActiveQuestionnaire[]> {
    const currentDataLength = this.sectionData['searchResults'].length;
    return this.appDataService.getFilteredActiveQuestionnaires(searchQuery).pipe(
      map(data => {
        const newItems = data.slice(currentDataLength, currentDataLength + this.loadLimit);
        this.sectionData['searchResults'] = [...this.sectionData['searchResults'], ...newItems];
        this.noMoreData['searchResults'] = newItems.length < this.loadLimit;
        return this.sectionData['searchResults'];
      }),
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to load more search results'))
    );
  }

  /**
   * Toggles the collapsed state of a dashboard section and fetches the initial data if needed.
   * @param section The section to toggle and potentially load data for.
   * @returns An observable of the section data.
   */
  toggleSection(section: DashboardSection): Observable<ActiveQuestionnaire[]> {
    this.collapsedState[section] = !this.collapsedState[section];
    if (!this.collapsedState[section] && this.sectionData[section].length === 0) {
      return this.getActiveQuestionnaires(section, 0);
    }
    return of(this.sectionData[section]);
  }

  /**
   * Returns the collapsed state of a given section.
   * @param section The section to check.
   * @returns A boolean indicating whether the section is collapsed.
   */
  getCollapsedState(section: DashboardSection): boolean {
    return this.collapsedState[section];
  }

  /**
   * Determines if more data can be loaded for a given section.
   * @param section The section to check.
   * @returns A boolean indicating if there is more data to load.
   */
  hasNoMoreData(section: DashboardSection): boolean {
    return this.noMoreData[section];
  }
}
