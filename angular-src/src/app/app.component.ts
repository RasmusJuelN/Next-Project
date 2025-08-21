import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, Renderer2 } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';

import {HeaderComponent } from './core/components/app-header/header.component';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet, HeaderComponent],
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.css'],
  // providers: [TranslateService]
})
export class AppComponent {
 private translate = inject(TranslateService);
 // Dropdown state
  showDropdown = false;
  currentLang = 'en';
  currentFlag = 'assets/icons/united-states-flag-icon.svg';

constructor() {
  this.translate.addLangs(['en', 'da']);
  this.translate.setDefaultLang('da');
  const browserLang = this.translate.getBrowserLang();
  this.translate.use(browserLang?.match(/en|da/) ? browserLang : 'da');

   this.currentLang = this.translate.currentLang || 'en';
    this.currentFlag =
      this.currentLang === 'da'
        ? 'assets/icons/denmark-flag-icon.svg'
        : 'assets/icons/united-states-flag-icon.svg';
  
}

setLanguage(lang: string) {
    this.currentLang = lang;
    this.currentFlag =
      lang === 'da'
        ? 'assets/icons/denmark-flag-icon.svg'
        : 'assets/icons/united-states-flag-icon.svg';

    this.translate.use(lang);
    this.showDropdown = false; // close menu after selection
  }

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

}
