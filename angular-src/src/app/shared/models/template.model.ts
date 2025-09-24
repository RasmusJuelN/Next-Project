/**
 * Status of a questionnaire template.
 */
export enum TemplateStatus {
  Draft = 'Draft',
  Finalized = 'Finalized',
}

/**
 * Full questionnaire template definition.
 */
export interface Template {
  /** Unique identifier of the template. */
  id?: string;

  /** Title of the template. */
  title: string;

  /** Optional description providing more context. */
  description?: string;

  /** Current status of the template (Draft/Finalized). */
  templateStatus: TemplateStatus;

  /** timestamp of when the template was created. */
  createdAt?: string;

  /** timestamp of the last update. */
  lastUpdated?: string;

  /** Whether the template is locked from edits. */
  isLocked?: boolean;

  /** List of questions that make up this template. */
  questions: Question[];
}

/**
 * Single question inside a questionnaire template.
 */
export interface Question {
  /** Unique question identifier. */
  id?: number;

  /** Question text. */
  prompt: string;

  /** Whether custom answers (outside predefined options) are allowed. */
  allowCustom: boolean;

  /** List of answer options available for this question. */
  options: Option[];
}

/**
 * Single answer option for a question.
 */
export interface Option {
  /** Unique identifier of the option. */
  id: number;

  /** Numeric value representing the option. */
  optionValue: number;

  /** Display text shown to the user. */
  displayText: string;
}

/**
 * Lightweight version of a template, used in lists/tables.
 */
export interface TemplateBase {
  /** Unique identifier of the template. */
  id: string;

  /** Title of the template. */
  title: string;

  /** timestamp of when the template was created. */
  createdAt: string;

  /** timestamp of the last update. */
  lastUpdated: string;

  /** Whether the template is locked. */
  isLocked: boolean;

  /** Current status of the template (Draft/Finalized). */
  templateStatus: TemplateStatus;
}