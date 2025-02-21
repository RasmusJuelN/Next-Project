import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { TemplateBase, Template } from './models/template.model';
import { TemplateService } from './services/template.service';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { PaginationComponent, PageChangeEvent } from '../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../shared/loading/loading.component';

@Component({
  selector: 'app-template-manager',
  standalone: true,
  imports: [
    TemplateEditorComponent,
    FormsModule,
    CommonModule,
    PaginationComponent,
    LoadingComponent,
  ],
  templateUrl: './template-manager.component.html',
  styleUrls: ['./template-manager.component.css'],
})
export class TemplateManagerComponent {
  private templateService = inject(TemplateService);

  templateBases: TemplateBase[] = [];
  // Caching cursors using page numbers.
  cachedCursors: { [pageNumber: number]: string | null } = {};
  selectedTemplate: Template | null = null;

  // Search & Pagination parameters.
  searchTerm = '';
  searchType: 'name' | 'id' = 'name';
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;
  pageSizeOptions: number[] = [5, 10, 15, 20];

  isLoading = false;
  errorMessage: string | null = null;
  private searchSubject = new Subject<string>();

  ngOnInit(): void {
    // Debounce search input to limit API calls.
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.resetData(); // Reset pagination and selection on new search.
        this.fetchTemplateBases();
      });

    // Initial data load.
    this.fetchTemplateBases();
  }

  // Called on each keystroke.
  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  // Called when page size changes.
  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.resetData();
    this.fetchTemplateBases();
  }

  // Called when search type (name or id) is changed.
  onSearchTypeChange(type: string): void {
    if (type === 'name' || type === 'id') {
      this.searchType = type;
      this.resetData();
      this.fetchTemplateBases();
    }
  }

  /**
   * Resets key state values.
   * @param resetSearch - If true, also resets searchTerm and searchType.
   */
  resetData(resetSearch: boolean = false): void {
    this.currentPage = 1;
    this.totalPages = 1;
    this.cachedCursors = {};
    this.selectedTemplate = null;
    if (resetSearch) {
      this.searchTerm = '';
      this.searchType = 'name';
    }
  }

  /**
   * Fetches the template bases based on current state.
   */
  private fetchTemplateBases(): void {
    this.isLoading = true;
    this.errorMessage = null;
    const nextCursor = this.cachedCursors[this.currentPage] ?? undefined;

    this.templateService
      .getTemplateBases(this.pageSize, nextCursor, this.searchTerm, this.searchType)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (response) => {
          this.templateBases = response.templateBases;

          if (response.templateBases.length === 0) {
            // No data means we've hit the end.
            this.totalPages = this.currentPage;
            this.cachedCursors[this.currentPage + 1] = null;
          } else if (response.queryCursor) {
            // Store the next cursor and calculate total pages from totalCount.
            this.cachedCursors[this.currentPage + 1] = response.queryCursor;
            this.totalPages = Math.ceil(response.totalCount / this.pageSize);
          } else {
            this.totalPages = this.currentPage;
          }
        },
        error: (err) => {
          console.error('Error fetching templates:', err);
          this.errorMessage = 'Failed to load templates. Please try again.';
        },
      });
  }

  /**
   * Handles page change events.
   * Uses the event payload (new page number and direction) to update state and fetch data.
   */
  handlePageChange(event: PageChangeEvent): void {
    // For forward, backward, or jump, update currentPage and then fetch data.
    const newPage = event.page;

    // If moving forward and the cursor isn't cached yet, fetch data.
    if (event.direction === 'forward' && newPage > this.currentPage && !this.cachedCursors[newPage]) {
      console.warn('Page not available yet. Fetching...');
      this.currentPage = newPage;
      this.fetchTemplateBases();
      return;
    }

    // For backward or jump, we assume the cursor was already cached.
    this.currentPage = newPage;
    this.fetchTemplateBases();
  }

  // Opens the editor for the given template.
  selectTemplate(templateBaseId: string): void {
    this.selectedTemplate = null;
    this.isLoading = true;
    this.templateService
      .getTemplateDetails(templateBaseId)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (fullTemplate) => (this.selectedTemplate = fullTemplate),
        error: (err) => {
          console.error('Error fetching full template:', err);
          this.errorMessage = 'Failed to load the full template details.';
        },
      });
  }

  // Prepares a new template for creation.
  addTemplate(): void {
    this.selectedTemplate = {
      id: '',
      templateTitle: 'New Template',
      description: 'Description for the new template',
      questions: [
        {
          id: -1,
          prompt: 'Default Question',
          allowCustom: true,
          options: [],
        },
      ],
    };
  }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

  // Deletes a template and then refreshes the list.
  deleteTemplate(templateId: string): void {
    this.templateService.deleteTemplate(templateId).subscribe({
      complete: () => {
        this.resetData();
        this.fetchTemplateBases();
      },
      error: () => console.error('Error deleting template'),
    });
  }

  // Saves a new or updated template.
  onSaveTemplate(updatedTemplate: Template): void {
    if (!updatedTemplate.id) {
      // New template.
      updatedTemplate.id = `temp-${Date.now()}`;
      this.templateService.addTemplate(updatedTemplate).subscribe({
        next: (createdTemplate: Template) => {
          console.log('Template added successfully:', createdTemplate);
          this.selectedTemplate = null;
          this.resetData();
          this.fetchTemplateBases();
        },
        error: () => {
          console.error('Error adding template');
          updatedTemplate.id = undefined;
        },
      });
    } else {
      // Update existing template.
      this.templateService.updateTemplate(updatedTemplate.id, updatedTemplate).subscribe({
        complete: () => {
          console.log('Template updated successfully:', updatedTemplate);
          this.selectedTemplate = null;
          this.fetchTemplateBases();
        },
        error: (err) => console.error('Error updating template:', err),
      });
    }
  }

  // Cancels the template editor.
  onCancelEdit(): void {
    this.selectedTemplate = null;
  }
}
