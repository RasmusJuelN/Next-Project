import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { jwtInterceptor } from './gurads and interceptors/jwt.interceptor';
import { AuthService } from './services/auth/auth.service';
import { MockAuthService } from './services/auth/mock-auth.service';
import { environment } from '../environments/environment';
import { DataService } from './services/data/data.service';
import { MockDataService } from './services/data/mock-data.service';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),provideHttpClient(
    withInterceptors([jwtInterceptor]),
  ), provideAnimationsAsync(),
  {
    // Provide either the real or mock auth service based on environment
    provide: AuthService, 
    useClass: environment.useMock ? MockAuthService : AuthService
  },
  {
    // Provide either the real or mock data service based on environment
    provide: DataService,
    useClass: environment.useMock ? MockDataService : DataService,
  }]
};
