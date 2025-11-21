import { Component, ElementRef, EventEmitter, inject, OnInit, Output, ViewChild } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ActiveService } from '../../services/active.service';
import { User } from '../../../../shared/models/user.model';
import { SearchEntity } from '../../models/searchEntity.model';
import { TemplateBase } from '../../../../shared/models/template.model';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';


// Extend the SearchEntity type for users to include sessionId and hasMore
interface UserSearchEntity<T> extends SearchEntity<T> {
  sessionId?: string;
  hasMore?: boolean;
}

// Extend the Template state to include a queryCursor (not used now, but kept for structure)
interface TemplateSearchEntity extends SearchEntity<TemplateBase> {
  queryCursor?: string;
}

type SearchType = 'student' | 'teacher' | 'template';

@Component({
  selector: 'app-active-questionnaire-builder',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, ModalComponent],
  templateUrl: './active-builder.component.html',
  styleUrls: ['./active-builder.component.css']
})
export class ActiveBuilderComponent implements OnInit {
  private activeService = inject(ActiveService);
  public groupName: string = '';
  public isAnonymousMode = false;
  public groupNameError: string = '';
  public studentError: string = '';
  public teacherError: string = '';
  public templateError: string = '';
  public showStudentResults = false;
  public showTeacherResults = false;
  public showTemplateResults = false;

  public student: UserSearchEntity<User> = {
    selected: [],
    searchInput: '',
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>(),
    sessionId: undefined,
    hasMore: false
  };

  public teacher: UserSearchEntity<User> = {
    selected: [],
    searchInput: '',
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>(),
    sessionId: undefined,
    hasMore: false
  };

  public template: TemplateSearchEntity = {
    selected: [],
    searchInput: '',
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>(),
    queryCursor: undefined
  };

    @ViewChild("studentSearchArea", { static: false })
    studentSearchArea!: ElementRef;
    @ViewChild("teacherSearchArea", { static: false })
    teacherSearchArea!: ElementRef;
    @ViewChild("templateSearchArea", { static: false })
    templateSearchArea!: ElementRef;

  // Set the page size (10 results per search)
  searchAmount = 10;

   // Confirmation modal state
  public showConfirmationModal = false;
  public confirmationTitle = '';
  public confirmationText = '';
  public confirmationConfirmText = '';
  public confirmationCancelText = '';

  @Output() backToListEvent = new EventEmitter<void>();


private handleDocumentClick = (event: MouseEvent) => {
  const studentArea = this.studentSearchArea?.nativeElement;
  const teacherArea  = this.teacherSearchArea?.nativeElement;
  const templateArea = this.templateSearchArea?.nativeElement;

  if (studentArea && !studentArea.contains(event.target as Node)) {
    this.showStudentResults = false;
  }
  if (teacherArea && !teacherArea.contains(event.target as Node)) {
    this.showTeacherResults = false;
  }
  if (templateArea && !templateArea.contains(event.target as Node)) {
    this.showTemplateResults = false;
  }
};

  ngOnInit(): void {
    // Subscribe to debounced search subjects.
    this.student.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.fetch('student', term);
      });

    this.teacher.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.fetch('teacher', term);
      });

    this.template.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.fetch('template', term);
      });

  document.addEventListener("mousedown", this.handleDocumentClick, true);
  }

  ngOnDestroy(): void {
  document.removeEventListener("mousedown", this.handleDocumentClick, true);
  }
  // Returns the proper state based on the entity.
  private getState(entity: SearchType): SearchEntity<any> {
    if (entity === 'student') {
      return this.student;
    } else if (entity === 'teacher') {
      return this.teacher;
    } else if (entity === 'template') {
      return this.template;
    }
    throw new Error(`Unknown entity: ${entity}`);
  }

  // Updated fetch method that always resets the search results and uses a constant page size.
  private fetch(entity: SearchType, term: string): void {
    const state = this.getState(entity);
    if (!term.trim()) return;

    // Always treat this as a new search.
    state.page = 1;
    state.searchResults = [];
    if (entity !== 'template') {
      (state as UserSearchEntity<User>).sessionId = undefined;
    } else {
      (state as TemplateSearchEntity).queryCursor = undefined;
    }
    state.isLoading = true;
    state.errorMessage = null;

    if (entity === 'template') {
      // For templates, pass the fixed searchAmount. (Assumes activeService.searchTemplates is updated accordingly.)
      const templateState = state as TemplateSearchEntity;
      this.activeService.searchTemplates(term, templateState.queryCursor).subscribe({
        next: (response) => {
          templateState.searchResults = response.templateBases;
          // Disable load-more functionality by setting totalPages to 1.
          templateState.totalPages = 1;
          state.isLoading = false;
        },
        error: () => {
          state.errorMessage = 'Failed to load templates.';
          state.isLoading = false;
        }
      });
    } else {
      const userState = state as UserSearchEntity<User>;
      // Use searchAmount (10) as the number of results.
      this.activeService.searchUsers(term, entity, this.searchAmount, userState.sessionId).subscribe({
        next: (response) => {
          const userBases = response.userBases || [];
          userState.searchResults = userBases;
          userState.sessionId = response.sessionId;
          // Remove ability to load more.
          userState.hasMore = false;
          state.isLoading = false;
        },
        error: () => {
          state.errorMessage = `Failed to load ${entity}s.`;
          state.isLoading = false;
        }
      });
    }
  }



  onInputChange(entity: SearchType, value: string): void {
    const state = this.getState(entity);
    state.searchInput = value;
    state.searchSubject.next(value);
  }

  // Add or remove user from selected array
select(entity: SearchType, item: any): void {
  const state = this.getState(entity);
  if (!Array.isArray(state.selected)) {
    state.selected = [];
  }
  
  // ✅ Special handling for teacher: only allow one selection
  if (entity === 'teacher') {
    this.showTeacherResults = false;
    const idx = state.selected.findIndex((u: any) => u.id === item.id);
    if (idx === -1) {
      // Replace the existing teacher with the new one
      state.selected = [item];
    } else {
      // If clicking the same teacher, deselect it
      state.selected = [];
    }
  } else {
    // Normal multi-select behavior for students and templates
    const idx = state.selected.findIndex((u: any) => u.id === item.id);
    if (idx === -1) {
      state.selected.push(item);
    } else {
      state.selected.splice(idx, 1);
    }
    // Clear search input and hide search results
    state.searchInput = '';
    
    // Hide search results dropdown based on entity type
    if (entity === 'student') {
      this.showStudentResults = false;
    } else if (entity === 'template') {
      this.showTemplateResults = false;
      }
    }
  }
  
    clearSelected(entity: SearchType): void {
      const state = this.getState(entity);
      state.selected = [];
    }

     /** Show confirmation modal before creating questionnaire */
  showCreateConfirmation(): void {
    if (this.isAnonymousMode) {
      this.confirmationTitle = 'ACTIVE_BUILDER.CONFIRM_ANONYMOUS_TITLE';
      this.confirmationText = 'ACTIVE_BUILDER.CONFIRM_ANONYMOUS_MESSAGE';
    } else {
      this.confirmationTitle = 'ACTIVE_BUILDER.CONFIRM_EVALUATION_TITLE';
      this.confirmationText = 'ACTIVE_BUILDER.CONFIRM_EVALUATION_MESSAGE';
    }
    this.confirmationConfirmText = 'ACTIVE_BUILDER.CONFIRM_YES';
    this.confirmationCancelText = 'ACTIVE_BUILDER.CONFIRM_NO';
    this.showConfirmationModal = true;
  }

  /** Handle confirmation modal confirm action */
  onConfirmCreate(): void {
    this.showConfirmationModal = false;
    this.createActiveQuestionnaire();
  }

  /** Handle confirmation modal cancel action */
  onCancelCreate(): void {
    this.showConfirmationModal = false;
  }
  
    createActiveQuestionnaire(): void {
    if (this.isAnonymousMode) {
      // Anonymous mode: only participants and template
      if (
        !Array.isArray(this.student.selected) || this.student.selected.length === 0 ||
        !Array.isArray(this.template.selected) || this.template.selected.length === 0 ||
        !this.template.selected[0].id
      ) {
        console.error('Missing required selections for Anonymous Questionnaire.');
        return;
      }
      const payload = {
        participantIds: this.student.selected.map(s => s.id),
        templateId: this.template.selected[0].id
      };
      this.activeService.createAnonymousQuestionnaireGroup(payload).subscribe(() => {
        alert('Anonymt spørgeskema oprettet!');
        this.backToListEvent.emit();
      });
      return;
    }

    this.groupNameError = '';
  this.studentError = '';
  this.teacherError = '';
  this.templateError = '';

  let hasError = false;

  // Normal mode: students, teachers, template, group name
  if (!Array.isArray(this.student.selected) || this.student.selected.length === 0) {
    this.studentError = 'Du skal vælge mindst én elev.';
    hasError = true;
  }
  if (!Array.isArray(this.teacher.selected) || this.teacher.selected.length === 0) {
    this.teacherError = 'Du skal vælge mindst én lærer.';
    hasError = true;
  }
  if (!Array.isArray(this.template.selected) || this.template.selected.length === 0) {
    this.templateError = 'Du skal vælge en skabelon.';
    hasError = true;
  }
  if (!this.template.selected[0].id) {
    this.templateError = 'Den valgte skabelon mangler et ID.';
    hasError = true;
  }
  if (!this.groupName.trim()) {
    this.groupNameError = 'Spørgeskema gruppen skal tildeles et navn.';
    hasError = true;
  }
  if (hasError) {
    return;
  }
  if (this.template.selected.length > 1) {
    alert('Der kan kun tildeles én skabelon ad gangen.');
    return;
    }
    
    const newGroup = {
      name: this.groupName,
      templateId: this.template.selected[0].id,
      studentIds: this.student.selected.map(s => s.id),
      teacherIds: this.teacher.selected.map(t => t.id)
    };
    this.activeService.createActiveQuestionnaireGroup(newGroup).subscribe(() => {
      alert('Spørgeskema-gruppe oprettet!');
      this.backToListEvent.emit();
    });
  }


  onBackToList(): void {
    this.backToListEvent.emit();
  }
}
