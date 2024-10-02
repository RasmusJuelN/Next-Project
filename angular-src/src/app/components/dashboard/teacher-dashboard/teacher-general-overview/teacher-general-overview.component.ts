import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActiveQuestionnaire } from '../../../../models/questionare';
import { Router } from '@angular/router';
import { AuthService } from '../../../../services/auth/auth.service';
import { DataService } from '../../../../services/data/data.service';

@Component({
  selector: 'app-teacher-general-overview',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-general-overview.component.html',
  styleUrl: './teacher-general-overview.component.css'
})
export class TeacherGeneralOverviewComponent {
  private dataService = inject(DataService)
  private router = inject(Router)
  private authService = inject(AuthService)
  private readonly loadLimit = 5;
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  filters: any;
  isCollapsed: boolean = true;
  currentPage: number = 1;
  noMoreData: boolean = false;


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

    this.dataService.getActiveQuestionnairePage(this.filters, this.currentPage, this.loadLimit).subscribe(
      questionnaires => {
        this.activeQuestionnaires = [...this.activeQuestionnaires, ...questionnaires];
        this.noMoreData = questionnaires.length < this.loadLimit;
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

  navigateTo(urlString: string, type: 'answer' | 'results'): void {
    this.router.navigate([`/${type}/${urlString}`]);
  }
}
