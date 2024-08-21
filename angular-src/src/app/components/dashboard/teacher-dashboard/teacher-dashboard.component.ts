import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TeacherDashboardService } from '../../../services/dashboard/teacher-dashboard.service';
import { ActiveQuestionnaire } from '../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// Define the types of sections available on the dashboard
type DashboardSection = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['../shared-dashboard-styles.css', './teacher-dashboard.component.css']
})
export class TeacherDashboardComponent {
  // Inject the TeacherDashboardService and Router using Angular's inject function
  private teacherDashboardService = inject(TeacherDashboardService);
  router = inject(Router);

  // Data arrays for the different sections of the dashboard
  searchResults: ActiveQuestionnaire[] = [];
  searchQuery: string = '';
  
  finishedByStudents: ActiveQuestionnaire[] = [];
  notAnsweredByStudents: ActiveQuestionnaire[] = [];
  notAnsweredByTeacher: ActiveQuestionnaire[] = [];

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.teacherDashboardService.searchActiveQuestionnaires(this.searchQuery).subscribe(data => {
        this.searchResults = data;
      });
    }
  }


  /**
   * Toggles the visibility of a dashboard section (collapsing/expanding) and fetches the data if necessary.
   * @param section - The section to toggle (e.g., 'finishedByStudents', 'notAnsweredByStudents', 'notAnsweredByTeacher').
   */
  toggleSection(section: DashboardSection): void {
    this.teacherDashboardService.toggleSection(section).subscribe(data => {
      // Updates the respective data array with the data fetched from the service
      this[section] = data;
    });
  }

  /**
   * Loads more data for a specified section and appends it to the existing list.
   * @param section - The section for which more data should be loaded (e.g., 'finishedByStudents', 'notAnsweredByStudents', 'notAnsweredByTeacher').
   */
  loadMoreData(section: DashboardSection): void {
    const currentDataLength = this[section].length; // Calculate the current number of items loaded
    this.teacherDashboardService.loadMoreData(section, currentDataLength).subscribe(data => {
      // Appends the new data to the existing data array
      this[section] = data;
    });
  }

  /**
   * Retrieves the collapsed state of a specified section.
   * @param section - The section for which the collapsed state is requested.
   * @returns True if the section is collapsed, false otherwise.
   */
  getCollapsedState(section: DashboardSection): boolean {
    return this.teacherDashboardService.getCollapsedState(section);
  }

  /**
   * Checks if there is no more data to be loaded for a specified section.
   * @param section - The section for which to check if more data is available.
   * @returns True if there is no more data to load, false otherwise.
   */
  hasNoMoreData(section: DashboardSection): boolean {
    return this.teacherDashboardService.hasNoMoreData(section);
  }

  /**
   * Navigates to a detailed view of the selected questionnaire.
   * @param urlString - The identifier for the questionnaire to navigate to.
   */
  toActiveQuestionnaire(urlString: string): void {
    this.router.navigate([`/answer/${urlString}`]);
  }
}
