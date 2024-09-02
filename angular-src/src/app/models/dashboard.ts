import { ActiveQuestionnaire } from "./questionare";

export enum DashboardSection {
    generalResults = 'generalResults',
    SearchResults = 'searchResults'
  }
  // NEW
  export interface QuestionnaireFilter {
    teacherId?: number;
    searchStudent?: string;
    studentIsFinished?: boolean;
    teacherIsFinished?: boolean;
  }