import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, OnDestroy, OnInit, Output, ViewChild, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ActiveService } from '../active-questionnaire-manager/services/active.service';
import { User, Role } from '../../shared/models/user.model';
import { TemplateBase } from '../active-questionnaire-manager/models/active.models';
import { SearchEntity } from '../active-questionnaire-manager/models/searchEntity.model';
import { ShowResultComponent, ShowResultConfig } from '../../shared/show-result/show-result.component';
import { Result } from '../result/models/result.model';
import { ResultService } from '../result/services/result.service';
import { ResultHistoryService } from './services/result-history.service';

interface UserSearchEntity {
  selected: User[];
  searchInput: string;
  searchResults: User[];
  page: number;
  totalPages: number;
  isLoading: boolean;
  errorMessage: string | null;
  searchSubject: Subject<string>;
  sessionId?: string;
  hasMore?: boolean;
}

interface TemplateSearchEntity extends SearchEntity<TemplateBase> {
  queryCursor?: string;
}

interface StudentResult {
  result: Result;
  completedDate: string;
  templateTitle: string;
}

type SearchType = "student" | "template";

@Component({
  selector: 'app-result-history',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, ShowResultComponent],
  templateUrl: './result-history.component.html',
  styleUrls: ['./result-history.component.css']
})
export class ResultHistoryComponent implements OnInit, OnDestroy {
  // Search references for click-outside logic
  @ViewChild("studentSearchArea", { static: false })
  studentSearchArea!: ElementRef;
  @ViewChild("templateSearchArea", { static: false })
  templateSearchArea!: ElementRef;

  // Injected services
  private activeService = inject(ActiveService);
  private resultService = inject(ResultService);
  private resultHistoryService = inject(ResultHistoryService);

  // Controls visibility of search results dropdowns
  public showStudentResults = false;
  public showTemplateResults = false;

  // Current results state
  public studentResults: StudentResult[] = [];
  public currentResultIndex = 0;
  public isLoading = false;
  public errorMessage: string | null = null;

  // Show result component configuration
  public resultConfig: ShowResultConfig = {
    showTemplate: true,
    showStudent: true,
    showTeacher: false,
    showCompletionDates: true,
    useCardStyling: true,
    showActions: false
  };

  /**
   * State for student search and selection
   */
  public student: UserSearchEntity = {
    selected: [],
    searchInput: "",
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>(),
    sessionId: undefined,
    hasMore: false,
  };

  /**
   * State for template search and selection
   */
  public template: TemplateSearchEntity = {
    selected: [],
    searchInput: "",
    searchResults: [],
    page: 1,
    totalPages: 1,
    isLoading: false,
    errorMessage: null,
    searchSubject: new Subject<string>(),
    queryCursor: undefined,
  };

  // Number of results per search
  searchAmount = 10;

  // Emits event to parent when returning to list view
  @Output() backToListEvent = new EventEmitter<void>();

  /**
   * Handles click events outside of search areas to close dropdowns
   */
  private handleDocumentClick = (event: MouseEvent) => {
    const studentArea = this.studentSearchArea?.nativeElement;
    const templateArea = this.templateSearchArea?.nativeElement;
    if (studentArea && !studentArea.contains(event.target as Node)) {
      this.showStudentResults = false;
    }
    if (templateArea && !templateArea.contains(event.target as Node)) {
      this.showTemplateResults = false;
    }
  };

  ngOnInit(): void {
    // Debounced search for students
    this.student.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.fetch("student", term);
      });

    // Debounced search for templates
    this.template.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.fetch("template", term);
      });

    document.addEventListener("click", this.handleDocumentClick, true);

    // Load mock data for demonstration
    this.loadMockData();
  }

  ngOnDestroy(): void {
    document.removeEventListener("click", this.handleDocumentClick, true);
  }

  /**
   * Returns the state object for the given entity type
   */
  private getState(entity: SearchType): SearchEntity<any> {
    if (entity === "student") {
      return this.student;
    } else if (entity === "template") {
      return this.template;
    }
    throw new Error(`Unknown entity: ${entity}`);
  }

  /**
   * Fetches search results for students or templates
   */
  private fetch(entity: SearchType, term: string): void {
    const state = this.getState(entity);
    
    if (!term.trim()) return;

    state.page = 1;
    state.searchResults = [];
    
    if (entity !== "template") {
      (state as UserSearchEntity).sessionId = undefined;
    } else {
      (state as TemplateSearchEntity).queryCursor = undefined;
    }
    
    state.isLoading = true;
    state.errorMessage = null;

    if (entity === "template") {
      const templateState = state as TemplateSearchEntity;
      this.activeService
        .searchTemplates(term, templateState.queryCursor)
        .subscribe({
          next: (response) => {
            templateState.searchResults = response.templateBases;
            templateState.totalPages = 1;
            state.isLoading = false;
          },
          error: () => {
            state.errorMessage = "Failed to load templates.";
            state.isLoading = false;
          },
        });
    } else {
      const userState = state as UserSearchEntity;
      this.activeService
        .searchUsers(term, "student", this.searchAmount, userState.sessionId)
        .subscribe({
          next: (response) => {
            userState.searchResults = response.userBases || [];
            userState.sessionId = response.sessionId;
            userState.hasMore = false;
            state.isLoading = false;
          },
          error: () => {
            state.errorMessage = "Failed to load students.";
            state.isLoading = false;
          },
        });
    }
  }

  /**
   * Selects or deselects a user/template from the search results
   */
  select(entity: SearchType, item: any): void {
    const state = this.getState(entity);
    if (!Array.isArray(state.selected)) {
      state.selected = [];
    }

    const idx = state.selected.findIndex((u: any) => u.id === item.id);
    if (idx === -1) {
      state.selected.push(item);
      // Only keep the last selected item
      state.selected = state.selected.slice(-1);
    } else {
      state.selected.splice(idx, 1);
    }

    state.searchInput = "";

    if (entity === "student") {
      this.showStudentResults = false;
    } else {
      this.showTemplateResults = false;
    }

    // Fetch results when both student and template are selected
    if (this.student.selected.length > 0 && this.template.selected.length > 0) {
      this.fetchStudentResults();
    }
  }

  /**
   * Clears all selected users/templates
   */
  clearSelected(entity: SearchType): void {
    const state = this.getState(entity);
    state.selected = [];
    this.studentResults = [];
    this.currentResultIndex = 0;
  }

  /**
   * Handles input change for search fields and triggers search
   */
  onInputChange(entity: SearchType, value: string): void {
    const state = this.getState(entity);
    state.searchInput = value;
    state.searchSubject.next(value);

    if (entity === "student") {
      this.showStudentResults = true;
    } else {
      this.showTemplateResults = true;
    }
  }

  /**
   * Emits event to parent to go back to list view
   */
  onBackToList(): void {
    this.backToListEvent.emit();
  }

  /**
   * Fetches student results for the selected student and template combination
   */
  fetchStudentResults(): void {
    if (this.student.selected.length === 0 || this.template.selected.length === 0) {
      return;
    }

    const studentId = this.student.selected[0].id;
    const templateId = this.template.selected[0].id;

    this.isLoading = true;
    this.errorMessage = null;

    // TODO: Replace with actual API call to get student results by templateId and studentId
    // For now, simulate the API call
    setTimeout(() => {
      this.loadMockDataForSelection(studentId, templateId);
      this.isLoading = false;
    }, 1000);
  }

  /**
   * Navigate to next result
   */
  nextResult(): void {
    if (this.currentResultIndex < this.studentResults.length - 1) {
      this.currentResultIndex++;
    }
  }

  /**
   * Navigate to previous result
   */
  prevResult(): void {
    if (this.currentResultIndex > 0) {
      this.currentResultIndex--;
    }
  }

  /**
   * Get the currently displayed result
   */
  getCurrentResult(): Result | null {
    if (this.studentResults.length === 0) return null;
    return this.studentResults[this.currentResultIndex]?.result || null;
  }

  /**
   * Get the current result metadata
   */
  getCurrentResultInfo(): StudentResult | null {
    if (this.studentResults.length === 0) return null;
    return this.studentResults[this.currentResultIndex] || null;
  }

  /**
   * Load mock data for demonstration
   */
  private loadMockData(): void {
    // Mock student results for demonstration
    this.studentResults = [
      {
        result: this.createMockResult("2024-01-15", "First Assessment"),
        completedDate: "2024-01-15",
        templateTitle: "Math Skills Assessment"
      },
      {
        result: this.createMockResult("2024-03-20", "Second Assessment"),
        completedDate: "2024-03-20", 
        templateTitle: "Math Skills Assessment"
      },
      {
        result: this.createMockResult("2024-06-10", "Third Assessment"),
        completedDate: "2024-06-10",
        templateTitle: "Math Skills Assessment"
      }
    ];
  }

  /**
   * Load mock data based on selection (simulates API call)
   */
  private loadMockDataForSelection(studentId: string, templateId: string): void {
    // Simulate different results based on selection
    this.studentResults = [
      {
        result: this.createMockResult("2024-02-01", "Initial Assessment"),
        completedDate: "2024-02-01",
        templateTitle: this.template.selected[0]?.title || "Selected Template"
      },
      {
        result: this.createMockResult("2024-04-15", "Mid-term Assessment"), 
        completedDate: "2024-04-15",
        templateTitle: this.template.selected[0]?.title || "Selected Template"
      },
      {
        result: this.createMockResult("2024-07-20", "Final Assessment"),
        completedDate: "2024-07-20",
        templateTitle: this.template.selected[0]?.title || "Selected Template"
      }
    ];
    this.currentResultIndex = 0;
  }

  /**
   * Create a mock result for demonstration
   */
  private createMockResult(date: string, sessionName: string): Result {
    // Create different responses based on the date to show progression
    const isEarly = new Date(date) < new Date('2024-03-01');
    const isMid = new Date(date) >= new Date('2024-03-01') && new Date(date) < new Date('2024-06-01');
    const isLate = new Date(date) >= new Date('2024-06-01');

    // Different responses to show progression over time
    let firstQuestionStudent = "Fair";
    let firstQuestionTeacher = "Good";
    let firstQuestionStudentIndex = 2; // Fair
    let firstQuestionTeacherIndex = 3; // Good

    let secondQuestionStudent = "Poor";
    let secondQuestionTeacher = "Fair";
    let secondQuestionStudentIndex = 1; // Poor
    let secondQuestionTeacherIndex = 2; // Fair

    let customStudentResponse = "I find math challenging but I'm trying my best. Sometimes I get confused with word problems.";
    let customTeacherResponse = "The student is making effort but needs more support with problem-solving strategies and confidence building.";

    if (isMid) {
      // Mid assessment - showing improvement
      firstQuestionStudent = "Good";
      firstQuestionTeacher = "Good";
      firstQuestionStudentIndex = 3; // Good
      firstQuestionTeacherIndex = 3; // Good

      secondQuestionStudent = "Fair";
      secondQuestionTeacher = "Good";
      secondQuestionStudentIndex = 2; // Fair
      secondQuestionTeacherIndex = 3; // Good

      customStudentResponse = "I'm getting better at understanding math concepts. The extra practice with real-world examples is really helping me.";
      customTeacherResponse = "Notable improvement in conceptual understanding. The student responds well to practical applications and is gaining confidence.";
    } else if (isLate) {
      // Late assessment - showing significant improvement
      firstQuestionStudent = "Excellent";
      firstQuestionTeacher = "Excellent";
      firstQuestionStudentIndex = 4; // Excellent
      firstQuestionTeacherIndex = 4; // Excellent

      secondQuestionStudent = "Good";
      secondQuestionTeacher = "Excellent";
      secondQuestionStudentIndex = 3; // Good
      secondQuestionTeacherIndex = 4; // Excellent

      customStudentResponse = "Math has become much more enjoyable! I love solving complex problems now and I can see how it applies to real life. Group discussions really help me think differently.";
      customTeacherResponse = "Excellent progress! The student has developed strong analytical skills and shows genuine enthusiasm for mathematical problem-solving. Peer collaboration has significantly enhanced their learning.";
    }

    return {
      id: `mock-${date}`,
      title: `${sessionName} - ${date}`,
      description: `Assessment completed on ${date}`,
      student: {
        user: {
          id: "student-1",
          fullName: "John Doe", 
          userName: "john.doe",
          role: Role.Student
        },
        completedAt: new Date(date)
      },
      teacher: {
        user: {
          id: "teacher-1",
          fullName: "Jane Smith",
          userName: "jane.smith",
          role: Role.Teacher
        },
        completedAt: new Date(date)
      },
      answers: [
        {
          question: "How well does the student understand basic arithmetic?",
          studentResponse: firstQuestionStudent,
          isStudentResponseCustom: false,
          teacherResponse: firstQuestionTeacher, 
          isTeacherResponseCustom: false,
          options: [
            { displayText: "Poor", optionValue: "1", isSelectedByStudent: firstQuestionStudentIndex === 1, isSelectedByTeacher: firstQuestionTeacherIndex === 1 },
            { displayText: "Fair", optionValue: "2", isSelectedByStudent: firstQuestionStudentIndex === 2, isSelectedByTeacher: firstQuestionTeacherIndex === 2 },
            { displayText: "Good", optionValue: "3", isSelectedByStudent: firstQuestionStudentIndex === 3, isSelectedByTeacher: firstQuestionTeacherIndex === 3 },
            { displayText: "Excellent", optionValue: "4", isSelectedByStudent: firstQuestionStudentIndex === 4, isSelectedByTeacher: firstQuestionTeacherIndex === 4 },
            { displayText: "Outstanding", optionValue: "5", isSelectedByStudent: firstQuestionStudentIndex === 5, isSelectedByTeacher: firstQuestionTeacherIndex === 5 }
          ]
        },
        {
          question: "Student's problem-solving skills in mathematics",
          studentResponse: secondQuestionStudent,
          isStudentResponseCustom: false,
          teacherResponse: secondQuestionTeacher,
          isTeacherResponseCustom: false,
          options: [
            { displayText: "Poor", optionValue: "1", isSelectedByStudent: secondQuestionStudentIndex === 1, isSelectedByTeacher: secondQuestionTeacherIndex === 1 },
            { displayText: "Fair", optionValue: "2", isSelectedByStudent: secondQuestionStudentIndex === 2, isSelectedByTeacher: secondQuestionTeacherIndex === 2 },
            { displayText: "Good", optionValue: "3", isSelectedByStudent: secondQuestionStudentIndex === 3, isSelectedByTeacher: secondQuestionTeacherIndex === 3 },
            { displayText: "Excellent", optionValue: "4", isSelectedByStudent: secondQuestionStudentIndex === 4, isSelectedByTeacher: secondQuestionTeacherIndex === 4 },
            { displayText: "Outstanding", optionValue: "5", isSelectedByStudent: secondQuestionStudentIndex === 5, isSelectedByTeacher: secondQuestionTeacherIndex === 5 }
          ]
        },
        {
          question: "What motivates the student most in learning?",
          studentResponse: customStudentResponse,
          isStudentResponseCustom: true,
          teacherResponse: customTeacherResponse,
          isTeacherResponseCustom: true,
          options: [
            { displayText: "Grades and recognition", optionValue: "1", isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: "Understanding concepts", optionValue: "2", isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: "Practical applications", optionValue: "3", isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: "Peer interaction", optionValue: "4", isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: "Other (please specify)", optionValue: "5", isSelectedByStudent: false, isSelectedByTeacher: false }
          ]
        },
        {
          question: "How does the student respond to collaborative learning activities?",
          studentResponse: isEarly ? "Peer interaction" : isMid ? "Practical applications" : "Understanding concepts",
          isStudentResponseCustom: false,
          teacherResponse: isEarly 
            ? "The student initially struggles with group dynamics and tends to be passive in collaborative settings. They need encouragement to share ideas and participate actively."
            : isMid 
            ? "Growing comfort with collaborative work. The student is beginning to engage more actively and shows improved communication skills during group activities."
            : "Excellent collaborative skills! The student has become a natural facilitator in group work, helping peers understand concepts while deepening their own learning through teaching others.",
          isTeacherResponseCustom: true,
          options: [
            { displayText: "Prefers individual work", optionValue: "1", isSelectedByStudent: false, isSelectedByTeacher: false },
            { displayText: "Peer interaction", optionValue: "2", isSelectedByStudent: isEarly, isSelectedByTeacher: false },
            { displayText: "Practical applications", optionValue: "3", isSelectedByStudent: isMid, isSelectedByTeacher: false },
            { displayText: "Understanding concepts", optionValue: "4", isSelectedByStudent: isLate, isSelectedByTeacher: false },
            { displayText: "Mixed results", optionValue: "5", isSelectedByStudent: false, isSelectedByTeacher: false }
          ]
        }
      ]
    };
  }
}
