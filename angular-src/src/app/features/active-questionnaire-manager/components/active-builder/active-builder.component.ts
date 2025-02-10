import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { User } from '../../../../shared/models/user.model';
import { Template } from '../../../template-manager/models/template.model';
import { PaginationResponse } from '../../../../shared/models/Pagination.model';
import { SearchEntity } from '../../models/searchEntity.model';

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

  public student: SearchEntity<User> = {
    selected: null,
    searchInput: '',
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>()
  };

  public teacher: SearchEntity<User> = {
    selected: null,
    searchInput: '',
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>()
  };

  public template: SearchEntity<Template> = {
    selected: null,
    searchInput: '',
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>()
  };

  @Output() backToListEvent = new EventEmitter<void>();

  ngOnInit(): void {
    // Subscribe to the debounced search subjects for each entity.
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

  // Helper Method that gets object based on string
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

  private fetch(entity: SearchType, term: string, isLoadMore: boolean = false): void {
    // Gets a specic search state
    const state = this.getState(entity);
    if (!term.trim()) return;
    if (!isLoadMore) {
      state.page = 1;
      state.searchResults = [];
    }
    state.isLoading = true;
    state.errorMessage = null;

    if (entity === 'template') {
      this.activeService.searchTemplates(term, state.page).subscribe({
        next: (response: PaginationResponse<Template>) => {
          //
          state.searchResults = isLoadMore
            ? [...state.searchResults, ...response.items]
            : response.items;
          state.totalPages = response.totalPages;
          state.isLoading = false;
        },
        error: () => {
          state.errorMessage = 'Failed to load templates.';
          state.isLoading = false;
        }
      });
    } else {
      this.activeService.searchUsers(term, entity, state.page).subscribe({
        next: (response: PaginationResponse<User>) => {
          state.searchResults = isLoadMore
            ? [...state.searchResults, ...response.items]
            : response.items;
          state.totalPages = response.totalPages;
          state.isLoading = false;
        },
        error: () => {
          state.errorMessage = `Failed to load ${entity}s.`;
          state.isLoading = false;
        }
      });
    }
  }

  loadMore(entity: SearchType): void {
    const state = this.getState(entity);
    if (state.page < state.totalPages) {
      state.page++;
      this.fetch(entity, state.searchInput, true);
    }
  }

  onInputChange(entity: SearchType, value: string): void {
    const state = this.getState(entity);
    state.searchInput = value;
    state.searchSubject.next(value);
  }

  select(entity: SearchType, item: any): void {
    const state = this.getState(entity);
    state.selected = item;
    state.searchInput = ''; // Clear the input field
    state.searchResults = []; // Reset the search results
  }

  clearSelected(entity: SearchType): void {
    const state = this.getState(entity);
    state.selected = null;
  }
  createActiveQuestionnaire(): void {
    // Ensure everything that needs to be has been selected.
    if (!this.student.selected || !this.teacher.selected || !this.template.selected) return;

    const newQuestionnaire = {
      studentId: this.student.selected.id,
      teacherId: this.teacher.selected.id,
      templateId: this.template.selected.id,
    };

    this.activeService.createActiveQuestionnaire(newQuestionnaire).subscribe(() => {
      alert('Active Questionnaire Created Successfully!');
      this.backToListEvent.emit();
    });
  }

  onBackToList(): void {
    this.backToListEvent.emit();
  }
}
