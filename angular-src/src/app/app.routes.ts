import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { HomeComponent } from './features/home/home.component';
import { AccessHubComponent } from './features/access-hub/access-hub.component';
import { PageNotFoundComponent } from './core/components/page-not-found/page-not-found.component';
import { ResultComponent } from './features/result/result.component';
import { QuestionnaireComponent } from './features/questionnaire/questionnaire.component';
import { authGuard } from './core/guards and interceptors/auth.guard';
import { TemplateManagerComponent } from './features/template-manager/template-manager.component';
import { ActiveQuestionnaireManagerComponent } from './features/active-questionnaire-manager/active-questionnaire-manager.component';
import { TeacherDashboardComponent } from './features/teacher-dashboard/teacher-dashboard.component';
import { roleGuard } from './core/guards and interceptors/role-guard.guard';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    {path: 'hub', component: AccessHubComponent, canActivate: [authGuard]},
    {
        path: 'results/:id',
        component: ResultComponent,
        canActivate: [roleGuard],
        data: { roles: ['teacher', 'student'] },
      },
    {
        path: 'answer/:id',
        component: QuestionnaireComponent,
        canActivate: [authGuard, roleGuard],
        data: { roles: ['teacher', 'student'] },
      },
      {
        path: 'active-questionnaire',
        component: ActiveQuestionnaireManagerComponent,
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
      },
    { 
        path: 'templates',
        component: TemplateManagerComponent,
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
    },
    {
        path: 'teacher-dashboard',
        component: TeacherDashboardComponent,
        canActivate: [authGuard, roleGuard],
        data: { roles: ['teacher'] },
      },
    { path: '**', component: PageNotFoundComponent}
]