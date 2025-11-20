import { inject, Injectable } from '@angular/core';

import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { TemplateBaseResponse } from '../models/template.model';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { Template } from '../../../shared/models/template.model';

/**
 * Template service.
 *
 * Handles:
 * - Listing template bases with cursor pagination.
 * - Fetching full template details.
 * - Creating, updating, and deleting templates.
 * - Finalizing (promoting) drafts.
 */
@Injectable({
  providedIn: 'root',
})
export class TemplateService {
  private apiUrl = `${environment.apiUrl}/questionnaire-template`;
  private apiService = inject(ApiService);

  constructor() {}

  /**
   * Gets a page of template bases with optional search.
   * Uses cursor-based pagination.
   *
   * @param pageSize    Number of items per page
   * @param queryCursor Cursor from previous page (optional)
   * @param searchTerm  Search text (optional)
   * @param searchType  Field to search: 'name' | 'id' (default 'name')
   */
  getTemplateBases(
    pageSize: number,
    queryCursor?: string,
    searchTerm: string = '',
    searchType: 'name' | 'id' = 'name'
  ): Observable<TemplateBaseResponse> {
    let params = new HttpParams()
      .set('PageSize', pageSize.toString())
      .set('Order', 'CreatedAtDesc');
  
    if (queryCursor) {
      params = params.set('QueryCursor', queryCursor);
    }
  
    if (searchTerm.trim()) {
      params = searchType === 'name'
        ? params.set('Title', searchTerm)
        : params.set('Id', searchTerm);
    }
  
    return this.apiService.get<TemplateBaseResponse>(this.apiUrl, params);
  }
  
  /** Gets full template details by id. */
  getTemplateDetails(id: string): Observable<Template> {
    return this.apiService.get<Template>(`${this.apiUrl}/${id}`).pipe(
      map((template: Template) => this.sortTemplateData(template))
    );
  }

  /** Creates a new template draft. */
  addTemplate(template: Template): Observable<Template> {
    // Ensure sort orders are properly set before sending
    this.ensureSortOrders(template);
    return this.apiService.post<Template>(`${this.apiUrl}/add`, template).pipe(
      map((response: Template) => this.sortTemplateData(response))
    );
  }

  /** Updates an existing template. */
  updateTemplate(templateId: string, updatedTemplate: Template): Observable<Template> {
    // Ensure sort orders are properly set before sending
    this.ensureSortOrders(updatedTemplate);
    const url = `${this.apiUrl}/${templateId}/update`;
    return this.apiService.put<Template>(url, updatedTemplate).pipe(
      map((response: Template) => this.sortTemplateData(response))
    );
  }

  /** Ensures all questions and options have proper sort orders. */
  private ensureSortOrders(template: Template): void {
    if (template.questions) {
      template.questions.forEach((question, questionIndex) => {
        if (question.sortOrder === undefined || question.sortOrder === null) {
          question.sortOrder = questionIndex;
        }
        if (question.options) {
          question.options.forEach((option, optionIndex) => {
            if (option.sortOrder === undefined || option.sortOrder === null) {
              option.sortOrder = optionIndex;
            }
          });
        }
      });
    }
  }

  /** Sorts template data by sortOrder. */
  private sortTemplateData(template: Template): Template {
    if (template.questions) {
      template.questions.sort((a, b) => a.sortOrder - b.sortOrder);
      template.questions.forEach(question => {
        if (question.options) {
          question.options.sort((a, b) => a.sortOrder - b.sortOrder);
        }
      });
    }
    return template;
  }

  /** Deletes a template by id. */
  deleteTemplate(templateId: string): Observable<void> {
    const url = `${this.apiUrl}/${templateId}/delete`;
    return this.apiService.delete<void>(url);
  }


  /**
   * updates a draft template to finalized
   */
  upgradeTemplate(templateId: string): Observable<Template>{
    const url = `${this.apiUrl}/${templateId}/finalize`;
    return this.apiService.post<Template>(url, {}).pipe(
      map((response: Template) => this.sortTemplateData(response))
    );
  }
}
