export interface Template {
    id: string;
    title: string;
    description: string;
    questions: Question[];
  }
  
  export interface Question {
    id: number;
    title: string;
    customAnswer: boolean;
    options: Option[];
  }
  
  export interface Option {
    id: number;
    label: string;
  }


  export interface TemplateBase {
    id: string;
    templateTitle: string;
    createdAt: string;
    lastUpdated: string;
    isLocked: boolean;
  }
  
  export interface NextCursor {
    createdAt: string;
    id: string;
  }
  
  export interface TemplateBaseResponse {
    templateBases: TemplateBase[];
    nextCursor?: NextCursor | null;
  }
  