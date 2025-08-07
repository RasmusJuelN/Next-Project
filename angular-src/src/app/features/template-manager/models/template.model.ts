  export type TemplateStatus = 'draft' | 'finalized';

export interface Template {
  id?: string; // Optional because some templates may not have an ID yet
  title: string; // Matches API field
  description?: string; // Optional since API does not specify this field
  draftStatus: TemplateStatus;
  createdAt?: string; // ✅ Added (ISO Date format)
  lastUpdated?: string; // ✅ Added (ISO Date format)
  isLocked?: boolean; // ✅ Added (default is false)
  questions: Question[];
}

export interface Question {
  id?: number; // Optional since API may auto-generate it
  prompt: string; // Matches API field
  allowCustom: boolean; // Matches API field
  options: Option[];
}

export interface Option {
  id: number; // Matches API field
  optionValue: number; // Matches API field
  displayText: string; // Matches API field
}




  export interface TemplateBase {
    id: string;
    title: string;
    createdAt: string;
    lastUpdated: string;
    isLocked: boolean;
    draftStatus: TemplateStatus;
  }
  
  export interface TemplateBaseResponse {
    templateBases: TemplateBase[];
    queryCursor?: string; // Cursor for next items.
    totalCount: number;
  }