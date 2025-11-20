import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';


import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {  provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { jwtInterceptor } from './core/guards and interceptors/jwt.interceptor';
import { AuthService } from './core/services/auth.service';
import { environment } from '../environments/environment';
import { MockAuthService } from './core/services/mock/mock.auth.service';
import { HomeService } from './features/home/services/home.service';
import { MockHomeService } from './features/home/services/mock.home.service';
import { AnswerService } from './features/questionnaire/services/answer.service';
import { MockAnswerService } from './features/questionnaire/services/mock.answer.service';
import { TemplateService } from './features/template-manager/services/template.service';
import { MockTemplateService } from './features/template-manager/services/mock-template.service';
import { ActiveService } from './features/active-questionnaire-manager/services/active.service';
import { MockActiveService } from './features/active-questionnaire-manager/services/mock.active.service';
import { ResultService } from './features/result/services/result.service';
import { MockResultService } from './features/result/services/mock.result.service';
import { TeacherService } from './features/teacher-dashboard/services/teacher.service';
import { MockTeacherService } from './features/teacher-dashboard/services/mock.teacher.service';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { I18nService } from './core/services/I18n.service';

describe('AppComponent (standalone)', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        // router
        provideRouter(routes),

        // http client + interceptors
        provideHttpClient(withInterceptors([jwtInterceptor])),

        // animations
        provideAnimationsAsync(),

        // swap real services for mocks so the test doesn't try to talk to backend/etc.
        { provide: AuthService, useClass: MockAuthService },
        { provide: HomeService, useClass: MockHomeService },
        { provide: AnswerService, useClass: MockAnswerService },
        { provide: TemplateService, useClass: MockTemplateService },
        { provide: ActiveService, useClass: MockActiveService },
        { provide: ResultService, useClass: MockResultService },
        { provide: TeacherService, useClass: MockTeacherService },

        // i18n / translate
        provideTranslateService({
          lang: I18nService.getInitialLanguage(),
          fallbackLang: 'da',
          loader: provideTranslateHttpLoader({
            prefix: '/assets/i18n/',
            suffix: '.json',
          }),
        }),
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });
});
