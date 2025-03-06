import { Subject } from "rxjs";

export interface SearchEntity<T> {
    selected: T | null;
    searchInput: string;
    searchResults: T[];
    page: number;
    totalPages: number;
    isLoading: boolean;
    errorMessage: string | null;
    searchSubject: Subject<string>;
  }