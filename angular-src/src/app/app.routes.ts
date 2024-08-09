import { Routes } from '@angular/router';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { QuestionareComponent } from './components/questionare/questionare.component';
import { ErrorComponent } from './components/error/error.component';

export const routes: Routes = [
    { path: '', component: LoginPageComponent },
    { path: 'dashboard', component: DashboardComponent},
    { path: 'answer/:id', component: QuestionareComponent},
    { path: 'error', component: ErrorComponent }
];
