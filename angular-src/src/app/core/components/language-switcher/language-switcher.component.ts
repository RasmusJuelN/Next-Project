import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { I18nService } from '../../services/I18n.service';
import { Lang } from '../../../i18n.config';


@Component({
    selector: 'app-language-switcher',
    imports: [CommonModule, TranslateModule],
    templateUrl: './language-switcher.component.html',
    styleUrl: './language-switcher.component.css'
})
export class LanguageSwitcherComponent {
  i18n = inject(I18nService);
  showDropdown = false;
  
  onSelect(lang: Lang) {
    this.i18n.setLanguage(lang);
    this.showDropdown = false;
  }
}