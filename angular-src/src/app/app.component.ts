import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, Renderer2 } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';

import {HeaderComponent } from './core/components/app-header/header.component';
import { TranslateService } from '@ngx-translate/core';
import { LanguageSwitcherComponent } from './core/components/language-switcher/language-switcher.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet, HeaderComponent, LanguageSwitcherComponent],
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.css'],
  // providers: [TranslateService]
})
export class AppComponent {

}
