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

  export interface PaginationResponse<T> {
    items: T[]; // The list of items for the current page
    totalItems: number; // Total number of items available
    currentPage: number; // The current page number
    pageSize: number; // Number of items per page
    totalPages: number; // Total number of pages
  }