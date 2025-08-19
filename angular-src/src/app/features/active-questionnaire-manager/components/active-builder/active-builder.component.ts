import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { User } from '../../../../shared/models/user.model';
import { SearchEntity } from '../../models/searchEntity.model';
import { Template, TemplateBase } from '../../models/active.models';

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
  imports: [CommonModule, FormsModule],
  templateUrl: './active-builder.component.html',
  styleUrls: ['./active-builder.component.css']
})
export class ActiveBuilderComponent implements OnInit {
  private activeService = inject(ActiveService);
  public groupName: string = '';
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

  // Set the page size (10 results per search)
  searchAmount = 10;

  @Output() backToListEvent = new EventEmitter<void>();

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
    const idx = state.selected.findIndex((u: any) => u.id === item.id);
    if (idx === -1) {
      state.selected.push(item);
    } else {
      state.selected.splice(idx, 1);
    }
    // Do NOT clear search results so user can select multiple
    state.searchInput = '';
    // state.searchResults = [];
  }

  clearSelected(entity: SearchType): void {
    const state = this.getState(entity);
    state.selected = [];
  }

  createActiveQuestionnaire(): void {
    if (
      !Array.isArray(this.student.selected) || this.student.selected.length === 0 ||
      !Array.isArray(this.teacher.selected) || this.teacher.selected.length === 0 ||
      !Array.isArray(this.template.selected) || this.template.selected.length === 0 ||
      !this.template.selected[0].id ||
      !this.groupName.trim()
    ) {
      console.error('Missing required selections for Active Questionnaire.');
      return;
    }
    
    // Ensure only one template is selected
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
    // const newQuestionnaire = {
    //   studentIds: this.student.selected.map(s => s.id),
    //   teacherIds: this.teacher.selected.map(t => t.id),
    //   templateId: this.template.selected[0].id,
    // };

    // this.activeService.createActiveQuestionnaire(newQuestionnaire).subscribe(() => {
    //   alert('Aktive spørgeskemaer oprettet!');
    //   this.backToListEvent.emit();
    // });
  }

  onBackToList(): void {
    this.backToListEvent.emit();
  }
}
