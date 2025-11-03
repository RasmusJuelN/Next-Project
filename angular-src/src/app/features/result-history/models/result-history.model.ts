import { Template, TemplateBase } from "../../../shared/models/template.model";
import { User } from "../../../shared/models/user.model";

  export interface TemplateBaseResponse {
    templateBases: TemplateBase[];
    queryCursor?: string; // Cursor for next items.
    totalCount: number;
  }

  export interface UserPaginationResult {
    userBases: User[];
    sessionId: string;
    hasMore: boolean;
  }


  export interface StudentResultHistory {
    student: User;
    teacher: User;
    template: Template;
    answersInfo: AnswerInfo[];
  }

  export interface AnswerInfo {
    activeQuestionnaireId: string;
    studentCompletedAt?: Date;
    teacherCompletedAt?: Date;
    answers: AnswerDetails[];
  }

  export interface AnswerDetails {
    questionId: string;
    studentResponse?: string;
    isStudentResponseCustom?: boolean;
    selectedOptionIdsByStudent?: number[];
    teacherResponse?: string;
    isTeacherResponseCustom?: boolean;
    selectedOptionIdsByTeacher?: number[];
  }

  // Keep the old interfaces for backward compatibility with existing mock data
  export interface Attempt {
    studentCompletedAt?: Date;
    teacherCompletedAt?: Date;
    answers: AttemptAnswer[];
  }

  export interface AttemptAnswer {
    questionId: string;
    studentResponse?: string;
    isStudentResponseCustom?: boolean;
    selectedOptionIdsByStudent?: number[];
    teacherResponse?: string;
    isTeacherResponseCustom?: boolean;
    selectedOptionIdsByTeacher?: number[];
  }