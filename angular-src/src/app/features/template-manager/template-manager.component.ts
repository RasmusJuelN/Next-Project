import { Component, inject } from '@angular/core';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { Template } from './models/template.model';
import { TemplateService } from './services/template.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { finalize } from 'rxjs/operators';
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

  templates: Template[] = [];
  searchTerm: string = '';
  searchType: "name" |"id" = 'name'; // Default search type
  currentPage: number = 1;
  pageSize: number = 5; // Items per page
  totalItems: number = 0; // Total number of items (from API)
  totalPages: number = 0; // Total pages (from API)
  pageSizeOptions: number[] = [5, 10, 15, 20];

  private searchSubject = new Subject<string>();
  isLoading = false;
  errorMessage: string | null = null;
  selectedTemplate: Template | null = null;

  ngOnInit(): void {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.currentPage = 1;
        this.fetchTemplates();
      });

    this.fetchTemplates();
  }

  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.currentPage = 1;
    this.fetchTemplates();
  }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

  onSearchTypeChange(type: string): void {
    if (type === "name" || type === "id") {
      this.searchType = type;
      this.currentPage = 1;
      this.fetchTemplates();
    }
  }

  // Fetch templates from the service and update pagination state
  private fetchTemplates(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.templateService
      .getTemplates(this.currentPage, this.pageSize, this.searchTerm, this.searchType)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (response) => {
          this.templates = response.items;
          this.totalItems = response.totalItems;
          this.totalPages = response.totalPages;
        },
        error: (err) => {
          console.error('Error fetching templates:', err);
          this.errorMessage = 'Failed to load templates. Please try again.';
        },
      });
  }

  addTemplate(): void {
    const newTemplate: Template = {
      id: '', // Temporary ID for new template
      title: 'New Template',
      description: 'Description for the new template',
      questions: [
        {
          id: -1, // Temporary negative ID
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
      complete: () => this.fetchTemplates(),
      error: () => console.error('Error deleting template'),
    });
  }

  // Called when the PaginationComponent emits a page change event
  handlePageChange(newPage: number): void {
    this.currentPage = newPage;
    this.fetchTemplates();
  }

  selectTemplate(templateId: string): void {
    const template = this.templates.find((t) => t.id === templateId);
    if (template) {
      // Clone the template to avoid direct mutation
      this.selectedTemplate = { ...template };
    }
  }

  editTemplate(template: Template): void {
    // Create a copy to avoid modifying the original until save
    this.selectedTemplate = { ...template };
  }

  onSaveTemplate(updatedTemplate: Template): void {
    if (!updatedTemplate.id) {
      // New template: assign a temporary ID and add via service
      updatedTemplate.id = `temp-${Date.now()}`;
      this.templateService.addTemplate(updatedTemplate).subscribe({
        next: (createdTemplate: Template) => {
          console.log('Template added successfully:', createdTemplate);
          this.selectedTemplate = null;
          this.fetchTemplates();
        },
        error: () => console.error('Error adding template'),
      });
    } else {
      // Existing template: update via service
      this.templateService.updateTemplate(updatedTemplate.id, updatedTemplate).subscribe({
        complete: () => {
          console.log('Template updated successfully:', updatedTemplate);
          this.fetchTemplates();
          this.selectedTemplate = null;
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
