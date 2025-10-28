import da from '../../assets/i18n/da.json';
import en from '../../assets/i18n/en.json';
import { Lang, SUPPORTED_LANGUAGES } from '../i18n.config';


type FlatMap = Record<string, true>;

function flattenKeys(obj: any, prefix = ''): FlatMap {
  const out: FlatMap = {};
  Object.keys(obj).forEach((key) => {
    const value = obj[key];
    const fullKey = prefix ? `${prefix}.${key}` : key;
    if (value !== null && typeof value === 'object' && !Array.isArray(value)) {
      Object.assign(out, flattenKeys(value, fullKey));
    } else {
      out[fullKey] = true;
    }
  });
  return out;
}

function diffKeys(base: FlatMap, compare: FlatMap): string[] {
  return Object.keys(base).filter((key) => !compare[key]);
}

// Map JSON imports to Lang enum
const LANG_DATA: Record<Lang, any> = {
  [Lang.EN]: en,
  [Lang.DA]: da,
};

describe('i18n key consistency', () => {
  const languages = SUPPORTED_LANGUAGES.map((cfg) => ({
    code: cfg.code,
    data: LANG_DATA[cfg.code],
  })).filter((l) => !!l.data); // skip any without actual data

  it('all languages should define the same translation keys', () => {
    const flattened = languages.map((lang) => ({
      code: lang.code,
      keys: flattenKeys(lang.data),
    }));

    const base = flattened.find((f) => f.code === SUPPORTED_LANGUAGES.find(l => l.default)?.code) 
      || flattened[0];

    const problems: string[] = [];

    for (const other of flattened) {
      if (other.code === base.code) continue;

      const missingInOther = diffKeys(base.keys, other.keys);
      if (missingInOther.length > 0) {
        problems.push(
          `[${other.code}] is missing keys that [${base.code}] has:\n` +
          missingInOther.map((k) => `  - ${k}`).join('\n')
        );
      }

      const extraInOther = diffKeys(other.keys, base.keys);
      if (extraInOther.length > 0) {
        problems.push(
          `[${other.code}] has extra keys not in [${base.code}]:\n` +
          extraInOther.map((k) => `  - ${k}`).join('\n')
        );
      }
    }

    if (problems.length > 0) {
      fail(
        '❌ i18n key mismatch detected:\n\n' +
        problems.join('\n\n') +
        '\n\nFix your translation files so they have the exact same keys.'
      );
    }
  });
});
