import { TemplateBase } from "../../../shared/models/template.model";

  
  export interface TemplateBaseResponse {
    templateBases: TemplateBase[];
    queryCursor?: string; // Cursor for next items.
    totalCount: number;
  }