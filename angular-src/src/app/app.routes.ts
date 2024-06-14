import { Routes } from '@angular/router';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { QuestionareComponent } from './components/questionare/questionare.component';
import { TestingComponent } from './components/testing/testing.component';

export const routes: Routes = [
    { path: 'login-page', component: LoginPageComponent },
    { path: 'dashboard', component: DashboardComponent},
    { path: 'answer/:userId', component: QuestionareComponent},
    { path: 'testing/:questionareId', component: TestingComponent}
];
