import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TeacherDashboardService } from '../../../services/dashboard/teacher-dashboard.service';
import { ActiveQuestionnaire } from '../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type DashboardSection = 'finishedByStudents' | 'notAnsweredByStudents' | 'notAnsweredByTeacher' | 'searchResults';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['../shared-dashboard-styles.css', './teacher-dashboard.component.css']
})
export class TeacherDashboardComponent {
  private teacherDashboardService = inject(TeacherDashboardService);
  router = inject(Router);

  finishedByStudents: ActiveQuestionnaire[] = [];
  notAnsweredByStudents: ActiveQuestionnaire[] = [];
  notAnsweredByTeacher: ActiveQuestionnaire[] = [];
  searchResults: ActiveQuestionnaire[] = [];
  searchQuery: string = '';

  toggleSection(section: DashboardSection): void {
    this.teacherDashboardService.toggleSection(section).subscribe(data => {
      this[section] = data;
    });
  }

  loadMoreData(section: DashboardSection): void {
    const currentDataLength = this[section].length;
    this.teacherDashboardService.getActiveQuestionnaires(section, currentDataLength).subscribe(data => {
      this[section] = data;
    });
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.teacherDashboardService.getActiveQuestionnaires('searchResults', 0, this.searchQuery).subscribe(data => {
        this.searchResults = data;
      });
    }
  }
  
  loadMoreSearchResults(): void {
    const currentDataLength = this.searchResults.length;
    this.teacherDashboardService.getActiveQuestionnaires('searchResults', currentDataLength, this.searchQuery).subscribe(data => {
      this.searchResults = data;
    });
  }

  getCollapsedState(section: DashboardSection): boolean {
    return this.teacherDashboardService.getCollapsedState(section);
  }

  hasNoMoreData(section: DashboardSection): boolean {
    return this.teacherDashboardService.hasNoMoreData(section);
  }

  toActiveQuestionnaire(urlString: string): void {
    this.router.navigate([`/answer/${urlString}`]);
  }
}
