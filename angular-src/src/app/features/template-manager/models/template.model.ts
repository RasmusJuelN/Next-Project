import { TemplateBase } from "../../../shared/models/template.model";

  
/**
 * Represents a paginated response of template bases.
 */
export interface TemplateBaseResponse {
  /** Array of template bases. */
  templateBases: TemplateBase[];

  /** Cursor for requesting the next page of results (if available). */
  queryCursor?: string;

  /** Total number of templates available. */
  totalCount: number;
}