import { Component, inject } from '@angular/core';
import { MockAuthService } from '../../services/mock-auth.service';
import { MockDataService } from '../../services/mock-data.service';
import { Router } from '@angular/router';
import { User } from '../../../models/questionare';
import { CommonModule } from '@angular/common';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  authService = inject(AuthService) // Change this to AuthService when using it really used
  dataService = inject(MockDataService) // Change this to DataService when using it really used
  router = inject(Router)

  studentList: User[] = [];
  studentsYetToFinish: User[] = [];

  ngOnInit(): void {
    const role = this.authService.getRole();
    console.log('User Role:', role);

    if ( role ==='teacher'){
      console.log(`welcome ${role}`);
      this.dataService.getStudents().subscribe((students) => {
        this.studentList = students;
      });
      this.dataService.getStudentsYetToFinish().subscribe((students) => {
        this.studentsYetToFinish = students;
      });

    }
    else if (role === 'admin'){
      console.log(`welcome ${role}`);
    }
    else if (role === 'student'){
      console.log(`welcome ${role}`);
      const userId = this.authService.getUserId();
      console.log('User ID:', userId);
      if (userId !== null) {
        this.router.navigate(['answer', userId]);
      } else {
        console.error('User ID is null, cannot navigate to answer page');
      }
    }
    else{
      console.log('Unknown role');
      this.router.navigate(['/']);
    }
  }
  
  addStudentToQuestionnaire(studentId: number): void {
    this.dataService.addStudentToQuestionnaire(studentId);
    this.dataService.getStudentsYetToFinish().subscribe((students) => {
      this.studentsYetToFinish = students;
    });
  }

  isStudentInQuestionnaire(studentId: number): boolean {
    return this.dataService.isStudentInQuestionnaire(studentId);
  }
}
