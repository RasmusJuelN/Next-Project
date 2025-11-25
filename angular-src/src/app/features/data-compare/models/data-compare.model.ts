import { User } from "../../../shared/models/user.model";



export interface DataCompare {
  id: string;
  title: string;
  description?: string | null;
  studentCompletedAt: Date;
}

export type AnswerCounts = Record<string, { count: number; dates: string[] }>;

export interface ChartBuildInput {
  question: string;
  year: string;
  answers: AnswerCounts;
}

export type ChartType = 'pie' | 'bar';

