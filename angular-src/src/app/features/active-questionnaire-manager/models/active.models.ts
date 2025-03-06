import { User } from "../../../shared/models/user.model";

export interface QuestionnaireSession {
    id: string;
    templateId: string;
    templateName: string;
    createdAt: Date;
    updatedAt: Date;
  
    student: {
      user: User;
      answered: boolean;
      answeredWhen: Date | null;
    };
  
    teacher: {
      user: User;
      answered: boolean;
      answeredWhen: Date | null;
    };
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
  