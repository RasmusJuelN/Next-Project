import { computed, effect, Injectable, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

const LANGS = ['en', 'da'] as const;
export type Lang = (typeof LANGS)[number];


@Injectable({
  providedIn: 'root'
})
export class I18nService {
  readonly languages = LANGS;

  readonly currentLang = signal<Lang>('en');

  readonly currentLabel = computed(() =>
    this.currentLang() === 'da' ? 'Dansk' : 'English'
  );
  readonly currentFlagSrc = computed(() =>
    this.currentLang() === 'da'
      ? 'assets/icons/denmark-flag-icon.svg'
      : 'assets/icons/united-states-flag-icon.svg'
  );

constructor(private translate: TranslateService) {
  this.translate.addLangs([...LANGS]);
  this.translate.setFallbackLang('en');

  const initial = this.resolveInitialLang();
  this.currentLang.set(initial);
  this.translate.use(initial);

  effect(() => {
    const lang = this.currentLang();
    if (this.translate.getCurrentLang() !== lang) {
      this.translate.use(lang);
    }
    this.safeSet('lang', lang);
  });

  this.translate.onLangChange.subscribe(ev => {
    const newLang = this.normalize(ev.lang);
    if (newLang && newLang !== this.currentLang()) {
      this.currentLang.set(newLang);
    }
  });
}

  /** Public setter for components */
  setLanguage(lang: Lang) {
    this.currentLang.set(lang);
  }

  // ---------- internals ----------
  private resolveInitialLang(): Lang {
    const saved = this.safeGet('lang');
    if (this.isSupported(saved)) return saved as Lang;

    // SSR-safe browser detection
    const navLang = this.safeNavigatorLang();
    if (this.isSupported(navLang)) return navLang as Lang;

    return 'en';
  }

  private normalize(v: unknown): Lang | null {
    if (typeof v !== 'string') return null;
    const base = v.split('-')[0].toLowerCase(); // "en-US" -> "en"
    return this.isSupported(base) ? (base as Lang) : null;
  }

  private isSupported(v: unknown): v is Lang {
    return typeof v === 'string' && (LANGS as readonly string[]).includes(v);
  }

  private safeGet(key: string): string | null {
    try { return typeof localStorage !== 'undefined' ? localStorage.getItem(key) : null; }
    catch { return null; }
  }
  private safeSet(key: string, value: string): void {
    try { if (typeof localStorage !== 'undefined') localStorage.setItem(key, value); }
    catch {}
  }

  private safeNavigatorLang(): string | null {
    try {
      if (typeof navigator === 'undefined') return null;
      const code = (navigator.language || navigator.languages?.[0] || '').toLowerCase();
      return code ? code.split('-')[0] : null;
    } catch { return null; }
  }
}

