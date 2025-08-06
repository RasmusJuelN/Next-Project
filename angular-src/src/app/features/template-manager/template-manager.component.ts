import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { TemplateBase, Template } from './models/template.model';
import { TemplateService } from './services/template.service';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { PaginationComponent, PageChangeEvent } from '../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../shared/loading/loading.component';

@Component({
    selector: 'app-template-manager',
    imports: [
    TemplateEditorComponent,
    FormsModule,
    PaginationComponent,
    LoadingComponent
],
    templateUrl: './template-manager.component.html',
    styleUrls: ['./template-manager.component.css']
})
export class TemplateManagerComponent {
  private templateService = inject(TemplateService);

  templateBases: TemplateBase[] = [];
  cachedCursors: { [pageNumber: number]: string | null } = {};
  selectedTemplate: Template | null = null;
  
  // New property to control locked modal visibility
  showLockedModal: boolean = false;

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

  // Deletion confirmation states.
  templateToDelete: string | null = null;
  deleteConfirmationStep: number = 0; // 0 for first message, 1 for final warning

  ngOnInit(): void {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.resetData();
        this.fetchTemplateBases();
      });
    this.fetchTemplateBases();
  }

  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.resetData();
    this.fetchTemplateBases();
  }

  onSearchTypeChange(type: string): void {
    if (type === 'name' || type === 'id') {
      this.searchType = type;
      this.resetData();
      this.fetchTemplateBases();
    }
  }

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
            this.totalPages = this.currentPage;
            this.cachedCursors[this.currentPage + 1] = null;
          } else if (response.queryCursor) {
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

  handlePageChange(event: PageChangeEvent): void {
    const newPage = event.page;
    if (event.direction === 'forward' && newPage > this.currentPage && !this.cachedCursors[newPage]) {
      console.warn('Page not available yet. Fetching...');
      this.currentPage = newPage;
      this.fetchTemplateBases();
      return;
    }
    this.currentPage = newPage;
    this.fetchTemplateBases();
  }

  // Update selectTemplate to check if the template is locked
  selectTemplate(templateBaseId: string): void {
    this.selectedTemplate = null;
    this.isLoading = true;
    this.templateService
      .getTemplateDetails(templateBaseId)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (fullTemplate) => {
          if (fullTemplate.isLocked) {
            // Show modal if the template is locked
            this.showLockedModal = true;
          } else {
            this.selectedTemplate = fullTemplate;
          }
        },
        error: (err) => {
          console.error('Error fetching full template:', err);
          this.errorMessage = 'Failed to load the full template details.';
        },
      });
  }

  addTemplate(): void {
    this.selectedTemplate = {
      id: '',
      title: 'New Template',
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

  // Opens the modal and resets the confirmation step.
  openDeleteModal(templateId: string): void {
    this.templateToDelete = templateId;
    this.deleteConfirmationStep = 0;
  }

  // Handles confirm click: if first step, update text; if already at final warning, delete.
  confirmDelete(): void {
    if (this.deleteConfirmationStep === 0) {
      this.deleteConfirmationStep = 1;
    } else {
      if (this.templateToDelete) {
        this.templateService.deleteTemplate(this.templateToDelete).subscribe({
          complete: () => {
            this.cancelDelete();
            this.resetData();
            this.fetchTemplateBases();
          },
          error: () => console.error('Error deleting template'),
        });
      }
    }
  }

  cancelDelete(): void {
    this.templateToDelete = null;
    this.deleteConfirmationStep = 0;
  }

  onSaveTemplate(updatedTemplate: Template): void {
    if (!updatedTemplate.id) {
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

  onCancelEdit(): void {
    this.selectedTemplate = null;
  }

  // New method to close the locked modal.
  closeLockedModal(): void {
    this.showLockedModal = false;
  }
}
