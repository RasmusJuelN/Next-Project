import { computed, effect, Injectable, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import {
  Lang,
  DEFAULT_LANGUAGE,
  I18nLanguageConfig,
  SUPPORTED_LANGUAGES,
  SUPPORTED_CODES,
} from '../../i18n.config';

/** Check if something is one of our supported language codes */
function isSupportedLang(v: unknown): v is Lang {
  return typeof v === 'string' && SUPPORTED_CODES.includes(v as Lang);
}

/** Try to read a key from localStorage safely */
function safeGet(key: string): string | null {
  try {
    return typeof localStorage !== 'undefined'
      ? localStorage.getItem(key)
      : null;
  } catch {
    return null;
  }
}

@Injectable({
  providedIn: 'root',
})
export class I18nService {
  // expose this so appConfig can use it at bootstrap time
  public static getInitialLanguage(): Lang {
    const stored = safeGet('lang');
    return stored && isSupportedLang(stored)
      ? stored
      : DEFAULT_LANGUAGE;
  }

  readonly currentLang = signal<Lang>(DEFAULT_LANGUAGE);

  readonly options = computed<ReadonlyArray<I18nLanguageConfig>>(
    () => SUPPORTED_LANGUAGES
  );

  readonly currentOption = computed<I18nLanguageConfig>(() => {
    const code = this.currentLang();
    return SUPPORTED_LANGUAGES.find(l => l.code === code)!;
  });

  constructor(private translate: TranslateService) {
    // tell ngx-translate which languages exist
    this.translate.addLangs(SUPPORTED_CODES);
    this.translate.setFallbackLang(DEFAULT_LANGUAGE);

    // pick initial language using same logic: localStorage -> default
    const initial = I18nService.getInitialLanguage();
    this.currentLang.set(initial);
    this.translate.use(initial);

    // sync translate <-> signal <-> localStorage
    effect(() => {
      const lang = this.currentLang();

      if (this.translate.getCurrentLang() !== lang) {
        this.translate.use(lang);
      }

      this.safeSet('lang', lang);
    });

    // keep signal updated if someone else calls translate.use(...)
    this.translate.onLangChange.subscribe(ev => {
      const next = ev.lang?.toLowerCase() as Lang | undefined;
      if (next && isSupportedLang(next) && next !== this.currentLang()) {
        this.currentLang.set(next);
      }
    });
  }

  setLanguage(lang: Lang): void {
    if (isSupportedLang(lang)) {
      this.currentLang.set(lang);
    }
  }

  // ---------- internals ----------

  private safeSet(key: string, value: string): void {
    try {
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem(key, value);
      }
    } catch {
      // ignore write failures
    }
  }
}
