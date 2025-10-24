import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, OnDestroy, OnInit, Output, ViewChild, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { User, Role } from '../../shared/models/user.model';
import { TemplateBase } from '../active-questionnaire-manager/models/active.models';
import { ShowResultComponent, ShowResultConfig } from '../../shared/show-result/show-result.component';
import { Result } from '../result/models/result.model';
import { ResultHistoryService } from './services/result-history.service';


enum SearchEnum 
{
  Student = 'student',
  Template = 'template'
}

interface SearchState<T> {
  selected: T | null;
  query: string;
  results: T[];
  loading: boolean;
  error: string | null;
  cursor?: string;      // reuse for sessionId/queryCursor
  hasMore?: boolean;
  input$: Subject<string>;
}


@Component({
  selector: 'app-result-history',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, ShowResultComponent],
  templateUrl: './result-history.component.html',
  styleUrls: ['./result-history.component.css']
})
export class ResultHistoryComponent implements OnInit {
  private resultHistoryService = inject(ResultHistoryService);

  public searchEnum = SearchEnum;
  public student = this.createSearchState<User>();
  public template = this.createSearchState<TemplateBase>();

  public showDropdown = { [SearchEnum.Student]: false, [SearchEnum.Template]: false };

  public results: Result[] = [];
  public currentResultIndex = 0;
  public isLoading = false;
  public errorMessage: string | null = null;

  /** Utility to create consistent search state */
  private createSearchState<T>(): SearchState<T> {
    return {
      selected: null,
      query: '',
      results: [],
      loading: false,
      error: null,
      cursor: undefined,
      hasMore: false,
      input$: new Subject<string>()
    };
  }

  public resultConfig: ShowResultConfig = {
    showTemplate: true,
    showStudent: true,
    showTeacher: false,
    showCompletionDates: true,
    useCardStyling: true,
    showActions: false
  };

  searchAmount = 10;

  @Output() backToListEvent = new EventEmitter<void>();

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const el = event.target as HTMLElement;
    for (const key in this.showDropdown) {
      this.showDropdown[key as SearchEnum] = !!el.closest(`[data-search="${key}"]`);
    }
  }

  private getState<T>(type: SearchEnum): SearchState<T> {
    return type === SearchEnum.Student ? (this.student as any) : (this.template as any);
  }

  ngOnInit(): void {
    this.setupSearch(this.student, SearchEnum.Student);
    this.setupSearch(this.template, SearchEnum.Template);
    this.loadMockData();
  }
  private setupSearch<T>(state: SearchState<T>, type: SearchEnum): void {
    state.input$.pipe(debounceTime(300), distinctUntilChanged()).subscribe((term) => this.fetch(type, term));
  }
  private fetch(entity: SearchEnum, term: string): void {
    const state = entity === SearchEnum.Student ? this.student : this.template;
    if (!term.trim()) return;

    state.loading = true;
    state.error = null;
    state.results = [];

    if (entity === SearchEnum.Template) {
      this.resultHistoryService.searchTemplates(term, state.cursor).subscribe({
        next: (response) => {
          state.results = response.templateBases;
          state.loading = false;
        },
        error: () => {
          state.error = 'Failed to load templates.';
          state.loading = false;
        }
      });
    } else {
      this.resultHistoryService.searchUsers(term, 'student', this.searchAmount, state.cursor).subscribe({
        next: (response) => {
          state.results = response.userBases || [];
          state.cursor = response.sessionId;
          state.loading = false;
        },
        error: () => {
          state.error = 'Failed to load students.';
          state.loading = false;
        }
      });
    }
  }

  select(type: SearchEnum, item: any): void {
    const state = this.getState(type);
    state.selected = item;
    state.query = '';
    this.showDropdown[type] = false;

    if (this.student.selected && this.template.selected) this.fetchStudentResults();
  }
  clearSelected(type: SearchEnum): void {
    this.getState(type).selected = null;
    this.results = [];
    this.currentResultIndex = 0;
  }

  onInputChange(type: SearchEnum, value: string): void {
    const state = this.getState(type);
    state.query = value;
    state.input$.next(value);
    this.showDropdown[type] = true;
  }
  onBackToList(): void {
    this.backToListEvent.emit();
  }

  fetchStudentResults(): void {
    if (!this.student.selected || !this.template.selected) return;

    this.isLoading = true;
    this.errorMessage = null;

    setTimeout(() => {
      this.loadMockDataForSelection();
      this.isLoading = false;
    }, 800);
  }
  
  nextResult() { if (this.currentResultIndex < this.results.length - 1) this.currentResultIndex++; }
  prevResult() { if (this.currentResultIndex > 0) this.currentResultIndex--; }
  getCurrentResult(): Result | null { return this.results[this.currentResultIndex] ?? null; }

  // -----------------------
  // Mock data helpers
  // -----------------------
  private loadMockData(): void {
    this.results = [
      this.createMockResult('2024-01-15', 'Math Skills Assessment • First Assessment'),
      this.createMockResult('2024-03-20', 'Math Skills Assessment • Second Assessment'),
      this.createMockResult('2024-06-10', 'Math Skills Assessment • Third Assessment')
    ];
  }

  private loadMockDataForSelection(): void {
    const title = this.template.selected?.title || 'Selected Template';
    this.results = [
      this.createMockResult('2024-02-01', `${title} • Initial Assessment`),
      this.createMockResult('2024-04-15', `${title} • Mid-term Assessment`),
      this.createMockResult('2024-07-20', `${title} • Final Assessment`)
    ];
    this.currentResultIndex = 0;
  }

  private createMockResult(date: string, sessionTitle: string): Result {
    const isEarly = new Date(date) < new Date('2024-03-01');
    const isMid = new Date(date) >= new Date('2024-03-01') && new Date(date) < new Date('2024-06-01');
    const isLate = new Date(date) >= new Date('2024-06-01');

    let firstQuestionStudent = 'Fair';
    let firstQuestionTeacher = 'Good';
    let firstQuestionStudentIndex = 2;
    let firstQuestionTeacherIndex = 3;

    let secondQuestionStudent = 'Poor';
    let secondQuestionTeacher = 'Fair';
    let secondQuestionStudentIndex = 1;
    let secondQuestionTeacherIndex = 2;

    let customStudentResponse =
      "I find math challenging but I'm trying my best. Sometimes I get confused with word problems.";
    let customTeacherResponse =
      'The student is making effort but needs more support with problem-solving strategies and confidence building.';

    if (isMid) {
      firstQuestionStudent = 'Good';
      firstQuestionTeacher = 'Good';
      firstQuestionStudentIndex = 3;
      firstQuestionTeacherIndex = 3;

      secondQuestionStudent = 'Fair';
      secondQuestionTeacher = 'Good';
      secondQuestionStudentIndex = 2;
      secondQuestionTeacherIndex = 3;

      customStudentResponse =
        "I'm getting better at understanding math concepts. The extra practice with real-world examples is really helping me.";
      customTeacherResponse =
        'Notable improvement in conceptual understanding. The student responds well to practical applications and is gaining confidence.';
    } else if (isLate) {
      firstQuestionStudent = 'Excellent';
      firstQuestionTeacher = 'Excellent';
      firstQuestionStudentIndex = 4;
      firstQuestionTeacherIndex = 4;

      secondQuestionStudent = 'Good';
      secondQuestionTeacher = 'Excellent';
      secondQuestionStudentIndex = 3;
      secondQuestionTeacherIndex = 4;

      customStudentResponse =
        'Math has become much more enjoyable! I love solving complex problems now and I can see how it applies to real life. Group discussions really help me think differently.';
      customTeacherResponse =
        'Excellent progress! The student has developed strong analytical skills and shows genuine enthusiasm for mathematical problem-solving. Peer collaboration has significantly enhanced their learning.';
    }

    return {
      id: `mock-${date}`,
      title: `${sessionTitle} (${date})`,
      description: `Assessment completed on ${date}`,
      student: {
        user: {
          id: 'student-1',
          fullName: 'John Doe',
          userName: 'john.doe',
          role: Role.Student
        },
        completedAt: new Date(date)
      },
      teacher: {
        user: {
          id: 'teacher-1',
          fullName: 'Jane Smith',
          userName: 'jane.smith',
          role: Role.Teacher
        },
        completedAt: new Date(date)
      },
      answers: [
        {
          question: 'How well does the student understand basic arithmetic?',
          studentResponse: firstQuestionStudent,
          isStudentResponseCustom: false,
          teacherResponse: firstQuestionTeacher,
          isTeacherResponseCustom: false,
          options: [
            { displayText: 'Poor', optionValue: '1', isSelectedByStudent: firstQuestionStudentIndex === 1, isSelectedByTeacher: firstQuestionTeacherIndex === 1 },
            { displayText: 'Fair', optionValue: '2', isSelectedByStudent: firstQuestionStudentIndex === 2, isSelectedByTeacher: firstQuestionTeacherIndex === 2 },
            { displayText: 'Good', optionValue: '3', isSelectedByStudent: firstQuestionStudentIndex === 3, isSelectedByTeacher: firstQuestionTeacherIndex === 3 },
            { displayText: 'Excellent', optionValue: '4', isSelectedByStudent: firstQuestionStudentIndex === 4, isSelectedByTeacher: firstQuestionTeacherIndex === 4 },
            { displayText: 'Outstanding', optionValue: '5', isSelectedByStudent: firstQuestionStudentIndex === 5, isSelectedByTeacher: firstQuestionTeacherIndex === 5 }
          ]
        },
        {
          question: "Student's problem-solving skills in mathematics",
          studentResponse: secondQuestionStudent,
          isStudentResponseCustom: false,
          teacherResponse: secondQuestionTeacher,
          isTeacherResponseCustom: false,
          options: [
            { displayText: 'Poor', optionValue: '1', isSelectedByStudent: secondQuestionStudentIndex === 1, isSelectedByTeacher: secondQuestionTeacherIndex === 1 },
            { displayText: 'Fair', optionValue: '2', isSelectedByStudent: secondQuestionStudentIndex === 2, isSelectedByTeacher: secondQuestionTeacherIndex === 2 },
            { displayText: 'Good', optionValue: '3', isSelectedByStudent: secondQuestionStudentIndex === 3, isSelectedByTeacher: secondQuestionTeacherIndex === 3 },
            { displayText: 'Excellent', optionValue: '4', isSelectedByStudent: secondQuestionStudentIndex === 4, isSelectedByTeacher: secondQuestionTeacherIndex === 4 },
            { displayText: 'Outstanding', optionValue: '5', isSelectedByStudent: secondQuestionStudentIndex === 5, isSelectedByTeacher: secondQuestionTeacherIndex === 5 }
          ]
        },
        {
          question: 'What motivates the student most in learning?',
          studentResponse: customStudentResponse,
          isStudentResponseCustom: true,
          teacherResponse: customTeacherResponse,
          isTeacherResponseCustom: true,
          options: [
            { displayText: 'Grades and recognition', optionValue: '1', isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: 'Understanding concepts', optionValue: '2', isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: 'Practical applications', optionValue: '3', isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: 'Peer interaction', optionValue: '4', isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: 'Other (please specify)', optionValue: '5', isSelectedByStudent: false, isSelectedByTeacher: false }
          ]
        },
        {
          question: 'How does the student respond to collaborative learning activities?',
          studentResponse: isEarly ? 'Peer interaction' : isMid ? 'Practical applications' : 'Understanding concepts',
          isStudentResponseCustom: false,
          teacherResponse: isEarly
            ? 'The student initially struggles with group dynamics and tends to be passive in collaborative settings. They need encouragement to share ideas and participate actively.'
            : isMid
            ? 'Growing comfort with collaborative work. The student is beginning to engage more actively and shows improved communication skills during group activities.'
            : 'Excellent collaborative skills! The student has become a natural facilitator in group work, helping peers understand concepts while deepening their own learning through teaching others.',
          isTeacherResponseCustom: true,
          options: [
            { displayText: 'Prefers individual work', optionValue: '1', isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: 'Peer interaction', optionValue: '2', isSelectedByStudent: isEarly, isSelectedByTeacher: false },
            { displayText: 'Practical applications', optionValue: '3', isSelectedByStudent: isMid, isSelectedByTeacher: false },
            { displayText: 'Understanding concepts', optionValue: '4', isSelectedByStudent: isLate, isSelectedByTeacher: false },
            { displayText: 'Mixed results', optionValue: '5', isSelectedByStudent: false, isSelectedByTeacher: false }
          ]
        }
      ]
    };
  }
}
