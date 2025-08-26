import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ActiveAnonymousBuilderComponent } from '../active-questionnaire-manager/components/active-anonymous-builder/active-anonymous-builder.component';
import { ActiveService } from '../active-questionnaire-manager/services/active.service';
import { User } from '../../shared/models/user.model';
import { SearchEntity } from '../active-questionnaire-manager/models/searchEntity.model';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { TemplateBase } from '../active-questionnaire-manager/models/active.models';

interface UserSearchEntity<T> extends SearchEntity<T> {
  sessionId?: string;
  hasMore?: boolean;
}
interface TemplateSearchEntity extends SearchEntity<TemplateBase> {
  queryCursor?: string;
}

type SearchType = 'student' | 'template';

@Component({
  selector: 'app-data-compare',
  standalone: true,
  imports: [TranslateModule, CommonModule, FormsModule, ActiveAnonymousBuilderComponent],
  templateUrl: './data-compare.component.html',
  styleUrl: './data-compare.component.css'
})
export class DataCompareComponent {
  private activeService = inject(ActiveService);
  public groupName: string = '';
  public isAnonymousMode = false;

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
    } else if (entity === 'template') {
      return this.template;
    }
    throw new Error(`Unknown entity: ${entity}`);
  }



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
  
  onInputChange(entity: SearchType, value: string): void {
    const state = this.getState(entity);
    state.searchInput = value;
    state.searchSubject.next(value);
  }






  onBackToList(): void {
  this.backToListEvent.emit();
  }
}



  
