import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, forkJoin, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { User } from '../../../../shared/models/user.model';
import { SearchEntity } from '../../models/searchEntity.model';
import { TemplateBase } from '../../../../shared/models/template.model';
import { Student } from '../../models/active.models';


// Extend the SearchEntity type for users to include sessionId and hasMore
interface UserSearchEntity<T> extends SearchEntity<T> {
  sessionId?: string;
  hasMore?: boolean;
}

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
  public groupSearchInput: string = '';
  public isAnonymousMode = false;

  public groups: { name: string }[] = [];
  

   public studentsByGroup: { [groupName: string]: Student[] } = {};
    public allStudentsFlat: Student[] = [];


  public showStudentsFor: string | null = null; // for collapse/expand
  public searchedGroupName: string | null = null; 
  
  
  
 public student: UserSearchEntity<Student> &
  { searchType?: 'name' | 'class', searchByName: string, searchByClass: string } = {
  selected: [],
  searchInput: '',
  searchResults: [],
  page: 1,
  totalPages: 1,
  isLoading: false,
  errorMessage: null,
  searchSubject: new Subject<string>(),
  sessionId: undefined,
  hasMore: false,
  searchType: 'name',
  searchByName: '',
  searchByClass: ''
};

  public groupNameError: string = '';
  public studentError: string = '';
  public teacherError: string = '';
  public templateError: string = '';


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
    this.student.searchSubject.pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(term => this.fetch('student', term));

    this.teacher.searchSubject.pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(term => this.fetch('teacher', term));

    this.template.searchSubject.pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(term => this.fetch('template', term));

  

    this.activeService.getClasses().subscribe(classes => {
       this.groups = classes.map(c => ({ name: c }));

        const observables = this.groups.map(group =>
      this.activeService.getStudentsInGroup(group.name)
    );

    forkJoin(observables).subscribe(responses => {
      responses.forEach((response: any, index: number) => {
        const groupName = this.groups[index].name;
        const classData = (response || []).find(
          (c: any) => (c.className || '').toLowerCase() === groupName.toLowerCase()
        );

        if (!classData) return;

        const mappedStudents: Student[] = (classData.students || []).map((s: any) => ({
          id: s.id || s.userId || s.userName || s.name,
          name: typeof s === 'string' ? s : s.fullName || s.userName || s.name || '',
          className: classData.className
        }));

        this.studentsByGroup[groupName] = mappedStudents;

        this.allStudentsFlat = [
          ...this.allStudentsFlat,
          ...mappedStudents.filter(ms => !this.allStudentsFlat.find(a => a.id === ms.id))
        ];
      });
    });
  });
}

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


  private fetch(entity: SearchType, term: string): void {
  const state = this.getState(entity);
  if (!term.trim()) {
    state.searchResults = [];
    state.isLoading = false;
    return;
  }

  state.isLoading = true;
  state.errorMessage = null;


   if (entity === 'student') {
  const searchTerm = term.toLowerCase();

  state.searchResults = this.allStudentsFlat
    .filter(s =>
      (s.name?.toLowerCase().includes(searchTerm)) ||
      (s.className?.toLowerCase().includes(searchTerm)) ||
      (s.id?.toLowerCase().includes(searchTerm))
    )
    .sort((a, b) => (a.name || '').localeCompare(b.name || ''));

  state.isLoading = false;  
  return;
}

    if (entity === 'teacher') {
      const userState = state as UserSearchEntity<User>;
      this.activeService.searchUsers(term, entity, this.searchAmount, userState.sessionId).subscribe({
        next: response => {
          userState.searchResults = response.userBases || [];
          userState.sessionId = response.sessionId;
          userState.hasMore = false;
          state.isLoading = false;
        },
        error: () => {
          state.errorMessage = `Failed to load ${entity}s.`;
          state.isLoading = false;
        }
      });
    }

    if (entity === 'template') {
      const templateState = state as TemplateSearchEntity;
      this.activeService.searchTemplates(term, templateState.queryCursor).subscribe({
        next: response => {
          templateState.searchResults = response.templateBases;
          templateState.totalPages = 1;
          state.isLoading = false;
        },
        error: () => {
          state.errorMessage = 'Failed to load templates.';
          state.isLoading = false;
        }
      });
    }
}


onInputChange(
  entity: SearchType,
  value: string,
  type?: 'name' | 'class'
): void {
  const state = this.getState(entity);
  state.searchInput = value;

  // Only set searchType for students
  if (entity === 'student' && type) {
    (state as UserSearchEntity<Student> & { searchType?: 'name' | 'class' }).searchType = type;
  }

  state.searchSubject.next(value);
}


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
    state.searchInput = '';
  }

  clearSelected(entity: SearchType): void {
    const state = this.getState(entity);
    state.selected = [];
  }

  createActiveQuestionnaire(): void {
    if (this.isAnonymousMode) {
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


loadStudentsForGroup(groupName: string) {
  this.activeService.getStudentsInGroup(groupName).subscribe({
    next: response => {
      const classData = (response || []).find(
        (c: any) => (c.className || '').toLowerCase() === groupName.toLowerCase()
      );

      if (!classData) {
        this.studentsByGroup[groupName] = [];
        return;
      }

      // Map students
      const mappedStudents: Student[] = (classData.students || []).map((s: any) => ({
        id: s.id || s.userId || s.userName || s.name, // unique ID
        name: typeof s === 'string' ? s : s.fullName || s.userName || s.name || '',
        className: classData.className
      }));
;


      this.studentsByGroup[groupName] = mappedStudents;

      // Add to flat list if not already there
      this.allStudentsFlat = [
        ...this.allStudentsFlat,
        ...mappedStudents.filter(ms => !this.allStudentsFlat.find(a => a.id === ms.id))
      ];
    },
    error: err => {
      console.error('Failed to load students for group:', groupName, err);
      this.studentsByGroup[groupName] = [];
    }
  });
}

  onGroupClick(groupName: string) {
    this.showStudentsFor = this.showStudentsFor === groupName ? null : groupName;
  }

  searchGroup() {
  const input = this.groupSearchInput.trim();
  if (!input) {
    this.searchedGroupName = null;
    return;
  }

  this.loadStudentsForGroup(input);
  this.searchedGroupName = input; // keep the same casing
}


// Filter students by name
get filteredStudentsByName(): Student[] {
  const term = (this.student.searchByName || '').trim().toLowerCase();
  if (!term) return [];
  return Object.values(this.studentsByGroup)
    .flat()
    .filter(s => s.name?.toLowerCase().includes(term))
    .sort((a,b)=> (a.name || '').localeCompare(b.name || ''));
}

// Filter students by class
get filteredGroupsByClass(): { name: string; students: Student[] }[] {
  const term = (this.student.searchByClass || '').trim().toLowerCase();
  if (!term) return [];
  return Object.keys(this.studentsByGroup)
    .map(groupName => ({ name: groupName, students: this.studentsByGroup[groupName] || [] }))
    .filter(group => group.students.some(s => s.className?.toLowerCase().includes(term)));
}

// Track multiple expanded groups
public expandedGroups: Set<string> = new Set();

toggleGroupExpansion(groupName: string) {
  if (this.expandedGroups.has(groupName)) {
    this.expandedGroups.delete(groupName);
  } else {
    this.expandedGroups.add(groupName);
  }
}







}
