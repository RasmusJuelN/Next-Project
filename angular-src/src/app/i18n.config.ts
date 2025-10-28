export enum Lang {
  EN = 'en',
  DA = 'da',
}

export interface I18nLanguageConfig {
  code: Lang;
  label: string;
  flagSrc: string;
  default?: boolean;
}

export const SUPPORTED_LANGUAGES: I18nLanguageConfig[] = [
  {
    code: Lang.EN,
    label: 'LANG.EN',
    flagSrc: 'assets/icons/united-states-flag-icon.svg',
  },
  {
    code: Lang.DA,
    label: 'LANG.DA',
    flagSrc: 'assets/icons/denmark-flag-icon.svg',
    default: true,
  },
];

export const SUPPORTED_CODES = SUPPORTED_LANGUAGES.map(l => l.code) as string[];
export const DEFAULT_LANGUAGE: Lang = Lang.DA;
