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




export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
    provideHttpClient(
    withInterceptors([jwtInterceptor]),
  ), provideAnimationsAsync(),
  {
    provide: AuthService,
    useClass: environment.useMock ? MockAuthService : AuthService,
  },
  {
    provide: HomeService,
    useClass: environment.useMock ? MockHomeService : HomeService,
  },
  {
    provide: AnswerService,
    useClass: environment.useMock ? MockAnswerService : AnswerService,
  },
  {
    provide: TemplateService,
    useClass: environment.useMock ? MockTemplateService : TemplateService
  },
  {
    provide: ActiveService,
    useClass: environment.useMock ? MockActiveService : ActiveService,
  },
  {
    provide: ResultService,
    useClass: environment.useMock ? MockResultService : ResultService
  },
  {
    provide: TeacherService,
    useClass: environment.useMock ? MockTeacherService : TeacherService
  },
  {
    provide: APP_INITIALIZER,// if not initialized here, it will cause issues when using browser bar
    useFactory: (authService: AuthService) => () => authService.initializeAuthState(),
    deps: [AuthService],
    multi: true,
  },
  {
    provide: APP_INITIALIZER,
    useFactory: (i18nService: I18nService) => () => {
      // The constructor already handles initialization
      return Promise.resolve();
    },
    deps: [I18nService],
    multi: true,
  },
 
    provideTranslateService({
      lang: I18nService.getInitialLanguage(),            
      fallbackLang: 'da',
      loader: provideTranslateHttpLoader({
        prefix: '/assets/i18n/',  
        suffix: '.json',          
      }),
    }),
  ],
};
