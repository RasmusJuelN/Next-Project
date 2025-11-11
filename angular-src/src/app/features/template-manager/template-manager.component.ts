import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { TemplateService } from './services/template.service';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { PaginationComponent, PageChangeEvent } from '../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../shared/loading/loading.component';
import { Template, TemplateBase, TemplateStatus } from '../../shared/models/template.model';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ModalComponent } from '../../shared/components/modal/modal.component';

/** Modal dialog purposes supported by this page. */
enum TemplateModalType {
  None = 'none',
  Delete = 'delete',
  Copy = 'copy'
}

/**
 * Template manager component.
 *
 * Provides an interface for questionnaire templates.
 *
 * Handles:
 * - Listing and searching templates (cursor pagination).
 * - Creating, editing, finalizing, copying, and deleting.
 * - Two-step delete confirmation via modal.
 *
 * Notes:
 * - Caches cursors by page to reduce refetching.
 * - Debounces search input (300 ms) to minimize API calls.
 * - Uses translation keys for all labels and default values.
 */
@Component({
    selector: 'app-template-manager',
    imports: [
        TemplateEditorComponent,
        FormsModule,
        CommonModule,
        PaginationComponent,
        LoadingComponent,
        TranslateModule,
        ModalComponent
    ],
    templateUrl: './template-manager.component.html',
    styleUrls: ['./template-manager.component.css']
})
export class TemplateManagerComponent {
  private templateService = inject(TemplateService);
  private translate = inject(TranslateService);

  templateBases: TemplateBase[] = [];
  cachedCursors: { [pageNumber: number]: string | null } = {};
  selectedTemplate: Template | null = null;

  templateStatus = TemplateStatus
  

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
  TemplateModalType = TemplateModalType;

  lockedTitle = 'Skabelonen er udgivet';
  lockedText =
    'Denne skabelon er i skrivebeskyttet tilstand.<br />' +
    'Vælg <strong>Kopiér</strong> i listen, hvis du vil lave ændringer på en ' +
    'redigerbar version.';


  activeModalType: TemplateModalType = TemplateModalType.None;
  activeModalTemplateId: string | null = null;
  deleteConfirmStep = 0;

 /**
 * Wire up debounced search and load the first page.
 * Debounce prevents excessive calls while typing; distinctUntilChanged
 * avoids re-querying the same term.
 */
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

  /** Push new search terms into the debounced pipeline. */
  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }
  /**
   * Change items-per-page and fetch the first page with the new size.
   */
  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.resetData();
    this.fetchTemplateBases();
  }

  /** Toggle server-side search type ('name'|'id'), */
  onSearchTypeChange(type: string): void {
    if (type === 'name' || type === 'id') {
      this.searchType = type;
      this.resetData();
      this.fetchTemplateBases();
    }
  }

 /**
 * Reset pagination state and (optionally) search parameters.
 * Also clears the currently selected template/editor view.
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

  /** Fetch a page of template bases. */
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
          this.errorMessage = 'TMP_FAIL_LOAD_LIST';
        },
      });
  }

/**
 * Handles next/prev button clicks from PaginationComponent.
 * If moving forward without a known cursor, performs a fetch to acquire it.
 */
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
  /**
   * takes the id of an template makes it the selected template for editor panel
   * Sets `isLoading` to cover the transition to the editor.
   * @param id templateId
   */
selectTemplate(id: string): void {
  this.selectedTemplate = null;
  this.isLoading = true;
  this.templateService.getTemplateDetails(id)
    .pipe(finalize(() => (this.isLoading = false)))
    .subscribe({
      next: tmpl => {
        this.selectedTemplate = tmpl;
      },
      error: err => {
        console.error('Error fetching template:', err);
        this.errorMessage = 'TMP_FAIL_LOAD_LIST';
      }
    });
}

/**
 * Promote a draft template to Finalized on the server,
 * then clear the editor and refresh the list.
 */
onFinalizeTemplate(tmpl: Template): void {
  if (tmpl.id){
  this.templateService.upgradeTemplate(tmpl.id).subscribe({
    next: () => {
      this.selectedTemplate = null;   // close editor
      this.fetchTemplateBases();      // refresh list
    },
    error: err => console.error('Upgrade failed', err)
  });
  }
}

/**
 * Create a local draft with translated defaults
 * and open it in the editor (not persisted until Save).
 */
  addTemplate(): void {
    this.selectedTemplate = {
      id: '',
      templateStatus: TemplateStatus.Draft,
       title: this.translate.instant('TMP_NEW_TITLE'), //  New Template
       description: this.translate.instant('TMP_NEW_DESC'), //Description for the new template
      questions: [
        {
          id: -1,
          prompt: this.translate.instant('TMP_NEW_QUESTION'), //Default Question
          allowCustom: true,
          options: [],
        },
      ],
    };
  }
  // addTemplate(): void {
  //   this.selectedTemplate = {
  //     id: '',
  //     title: ' New Template', //  
  //     description: 'Description for the new template', //
  //     questions: [
  //       {
  //         id: -1,
  //         prompt: 'Default Question', //Default Question
  //         allowCustom: true,
  //         options: [],
  //       },
  //     ],
  //   };
  // }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

openDeleteModal(templateId: string): void {
  this.showModal(TemplateModalType.Delete, templateId);
}

/**
 * updates the current editor state.
 * - If no id: creates a new template then resets list & closes editor.
 * - If id exists: updates the template and refreshes the list.
 */
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

/** Open a modal of a given type; seed it with a specific id or selected template id. */
showModal(type: TemplateModalType, id?: string | null) {
  this.activeModalType = type;
  this.activeModalTemplateId = id ?? this.selectedTemplate?.id ?? null;
  if (type === TemplateModalType.Delete) this.deleteConfirmStep = 0;
}

/** Close and reset modal state. */
hideModal() {
  this.activeModalType = TemplateModalType.None;
  this.activeModalTemplateId = null;
  this.deleteConfirmStep = 0;
}

/** True when any modal is visible (used by <app-modal>). */
get isModalOpen(): boolean { 
  return this.activeModalType !== TemplateModalType.None; 
}

/** Title text resolved from i18n depending on modal type/step. */
get modalTitle(): string {
  switch (this.activeModalType) {
    case TemplateModalType.Delete:
      return this.deleteConfirmStep === 0
        ? this.translate.instant('TMP_DELETE_CONFIRM_TITLE')
        : this.translate.instant('TMP_DELETE_CONFIRM_WARN');
    case TemplateModalType.Copy:
      return this.translate.instant('TMP_LOCKED_TITLE');
    default:
      return '';
  }
}
/** Body text resolved from i18n depending on modal type/step. */
get modalText(): string {
  switch (this.activeModalType) {
    case TemplateModalType.Delete:
      return this.deleteConfirmStep === 0
        ? this.translate.instant('TEMPLATES.DELETE.MSG')
        : this.translate.instant('TMP_DELETE_FINAL_WARN_MSG');
    case TemplateModalType.Copy:
      return this.translate.instant('TMP_LOCKED_MSG');
    default:
      return '';
  }
}

/** Confirm button label, varies per type/step. */
get confirmText(): string {
  switch (this.activeModalType) {
    case TemplateModalType.Delete:
      return this.deleteConfirmStep === 0
        ? this.translate.instant('COMMON.BUTTONS.CONTINUE')
        : this.translate.instant('TMP_DELETE');
    case TemplateModalType.Copy:
      return this.translate.instant('COMMON.BUTTONS.COPY');
    default:
      return this.translate.instant('COMMON.BUTTONS.CLOSE');
  }
}

/** Cancel/Close button label, varies per type/step. */
get cancelText(): string {
  switch (this.activeModalType) {
    case TemplateModalType.Delete:
      return this.translate.instant('COMMON.BUTTONS.CANCEL');
    case TemplateModalType.Copy:
    default:
      return this.translate.instant('COMMON.BUTTONS.CLOSE');       // already in your JSON
  }
}

/**
 * confirm handler:
 * - Delete: two-step confirm; on final confirm calls delete API.
 * - Copy: loads source template, deep-copies it into a new local draft and opens editor.
 */
onModalConfirm(): void {
  const id = this.activeModalTemplateId ?? undefined;
  if (!id) return;

  if (this.activeModalType === TemplateModalType.Delete) {
    if (this.deleteConfirmStep === 0) {
      this.deleteConfirmStep = 1;
      return;
    }
    this.templateService.deleteTemplate(id).subscribe({
      complete: () => { this.hideModal(); this.resetData(); this.fetchTemplateBases(); },
      error: () => console.error('Error deleting template')
    });
    return;
  }

  if (this.activeModalType === TemplateModalType.Copy) {
    // fetch the source template, then deep-copy it into a brand new local draft
    this.templateService.getTemplateDetails(id).subscribe({
      next: tmplSrc => {
        const draftCopy = this.deepCopyAsNewTemplate(tmplSrc);
        this.selectedTemplate = draftCopy;   // open the new local draft immediately
        // no server refresh needed yet; user will Save to persist
        this.hideModal();
      },
      error: err => console.error('Error loading template to copy', err)
    });
  }
}

/**
 * cancel handler:
 * - For delete step 1, step back to initial confirm.
 * - Otherwise close modal.
 */
onModalCancel(): void {
  if (this.activeModalType === TemplateModalType.Delete && this.deleteConfirmStep === 1) {
    this.deleteConfirmStep = 0;
    return; // keep the modal open, just step back
  }
  this.hideModal();
}

/**
 * Create a deep-cloned editable draft from an existing template:
 * - Clears server metadata, sets Draft status, unlocks.
 * - Assigns temporary ids to template/questions/options.
 */
private deepCopyAsNewTemplate(template: Template): Template {
  // Deep clone to avoid mutating the original
  const clone: Template = JSON.parse(JSON.stringify(template));

  // If it has an ID, replace with a unique negative ID
  // If it has no ID, leave it undefined
  clone.id = clone.id ? `temp-${Date.now()}` : undefined;

  // Reset meta fields
  clone.createdAt = undefined;
  clone.lastUpdated = undefined;
  clone.templateStatus = TemplateStatus.Draft;
  clone.isLocked = false;
  clone.title = `${clone.title} (kopi)`
  clone.id = "";

  // Assign fresh negative IDs to questions & options
  clone.questions = clone.questions.map((q, qIndex) => ({
    ...q,
    id: -1 * (qIndex + 1), // new negative ID
    options: q.options.map((o, oIndex) => ({
      ...o,
      id: -1 * (oIndex + 1) // new negative ID
    }))
  }));

  return clone;
}
}
