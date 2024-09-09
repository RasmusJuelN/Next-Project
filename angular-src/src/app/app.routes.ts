import { Routes } from '@angular/router';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { QuestionareComponent } from './components/questionare/questionare.component';
import { ErrorComponent } from './components/error/error.component';
import { AdminDashboardComponent } from './components/dashboard/admin-dashboard/admin-dashboard.component';
import { TeacherDashboardComponent } from './components/dashboard/teacher-dashboard/teacher-dashboard.component';
import { roleGuard } from './gurads and interceptors/role-guard.guard';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { TemplateManagerComponent } from './components/dashboard/admin-dashboard/template-manager/template-manager.component';
import { ActiveQuestionnaireManagerComponent } from './components/dashboard/admin-dashboard/active-questionnaire-manager/active-questionnaire-manager.component';
import { authGuard } from './gurads and interceptors/auth.guard';

export const routes: Routes = [
    { path: '', component: LoginPageComponent },
    {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [authGuard],
        children: [
          {
            path: 'admin',
            component: AdminDashboardComponent,
            canActivate: [roleGuard],
            data: { roles: ['admin'] } // Single role as array
          },
          {
            path: 'teacher',
            component: TeacherDashboardComponent,
            canActivate: [roleGuard],
            data: { roles: ['teacher'] } // Single role as array
          },
          {
            path: 'testing-templates',
            component: TemplateManagerComponent,
            canActivate: [roleGuard],
            data: { roles: ['admin'] } // Single role as array
          },
          {
            path: 'testing-active-questionnaire',
            component: ActiveQuestionnaireManagerComponent,
            canActivate: [roleGuard],
            data: { roles: ['admin'] } // Single role as array
          },
        ]
      },
      { 
        path: 'answer/:id', 
        canActivate: [authGuard, roleGuard],
        data: { roles: ['teacher', 'student'] }, // Allow both "teacher" and "student"
        component: QuestionareComponent
      },
      { path: 'error', component: ErrorComponent },
      { path: '**', component: PageNotFoundComponent }
]