import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { TemplateBase, Template, NextCursor } from './models/template.model';
import { TemplateService } from './services/template.service';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../shared/loading/loading.component';

@Component({
  selector: 'app-template-manager',
  standalone: true,
  imports: [
    TemplateEditorComponent,
    FormsModule,
    CommonModule,
    PaginationComponent,
    LoadingComponent
  ],
  templateUrl: './template-manager.component.html',
  styleUrls: ['./template-manager.component.css'],
})
export class TemplateManagerComponent {
  private templateService = inject(TemplateService);

  templateBases: TemplateBase[] = [];

  // Caches the cursor for each page number.
  // For page 1, no cursor is needed.
  // For page N, the cursor stored at cachedCursors[N-1] is used to load page N.
  cachedCursors: { [pageNumber: number]: NextCursor | null } = {};

  selectedTemplate: Template | null = null;

  searchTerm: string = '';
  searchType: 'name' | 'id' = 'name';

  currentPage: number = 1;
  pageSize: number = 5;
  totalPages: number = 1; 
  pageSizeOptions: number[] = [5, 10, 15, 20];

  private searchSubject = new Subject<string>();
  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    // Debounce search input.
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.currentPage = 1;
        // Clear cached cursors on new search.
        this.cachedCursors = {};
        this.fetchTemplateBases();
      });

    this.fetchTemplateBases();
  }

  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.currentPage = 1;

    // Clear cached cursors when page size changes.
    this.cachedCursors = {};
    this.fetchTemplateBases();
  }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

  onSearchTypeChange(type: string): void {
    if (type === 'name' || type === 'id') {
      this.searchType = type;
      this.currentPage = 1;
      this.cachedCursors = {};
      this.fetchTemplateBases();
    }
  }

  private fetchTemplateBases(): void {
    this.isLoading = true;
    this.errorMessage = null;
  
    // For page 1, no cursor is needed.
    // For pages > 1, get the cursor from the previous page.
    const cursor = this.currentPage === 1 ? undefined : this.cachedCursors[this.currentPage - 1];
    const nextCursorCreatedAt = cursor ? cursor.createdAt : undefined;
    const nextCursorId = cursor ? cursor.id : undefined;
  
    this.templateService
      .getTemplateBases(
        this.pageSize,
        nextCursorCreatedAt,
        nextCursorId,
        this.searchTerm,
        this.searchType
      )
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (response) => {
          // Replace the current list with the fetched items.
          this.templateBases = response.templateBases;
          // Cache the nextCursor for the current page.
          // This cursor will be used to load the next page.
          this.cachedCursors[this.currentPage] = response.nextCursor || null;
  
          // Update totalPages:
          // If there's no nextCursor, then this is the last page.
          // Otherwise, we assume at least one more page exists.
          if (!response.nextCursor) {
            this.totalPages = this.currentPage;
          } else {
            // Simulate totalPages as the highest page number loaded plus one.
            this.totalPages = Math.max(this.totalPages, this.currentPage + 1);
          }
        },
        error: (err) => {
          console.error('Error fetching templates:', err);
          this.errorMessage = 'Failed to load templates. Please try again.';
        },
      });
  }
  


  handlePageChange(newPage: number): void {
    // For page 1, we can always load it.
    // For pages > 1, ensure we have the previous page's cursor cached.
    if (newPage === 1 || this.cachedCursors[newPage - 1] !== undefined) {
      this.currentPage = newPage;
      this.fetchTemplateBases();
    } else {
      console.warn('That page is not available yet.');
    }
  }

  selectTemplate(templateBaseId: string): void {
    this.selectedTemplate = null;
    this.isLoading = true;
    this.templateService.getTemplateDetails(templateBaseId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (fullTemplate) => {
          this.selectedTemplate = fullTemplate;
        },
        error: (err) => {
          console.error('Error fetching full template:', err);
          this.errorMessage = 'Failed to load the full template details.';
        },
      });
  }

  addTemplate(): void {
    const newTemplate: Template = {
      id: '', // New template has an empty id
      title: 'New Template',
      description: 'Description for the new template',
      questions: [
        {
          id: -1,
          title: 'Default Question',
          customAnswer: true,
          options: [],
        },
      ],
    };
    this.selectedTemplate = newTemplate;
  }

  deleteTemplate(templateId: string): void {
    this.templateService.deleteTemplate(templateId).subscribe({
      complete: () => this.fetchTemplateBases(),
      error: () => console.error('Error deleting template'),
    });
  }

  onSaveTemplate(updatedTemplate: Template): void {
    if (!updatedTemplate.id) {
      // New template: assign a temporary id and add it.
      updatedTemplate.id = `temp-${Date.now()}`;
      this.templateService.addTemplate(updatedTemplate).subscribe({
        next: (createdTemplate: Template) => {
          console.log('Template added successfully:', createdTemplate);
          this.selectedTemplate = null;
          this.fetchTemplateBases();
        },
        error: () => console.error('Error adding template'),
      });
    } else {
      // Existing template: update it.
      this.templateService.updateTemplate(updatedTemplate.id, updatedTemplate).subscribe({
        complete: () => {
          console.log('Template updated successfully:', updatedTemplate);
          this.selectedTemplate = null;
          this.fetchTemplateBases();
        },
        error: (err) => {
          console.error('Error updating template:', err);
        },
      });
    }
  }

  onCancelEdit(): void {
    this.selectedTemplate = null;
  }
}
