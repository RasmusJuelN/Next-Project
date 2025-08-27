import { TemplateBase } from "../../../shared/models/template.model";
import { User } from "../../../shared/models/user.model";
export type TemplateStatus = 'draft' | 'finalized';

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
      activeQuestionnaireBases: ActiveQuestionnaireBase[];
      queryCursor?: string; // Cursor for next items.
      totalCount: number;
    }

    export interface UserPaginationResult {
      userBases: User[];
      sessionId: string;
      hasMore: boolean;
    }

    
    export interface TemplateBaseResponse {
      templateBases: TemplateBase[];
      queryCursor?: string; // Cursor for next items.
      totalCount: number;
    }

export { TemplateBase };
