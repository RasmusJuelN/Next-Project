import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { User, ActiveQuestionnaire } from '../../../models/questionare';
import { DashboardService } from '../../../services/dashboard.service';
import { AppAuthService } from '../../../services/auth/app-auth.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['../shared-dashboard-styles.css','./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  studentList: User[] = [];
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  studentsInQuestionnaire: Set<number> = new Set<number>();

  private authService = inject(AppAuthService);
  private dashboardService = inject(DashboardService);
  private router = inject(Router);

  ngOnInit(): void {
    if (!this.authService.hasRole('admin')) {
      this.router.navigate(['/']);
    } else {
      this.loadDashboardData();
    }
  }

  loadDashboardData(): void {
    this.dashboardService.getDashboardData().subscribe((data) => {
      this.studentList = data.students;
      this.activeQuestionnaires = data.activeQuestionnaires;
      this.studentsInQuestionnaire = new Set(data.activeQuestionnaires.map(q => q.student.id));
    });
  }

  addStudentToQuestionnaire(studentId: number): void {
    this.dashboardService.addStudentToQuestionnaire(studentId).subscribe(() => {
      this.loadDashboardData();
    });
  }

  createActiveQuestionnaire(studentId: number, teacherId: number): void {
    this.dashboardService.createActiveQuestionnaire(studentId, teacherId).subscribe(() => {
      this.loadDashboardData();
    });
  }

  deleteActiveQuestionnaire(questionnaireId: string): void {
    this.dashboardService.deleteActiveQuestionnaire(questionnaireId).subscribe(() => {
      this.loadDashboardData();
    });
  }

  isStudentInQuestionnaire(studentId: number): boolean {
    return this.studentsInQuestionnaire.has(studentId);
  }
}
