import { Component, inject } from '@angular/core';
import { TeacherDashboardService } from '../../../services/dashboard/teacher-dashboard.service';
import { ActiveQuestionnaire } from '../../../models/questionare';
import { DashboardFilter } from '../../../models/dashboard';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

type LoadSection = 'generalResults' | 'searchResults';

interface SectionState {
  data: ActiveQuestionnaire[];
  collapsed: boolean;
  noMoreData: boolean;
  currentOffset: number;
}

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['../shared-dashboard-styles.css', './teacher-dashboard.component.css']
})
export class TeacherDashboardComponent {
  // State for each section. Offset is not currently used but could be in the future.
  sectionStates: { [key in LoadSection]: SectionState } = {
    generalResults: { data: [], collapsed: true, noMoreData: false, currentOffset: 0 },
    searchResults: { data: [], collapsed: false, noMoreData: false, currentOffset: 0 }
  };

  selectedFilter: DashboardFilter = DashboardFilter.FinishedByStudents; // Default filter
  filters = Object.values(DashboardFilter); // Get all filters for dropdown
  searchQuery: string = '';

  router = inject(Router);

  constructor(private teacherDashboardService: TeacherDashboardService) {}

  ngOnInit(): void {
    this.loadResults('generalResults'); // Initial load for general results
  }

  applyFilter(): void {
    this.resetSection('generalResults');
    this.loadResults('generalResults');
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.resetSection('searchResults');
      this.loadResults('searchResults');
    }
  }

  loadResults(section: LoadSection, loadMore: boolean = false): void {
    const { currentOffset, data } = this.sectionStates[section];
    const limit = this.teacherDashboardService.getLoadLimit();

    const loadData$ = section === 'generalResults'
      ? this.teacherDashboardService.loadFilteredData(this.selectedFilter, currentOffset)
      : this.teacherDashboardService.searchQuestionnaires(this.searchQuery, currentOffset);

    loadData$.subscribe(newData => {
      this.sectionStates[section].data = loadMore ? [...data, ...newData] : newData;
      this.sectionStates[section].noMoreData = newData.length < limit;
      if (loadMore) this.sectionStates[section].currentOffset += limit;
    });
  }

  loadMore(section: LoadSection): void {
    this.loadResults(section, true);
  }

  toggleSection(section: LoadSection): void {
    this.sectionStates[section].collapsed = !this.sectionStates[section].collapsed;
    if (!this.sectionStates[section].collapsed && this.sectionStates[section].data.length === 0) {
      this.loadResults(section);
    }
  }

  resetSection(section: LoadSection): void {
    this.sectionStates[section].data = [];
    this.sectionStates[section].currentOffset = 0;
    this.sectionStates[section].noMoreData = false;
  }

  toActiveQuestionnaire(urlString: string): void {
    this.router.navigate([`/answer/${urlString}`]);
  }
}