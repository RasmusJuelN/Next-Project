import { Component, inject, OnInit } from '@angular/core';
import { DashboardService } from '../../../services/dashboard.service';
import { Router } from '@angular/router';
import { ActiveQuestionnaire } from '../../../models/questionare';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['../shared-dashboard-styles.css', './teacher-dashboard.component.css']
})
export class TeacherDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService); // Inject DashboardService
  router = inject(Router);
  finishedByStudents: ActiveQuestionnaire[] = [];
  notAnsweredByStudents: ActiveQuestionnaire[] = [];
  notAnsweredByTeacher: ActiveQuestionnaire[] = [];

  ngOnInit(): void {
    this.loadDashboardData();
  }

  createNewQuestionnaire(studentId: number, teacherId: number) {
    this.dashboardService.createNewQuestionnaire(studentId, teacherId).subscribe({
      next: () => this.loadDashboardData(), // Reload data after creating a new questionnaire
      error: (err) => console.error('Error creating questionnaire:', err)
    });
  }

  private loadDashboardData(): void {
    this.dashboardService.getDashboardDataTeacher().subscribe({
      next: (data) => {
        // Assign the categorized data to the respective properties
        this.finishedByStudents = data.finishedByStudents;
        this.notAnsweredByStudents = data.notAnsweredByStudents;
        this.notAnsweredByTeacher = data.notAnsweredByTeacher;
      },
      error: (err) => console.error('Error loading dashboard data:', err)
    });
  }
  toActiveQuestionnaire(urlString: string) {
    this.router.navigate([`/answer/${urlString}`]);
  }
}
