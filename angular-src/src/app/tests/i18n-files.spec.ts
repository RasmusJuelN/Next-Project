
// IMPORTANT:
// Make sure TypeScript can import json modules.
// In tsconfig.spec.json add:
//   "resolveJsonModule": true,
//   "esModuleInterop": true
//
// under "compilerOptions".

import da from '../../assets/i18n/da.json';
import en from '../../assets/i18n/en.json';

type FlatMap = Record<string, true>;

/**
 * Recursively walk an object and produce a flat map of keys like:
 * COMMON.BUTTONS.DELETE_PERMANENT
 * NAV.TEMPLATES
 * TMP_FINALIZE_CONFIRM
 */
function flattenKeys(obj: any, prefix = ''): FlatMap {
  const out: FlatMap = {};

  Object.keys(obj).forEach((key) => {
    const value = obj[key];
    const fullKey = prefix ? `${prefix}.${key}` : key;

    if (
      value !== null &&
      typeof value === 'object' &&
      !Array.isArray(value)
    ) {
      // recurse deeper
      const child = flattenKeys(value, fullKey);
      Object.assign(out, child);
    } else {
      // leaf string / number / etc.
      out[fullKey] = true;
    }
  });

  return out;
}

/**
 * Helper to diff the sets of keys between languages.
 * Returns missing keys: keys that are in base but not in compare.
 */
function diffKeys(base: FlatMap, compare: FlatMap): string[] {
  return Object.keys(base).filter((key) => !compare[key]);
}

describe('i18n key consistency', () => {
  // Register all languages you want to validate here.
  // The first one in the array is treated as the "source of truth".
  const languages = [
    { code: 'da', data: da },
    { code: 'en', data: en },
    // { code: 'de', data: de },
  ];

  it('all languages should define the same translation keys', () => {
    // Flatten each lang into "set of full keys"
    const flattened = languages.map((lang) => ({
      code: lang.code,
      keys: flattenKeys(lang.data),
    }));

    // Take the first language as the reference
    const base = flattened[0];

    // Collect all problems to assert once at the end
    const problems: string[] = [];

    // Compare every other language to the base
    for (let i = 1; i < flattened.length; i++) {
      const other = flattened[i];

      // 1. keys that base has but other is missing
      const missingInOther = diffKeys(base.keys, other.keys);
      if (missingInOther.length > 0) {
        problems.push(
          `[${other.code}] is missing keys that [${base.code}] has:\n` +
            missingInOther.map((k) => `  - ${k}`).join('\n')
        );
      }

      // 2. keys that other has but base is missing
      const extraInOther = diffKeys(other.keys, base.keys);
      if (extraInOther.length > 0) {
        problems.push(
          `[${other.code}] has extra keys not in [${base.code}]:\n` +
            extraInOther.map((k) => `  - ${k}`).join('\n')
        );
      }
    }

    // If we found anything, fail the test with a helpful diff
    if (problems.length > 0) {
      fail(
        '❌ i18n key mismatch detected:\n\n' +
          problems.join('\n\n') +
          '\n\nFix your translation files so they have the exact same keys.'
      );
    }
  });
});
