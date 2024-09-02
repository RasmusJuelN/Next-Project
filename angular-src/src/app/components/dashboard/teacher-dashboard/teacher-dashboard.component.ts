import { Component, OnInit } from '@angular/core';
import { TeacherDashboardService } from '../../../services/dashboard/teacher-dashboard.service';
import { ActiveQuestionnaire } from '../../../models/questionare';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppAuthService } from '../../../services/auth/app-auth.service';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.css']
})
export class TeacherDashboardComponent {
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  filters: any;
  isCollapsed: boolean = true;
  currentPage: number = 1;
  noMoreData: boolean = false;

  constructor(private teacherDashboardService: TeacherDashboardService, private router: Router, private authService:AppAuthService) {}

  ngOnInit(): void {
    this.filters = {
      studentIsFinished: undefined,
      teacherIsFinished: undefined,
      teacherId: this.authService.getUserId()
    };
    this.loadQuestionnaires(true);
  }

  applyFilters(): void {
    this.loadQuestionnaires(true);
  }

  loadQuestionnaires(reset: boolean = false): void {
    if (reset) {
      this.activeQuestionnaires = [];
      this.currentPage = 1;
      this.noMoreData = false;
    }

    this.teacherDashboardService.loadActiveQuestionnaires(this.filters, this.currentPage).subscribe(
      questionnaires => {
        this.activeQuestionnaires = [...this.activeQuestionnaires, ...questionnaires];
        this.noMoreData = questionnaires.length < this.teacherDashboardService.getLoadLimit();
        if (!this.noMoreData) {
          this.currentPage++;
        }
      },
      error => console.error('Failed to load questionnaires:', error)
    );
  }

  toggleCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
  }

  onStudentFinishedChange(value: boolean | null): void {
    this.filters.studentIsFinished = value === null ? undefined : value;
    this.applyFilters();
  }

  onTeacherFinishedChange(value: boolean | null): void {
    this.filters.teacherIsFinished = value === null ? undefined : value;
    this.applyFilters();
  }

  loadMoreQuestionnaires(): void {
    if (!this.noMoreData) {
      this.loadQuestionnaires();
    }
  }

  toActiveQuestionnaire(urlString: string): void {
    this.router.navigate([`/answer/${urlString}`]);
  }
}
