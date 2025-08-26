export enum TemplateStatus {
  Draft = 'Draft',
  Finalized = 'Finalized',
}

export interface Template {
  id?: string;
  title: string;
  description?: string;
  templateStatus: TemplateStatus;
  createdAt?: string;
  lastUpdated?: string;
  isLocked?: boolean;
  questions: Question[];
}

export interface Question {
  id?: number;
  prompt: string;
  allowCustom: boolean;
  options: Option[];
}

export interface Option {
  id: number;
  optionValue: number; 
  displayText: string; 
}

  export interface TemplateBase {
    id: string;
    title: string;
    createdAt: string;
    lastUpdated: string;
    isLocked: boolean;
    templateStatus: TemplateStatus;
  }