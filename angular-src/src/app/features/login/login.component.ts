import { Component, EventEmitter, Output, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { LoginErrorCode } from '../home/models/login.model';
import { LanguageSwitcherComponent } from '../../core/components/language-switcher/language-switcher.component';
import { TrackCapsDirective } from '../../shared/directives/caps-lock';


const ERROR_I18N: Record<LoginErrorCode, string> = {
  [LoginErrorCode.InvalidCredentials]: 'LOGIN.ERRORS.INVALID',
  [LoginErrorCode.Network]:            'LOGIN.ERRORS.NETWORK',
  [LoginErrorCode.Server]:             'LOGIN.ERRORS.SERVER',
  [LoginErrorCode.Unknown]:            'LOGIN.ERRORS.GENERIC',
  // If your enum also includes these, keep them:
  [LoginErrorCode.BadRequest]:         'LOGIN.ERRORS.BAD_REQUEST',
  [LoginErrorCode.Forbidden]:          'LOGIN.ERRORS.FORBIDDEN',
  [LoginErrorCode.RateLimited]:        'LOGIN.ERRORS.RATE_LIMITED',
  [LoginErrorCode.Unavailable]:        'LOGIN.ERRORS.UNAVAILABLE',
  [LoginErrorCode.Timeout]:            'LOGIN.ERRORS.TIMEOUT',
};

@Component({
    selector: 'app-login',
    imports: [FormsModule, CommonModule, TranslateModule, LanguageSwitcherComponent, TrackCapsDirective],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  

  @Output() loggedIn = new EventEmitter<boolean>();
  @Output() errorOccurred = new EventEmitter<LoginErrorCode>();

  userName = '';
  password = '';

  errorKey: string | null = null;

  isLoading = false;
  capsLockOn = false;

  login() {
    if (this.isLoading) return;
    this.isLoading = true;
    this.errorKey = null;

    this.authService.login(this.userName, this.password).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe(res => {
      if (res.success) {
        this.loggedIn.emit(true);
        return;
      }
      this.errorKey = ERROR_I18N[res.code];
      this.errorOccurred.emit(res.code);
    });
  }

  onSubmit() {
    // Prevent submission if required fields are empty
    if (!this.userName?.trim() || !this.password?.trim()) {
      this.errorKey = 'LOGIN.ERRORS.REQUIRED_FIELDS';
      return;
    }
    this.login();
  }

  onCapsLockChange(capsLockOn: boolean) {
    this.capsLockOn = capsLockOn;
  }
}
