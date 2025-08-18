import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, Renderer2 } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';

import {HeaderComponent } from './core/components/app-header/header.component';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet, HeaderComponent],
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.css'],
  // providers: [TranslateService]
})
export class AppComponent {
// private translate = inject(TranslateService);

// constructor() {
//   this.translate.addLangs(['en', 'da']);
//   this.translate.setDefaultLang('da');
//   const browserLang = this.translate.getBrowserLang();
//   this.translate.use(browserLang?.match(/en|da/) ? browserLang : 'da');
// }

// setLanguage(lang: string) {
//   this.translate.use(lang);
// }

}
