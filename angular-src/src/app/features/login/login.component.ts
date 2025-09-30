import { Component, EventEmitter, Output, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { LoginErrorCode } from '../home/models/login.model';
import { LanguageSwitcherComponent } from '../../core/components/language-switcher/language-switcher.component';


const ERROR_I18N: Record<LoginErrorCode, string> = {
  INVALID_CREDENTIALS: 'LOGIN.ERRORS.INVALID',
  NETWORK: 'LOGIN.ERRORS.NETWORK',
  SERVER: 'LOGIN.ERRORS.SERVER',
  UNKNOWN: 'LOGIN.ERRORS.GENERIC',
};

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, TranslateModule, LanguageSwitcherComponent],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
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
    this.login();
  }
}
