import { Component, inject } from '@angular/core';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { Template } from './models/template.model';
import { TemplateService } from './services/template.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-template-manager',
  standalone: true,
  imports: [TemplateEditorComponent, FormsModule, CommonModule],
  templateUrl: './template-manager.component.html',
  styleUrls: ['./template-manager.component.css'],
})
export class TemplateManagerComponent {
  private templateService = inject(TemplateService);

  templates: Template[] = [];
  searchTerm: string = '';
  searchType: string = 'name'; // Default search type
  currentPage: number = 1;
  pageSize: number = 5; // Number of items per page
  totalItems: number = 0; // Total number of templates
  hasNextPage: boolean = false;
  hasPreviousPage: boolean = false;
  selectedTemplate: Template | null = null;
  private searchSubject = new Subject<string>();

  ngOnInit(): void {
    // Initialize debounce for search input
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.currentPage = 1; // Reset to the first page on new search
        this.fetchTemplates();
      });

    // Initial fetch
    this.fetchTemplates();
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  onSearchTypeChange(type: string): void {
    this.searchType = type;
    this.currentPage = 1;
    this.fetchTemplates();
  }

  private fetchTemplates(): void {
    this.templateService
      .getTemplates(this.currentPage, this.pageSize, this.searchTerm, this.searchType)
      .subscribe({
        next: (response) => {
          this.templates = response.items;
          this.totalItems = response.totalItems;
          this.hasNextPage = this.currentPage < this.totalPages;
          this.hasPreviousPage = this.currentPage > 1;
        },
        error: () => {
          console.error('Error fetching templates');
        },
      });
  }

  refresh(): void {
    this.searchTerm = '';
    this.currentPage = 1;
    this.fetchTemplates();
  }

  addTemplate(): void {
    const newTemplate: Template = {
      id: '', // The ID will be assigned by the service or backend
      title: 'New Template',
      description: 'Description for the new template',
      questions: [],
    };
  
    this.templateService.addTemplate(newTemplate).subscribe({
      next: (createdTemplate: Template) => {
        this.selectedTemplate = createdTemplate; // Automatically select the new template
      },
      error: () => console.error('Error adding template'),
    });
  }

  deleteTemplate(templateId: string): void {
    this.templateService.deleteTemplate(templateId).subscribe({
      complete: () => this.fetchTemplates(),
      error: () => console.error('Error deleting template'),
    });
  }

  previousPage(): void {
    if (this.hasPreviousPage) {
      this.currentPage--;
      this.fetchTemplates();
    }
  }

  jumpToPage(page: string): void {
    const pageNumber = +page;
    if (pageNumber !== this.currentPage && pageNumber > 0 && pageNumber <= this.totalPages) {
      this.currentPage = pageNumber;
      this.fetchTemplates();
    }
  }

  nextPage(): void {
    if (this.hasNextPage) {
      this.currentPage++;
      this.fetchTemplates();
    }
  }

  selectTemplate(templateId: string): void {
    const template = this.templates.find(t => t.id === templateId);
    if (template) {
      this.selectedTemplate = { ...template }; // Clone the template to avoid directly modifying the original
    }
  }


  // Select a template for editing
  editTemplate(template: Template): void {
    this.selectedTemplate = { ...template }; // Create a copy to avoid modifying the original
  }

  onSaveTemplate(updatedTemplate: Template): void {
    this.templateService.updateTemplate(updatedTemplate.id, updatedTemplate).subscribe({
      complete: () => {
        console.log('Template updated successfully:', updatedTemplate);
        this.fetchTemplates(); // Refresh the template list to reflect changes
        this.selectedTemplate = null; // Close the editor
      },
      error: (err) => {
        console.error('Error updating template:', err);
      },
    });
  }
  

  // Cancel editing
  onCancelEdit(): void {
    this.selectedTemplate = null;
  }

}
