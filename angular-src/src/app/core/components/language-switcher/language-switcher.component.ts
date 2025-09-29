import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { I18nService, Lang } from '../../services/I18n.service';


@Component({
  selector: 'app-language-switcher',
  standalone: true,
  imports: [CommonModule],
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