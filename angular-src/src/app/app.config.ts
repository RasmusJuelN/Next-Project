import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { jwtInterceptor } from './core/guards and interceptors/jwt.interceptor';
import { AuthService } from './core/services/auth.service';
import { environment } from '../environments/environment';
import { MockAuthService } from './core/services/mock/mock.auth.service';
import { HomeService } from './features/home/services/home.service';
import { MockHomeService } from './features/home/services/mock.home.service';
import { AnswerService } from './features/questionnaire/services/answer.service';
import { MockAnswerService } from './features/questionnaire/services/mock.answer.service';


export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),provideHttpClient(
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
}
]
};
