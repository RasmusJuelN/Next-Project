import { inject, Injectable } from '@angular/core';
import { PaginationResponse, Template } from '../models/template.model';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment.development';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class TemplateService {
  private apiUrl = `${environment.apiUrl}/templates`;
  private apiService = inject(ApiService);

  constructor() {}

  // Get templates with pagination and optional search term
  getTemplates(
    page: number,
    pageSize: number,
    searchTerm: string = '',
    searchType: string = 'name'
  ): Observable<PaginationResponse<Template>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('searchType', searchType)
      .set('pageSize', pageSize.toString())
      .set('search', searchTerm);
      
    return this.apiService.get<PaginationResponse<Template>>(this.apiUrl, params);
  }

  // Add a new template
  addTemplate(template: Template): Observable<Template> {
    return this.apiService.post<Template>(this.apiUrl, template);
  }

  // Delete a template by ID
  deleteTemplate(templateId: string): Observable<void> {
    const url = `${this.apiUrl}/${templateId}`;
    return this.apiService.delete<void>(url);
  }

  // Update an existing template
  updateTemplate(templateId: string, updatedTemplate: Template): Observable<Template> {
    const url = `${this.apiUrl}/${templateId}`;
    return this.apiService.put<Template>(url, updatedTemplate);
  }
}
