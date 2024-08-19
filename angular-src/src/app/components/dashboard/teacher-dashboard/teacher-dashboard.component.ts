import { Component, inject } from '@angular/core';
import { DashboardService } from '../../../services/dashboard.service';
import { Router } from '@angular/router';
import { ActiveQuestionnaire } from '../../../models/questionare';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['../shared-dashboard-styles.css','./teacher-dashboard.component.css']
})
export class TeacherDashboardComponent {
  private dashboardService = inject(DashboardService); // holds the functions for it to use
  activeQuestionnaires: ActiveQuestionnaire[] = [];

  ngOnInit(): void{
    this.loadDashboardData();
  }

  createNewQuestionnaire(studentId: number, teacherId: number) {
    this.dashboardService.createNewQuestionnaire(studentId, teacherId).subscribe({
      next: (response) => {
        // Handle refreash
        this.loadDashboardData();
      },
      error: (err) => console.error('Error creating questionnaire:', err)
    });
  }

  private loadDashboardData(): void {
    this.dashboardService.getDashboardDataTeacher().subscribe({
      next: (data) => {
        this.activeQuestionnaires = data.activeQuestionnaires;
      },
      error: (err) => console.error('Error loading dashboard data:', err)
    });
  }

}