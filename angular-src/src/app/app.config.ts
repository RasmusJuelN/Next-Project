import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { jwtInterceptor } from './core/guards and interceptors/jwt.interceptor';
import { AuthService } from './core/services/auth.service';
import { environment } from '../environments/environment';
import { MockAuthService } from './core/services/mock/mock.auth.service';


export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),provideHttpClient(
    withInterceptors([jwtInterceptor]),
  ), provideAnimationsAsync(),
  {
    provide: AuthService,
    useClass: environment.useMock ? MockAuthService : AuthService,
  },]
};
