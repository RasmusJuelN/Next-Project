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