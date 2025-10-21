import { computed, effect, Injectable, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

const LANGS = ['en', 'da'] as const;
export type Lang = (typeof LANGS)[number];
type LangOption = { code: Lang; label: string; flagSrc: string };
const DEFAULT_LANG: Lang = 'da'; // Changed to Danish as default

@Injectable({
  providedIn: 'root'
})
export class I18nService {
  readonly currentLang = signal<Lang>(DEFAULT_LANG);

  /**
   * Gets the initial language for the application
   * Priority: localStorage > default (Danish)
   */
  static getInitialLanguage(): Lang {
    // 1. Check localStorage for user's saved preference (highest priority)
    try {
      const savedLang = typeof localStorage !== 'undefined' ? localStorage.getItem('lang') : null;
      if (savedLang && (LANGS as readonly string[]).includes(savedLang)) {
        return savedLang as Lang;
      }
    } catch {
      // localStorage access failed, continue to default
    }

    // 2. Fallback to default
    return DEFAULT_LANG;
  }

  // Single source of truth for flags
  private readonly flagByCode: Record<Lang, string> = {
    en: 'assets/icons/united-states-flag-icon.svg',
    da: 'assets/icons/denmark-flag-icon.svg',
  };

  // If you prefer i18n’d labels, use keys & translate.stream (see note below)
  private readonly labelByCode: Record<Lang, string> = {
    en: 'LANG.EN',
    da: 'LANG.DA',
  };

  // Options list the component will iterate over
  readonly options = computed<ReadonlyArray<LangOption>>(() =>
    LANGS.map(code => ({
      code,
      label: this.labelByCode[code],
      flagSrc: this.flagByCode[code],
    }))
  );

  // Convenience for current display
  readonly currentOption = computed<LangOption>(() => {
    const code = this.currentLang();
    return {
      code,
      label: this.labelByCode[code],
      flagSrc: this.flagByCode[code],
    };
  });

  constructor(private translate: TranslateService) {
    this.translate.addLangs([...LANGS]);
    this.translate.setFallbackLang(DEFAULT_LANG);

    const initial = this.resolveInitialLang();
    this.currentLang.set(initial);
    this.translate.use(initial);

    effect(() => {
      const lang = this.currentLang();
      if (this.translate.getCurrentLang() !== lang) this.translate.use(lang);
      this.safeSet('lang', lang);
    });

    this.translate.onLangChange.subscribe(ev => {
      const newLang = this.normalize(ev.lang);     // <- use normalize/isSupported
      if (newLang && newLang !== this.currentLang()) {
        this.currentLang.set(newLang);
      }
    });
  }

  setLanguage(lang: Lang) {
    this.currentLang.set(lang);
  }

  // ---------- internals ----------
  private resolveInitialLang(): Lang {
    return I18nService.getInitialLanguage();
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

