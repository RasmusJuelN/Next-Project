import { ActiveQuestionnaire } from "./questionare";

export enum DashboardSection {
    generalResults = 'generalResults',
    SearchResults = 'searchResults'
  }
  
  export enum DashboardFilter {
    FinishedByStudents = 'finishedByStudents',
    NotAnsweredByStudents = 'notAnsweredByStudents',
    NotAnsweredByTeacher = 'notAnsweredByTeacher'
  }

  export type LoadSection = 'generalResults' | 'searchResults';

  export interface SectionState {
    data: ActiveQuestionnaire[];
    collapsed: boolean;
    noMoreData: boolean;
    currentOffset: number;
}