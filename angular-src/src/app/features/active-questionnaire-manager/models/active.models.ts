import { User } from "../../../shared/models/user.model";

export interface ActiveQuestionnaire {
    id: string;
    title: string;
    description?: string;
    activatedAt: Date;
    student: User;
    teacher: User;
    studentCompletedAt: Date | null;
    teacherCompletedAt: Date | null;
}
  

  export interface Template {
    id?: string; // Optional because some templates may not have an ID yet
    templateTitle: string; // Matches API field
    description?: string; // Optional since API does not specify this field
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
  
  export interface ActiveQuestionnaireBase {
    id: string;
    title?: string
    description?: string
    activatedAt: Date
    student: User
    teacher: User
    studentCompletedAt: Date | null;
    teacherCompletedAt: Date | null;
  }


    export interface ResponseActiveQuestionnaireBase {
      activeQuestionnaireBase: ActiveQuestionnaireBase[];
      queryCursor?: string; // Cursor for next items.
      totalCount: number;
    }