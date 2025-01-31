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

export const routes: Routes = [
    { path: '', component: HomeComponent },
    {path: 'hub', component: AccessHubComponent, canActivate: [authGuard]},
    { path: 'result', component:ResultComponent, canActivate: [authGuard]},
    { path: 'answer/:id', component: QuestionnaireComponent, canActivate: [authGuard]},
    {path: 'active-questionnaire', component: ActiveQuestionnaireManagerComponent, canActivate: [authGuard]},
    { path: 'templates', component: TemplateManagerComponent},
    { path: '**', component: PageNotFoundComponent}
]