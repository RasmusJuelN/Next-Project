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

    // Base DTO for a group (similar to QuestionnaireGroupBase in backend)
export interface QuestionnaireGroupResult {
  groupId: string;
  name: string;
  createdAt: string;
  templateId: string;
  questionnaires: QuestionnaireBase[];
}

export interface QuestionnaireBase {
  id: string;
  title: string;
  description?: string;
  activatedAt: string; // or Date
  student: User;
  teacher: User;
  studentCompletedAt?: string; // or Date
  teacherCompletedAt?: string; // or Date
}
// Response DTO for keyset pagination
export interface QuestionnaireGroupKeysetPaginationResult {
  groups: QuestionnaireGroupResult[];
  currentPage: number;  
  totalPages: number;    
  totalCount: number;
}

export { TemplateBase };
