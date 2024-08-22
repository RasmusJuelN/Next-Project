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
  private readonly loadLimit = this.teacherDashboardService.getLoadLimit();  


  sectionData: { [key in DashboardSection]: ActiveQuestionnaire[] } = {
    finishedByStudents: [],
    notAnsweredByStudents: [],
    notAnsweredByTeacher: [],
    searchResults: []
  };
  sectionStates: { [key in DashboardSection]: { collapsed: boolean; noMoreData: boolean } } = {
    finishedByStudents: { collapsed: true, noMoreData: false },
    notAnsweredByStudents: { collapsed: true, noMoreData: false },
    notAnsweredByTeacher: { collapsed: true, noMoreData: false },
    searchResults: { collapsed: false, noMoreData: false }
  };

  searchQuery: string = '';

  constructor(private teacherDashboardService: TeacherDashboardService, private router: Router) {}

  toggleSection(section: DashboardSection): void {
    this.sectionStates[section].collapsed = !this.sectionStates[section].collapsed;
    if (!this.sectionStates[section].collapsed && this.sectionData[section].length === 0) {
      this.loadMoreData(section);
    }
  }

  loadMoreData(section: DashboardSection): void {
    const currentDataLength = this.sectionData[section].length;
    this.teacherDashboardService.loadActiveQuestionnaires(section, currentDataLength).subscribe(data => {
      this.sectionData[section] = [...this.sectionData[section], ...data];
      this.sectionStates[section].noMoreData = data.length < this.loadLimit;
    });
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.teacherDashboardService.loadActiveQuestionnaires('searchResults', 0, this.searchQuery).subscribe(data => {
        this.sectionData['searchResults'] = data;
        this.sectionStates['searchResults'].noMoreData = data.length < this.loadLimit;
      });
    }
  }

  loadMoreSearchResults(): void {
    const currentDataLength = this.sectionData["searchResults"].length;
    this.teacherDashboardService.loadActiveQuestionnaires('searchResults', currentDataLength, this.searchQuery).subscribe(data => {
      if (data.length > 0) {
        this.sectionData["searchResults"] = [...this.sectionData["searchResults"], ...data];
      }
      this.sectionStates['searchResults'].noMoreData = data.length < this.loadLimit;
    });
  }

  toActiveQuestionnaire(urlString: string): void {
    this.router.navigate([`/answer/${urlString}`]);
  }
}
