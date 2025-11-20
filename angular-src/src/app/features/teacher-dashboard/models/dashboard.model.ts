import { User } from "../../../shared/models/user.model";

/**
 * Represents a single active questionnaire entry in the teacher dashboard.
 */
export interface ActiveQuestionnaireBase {
  /** Unique identifier of the active questionnaire. */
  id: string;

  /** Title of the questionnaire. */
  title: string;

  /** Optional description of the questionnaire. */
  description?: string;

  /** Date when the questionnaire was activated. */
  activatedAt: Date;

  /** Date when the student completed it, or null if pending. */
  studentCompletedAt: Date | null;

  /** Date when the teacher completed it, or null if pending. */
  teacherCompletedAt: Date | null;

  /** The student assigned to this questionnaire (if available). */
  student?: User;

  /** The teacher assigned to this questionnaire (if available). */
  teacher?: User;
}

/**
 * Response model for fetching active questionnaires with pagination.
 */
export interface ActiveQuestionnaireResponse {
  /** List of active questionnaire entries. */
  activeQuestionnaireBases: ActiveQuestionnaireBase[];

  /** Cursor for requesting the next page, or null if none. */
  queryCursor: string | null;

  /** Total number of questionnaires available. */
  totalCount: number;
}

export interface QuestionnaireGroup {
  groupId: string;
  groupName: string;
  createdAt: string;
  templateId: string;
  questionnaires: ActiveQuestionnaireBase[];
}

// new: grouped response shape (cursor + count)
export interface QuestionnaireGroupResponse {
  groups: QuestionnaireGroup[];
  totalCount: number;
  currentPage: number;
  totalPages: number;
}