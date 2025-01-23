import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, of } from "rxjs";
import { TokenService } from "../token.service";

@Injectable({
    providedIn: 'root'
  })
  export class MockAuthService {
    private isAuthenticatedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
    tokenService = inject(TokenService)

    loginAuthentication(userName: string, password: string) {
        if (userName === 'test' && password === 'password') {
          this.isAuthenticatedSubject.next(true);
          const fakeToken = JSON.stringify({ sub: 'mockUserId', scope: 'mockUserRole', exp: Date.now() / 1000 + 3600 });
          this.tokenService.setToken(fakeToken);

          return of(true);
        } else {
          this.isAuthenticatedSubject.next(false);
          return of(false);
        }
      }

      logout() {
        // Simulate clearing user session
        localStorage.removeItem('access_token');
        this.isAuthenticatedSubject.next(false);
      }
  }