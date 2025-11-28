// Angular component for comparing anonymised questionnaire data

import {
  Component,
  ElementRef,
  EventEmitter,
  OnDestroy,
  OnInit,
  ViewChild,
  inject,
  Output,
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TranslateModule } from "@ngx-translate/core";
import { ActiveAnonymousBuilderComponent } from "../active-questionnaire-manager/components/active-anonymous-builder/active-anonymous-builder.component";
import { ActiveService } from "../active-questionnaire-manager/services/active.service";
import { User } from "../../shared/models/user.model";
import { SearchEntity } from "../active-questionnaire-manager/models/searchEntity.model";
import { debounceTime, distinctUntilChanged, Subject } from "rxjs";
import { TemplateBase } from "../active-questionnaire-manager/models/active.models";
import { AgCharts } from "ag-charts-angular";
import { DataCompareService } from "./services/data-compare.service";
import { HttpClient } from "@angular/common/http";

// ---- NEW: chart strategy imports ----
import { ChartRegistry } from "./charts/chart-registry";
import {
  ChartBuildInput,
  ChartType,
  AnswerCounts,
} from "./models/data-compare.model";

// User or group result type for search
type UserOrGroup =
  | (User & { type: "user" })
  | { groupId: string; name: string; type: "group" };

interface UserSearchEntity {
  selected: UserOrGroup[];
  searchInput: string;
  searchResults: UserOrGroup[];
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

type SearchType = "student" | "template";

@Component({
    selector: "app-data-compare",
    imports: [TranslateModule, FormsModule, AgCharts],
    templateUrl: "./data-compare.component.html",
    styleUrl: "./data-compare.component.css"
})
/**
 * DataCompareComponent
 * UI and logic for comparing anonymised questionnaire data by question and year.
 */
export class DataCompareComponent implements OnInit, OnDestroy {
  // Index of the currently selected question
  public currentQuestionIndex: number = 0;
  // List of all questions found in the dataset
  public questions: string[] = [];
  // Optional stable identifiers for questions (when backend provides them)
  private questionIds: Array<string | null> = [];
  // All datasets returned from the API (used for aggregation)
  private allAnswers: any[] = [];
  // List of all years found in the dataset
  public years: string[] = [];
  // Index of the currently selected year
  public currentYearIndex: number = 0;

  // ---- NEW: Chart state switched to strategy-based ----
  public chartType: ChartType = "pie";
  public chartOptions: any = {};
  private registry = new ChartRegistry();

  // References to search input areas for click-outside logic
  @ViewChild("studentSearchArea", { static: false })
  studentSearchArea!: ElementRef;
  @ViewChild("templateSearchArea", { static: false })
  templateSearchArea!: ElementRef;

  // Inject services
  constructor(
    public DataCompareService: DataCompareService,
    private http: HttpClient
  ) {}
  private activeService = inject(ActiveService);

  // Controls visibility of student/template search results dropdowns
  public showStudentResults = false;
  public showTemplateResults = false;

  // Misc state (not used in chart logic)
  public groupName: string = "";
  public isAnonymousMode = false;

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

  /**
   * Component initialization: subscribe to search subjects and set up click-outside handler
   */
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
  }

  /**
   * Cleanup: remove click-outside handler
   */
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
   * Fetches users/templates and merges with groups (for students)
   */
  private fetch(entity: SearchType, term: string): void {
    const state = this.getState(entity);
    if (!term.trim()) return;

    // Reset pagination/results
    state.page = 1;
    state.searchResults = [];

    // Reset cursors
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
        .searchUsers(term, entity, this.searchAmount, userState.sessionId)
        .subscribe({
          next: (response) => {
            const userBases = response.userBases || [];
            userState.sessionId = response.sessionId;

            // Also search groups
            this.http
              .get<any[]>(`${this.DataCompareService.apiUrl}/groupsbasic`)
              .subscribe({
                next: (groups) => {
                  const filteredGroups = groups.filter((g) =>
                    g.name.toLowerCase().includes(term.toLowerCase())
                  );
                  const groupResults = filteredGroups.map((g) => ({
                    ...g,
                    type: "group",
                  }));
                  const userResults = userBases.map((u: any) => ({
                    ...u,
                    type: "user",
                  }));
                  userState.searchResults = [...userResults, ...groupResults];
                  userState.hasMore = false;
                  state.isLoading = false;
                },
                error: () => {
                  // Fall back to users only
                  userState.searchResults = userBases.map((u: any) => ({
                    ...u,
                    type: "user",
                  }));
                  userState.hasMore = false;
                  state.isLoading = false;
                },
              });
          },
          error: () => {
            state.errorMessage = `Failed to load ${entity}s.`;
            state.isLoading = false;
          },
        });
    }
  }

  /**
   * Selects or deselects a user or group from the search results
   */
  select(entity: SearchType, item: any): void {
    const state = this.getState(entity);
    if (!Array.isArray(state.selected)) {
      state.selected = [];
    }
    // Only allow one selected item (keep last selected)
    const idKey = item.type === "group" ? "groupId" : "id";
    const idx = state.selected.findIndex(
      (u: any) => u.type === item.type && u[idKey] === item[idKey]
    );
    if (idx === -1) {
      state.selected.push(item);
      state.selected = state.selected.slice(-1);
    } else {
      state.selected.splice(idx, 1);
    }
    // Clear search input after selection
    state.searchInput = "";

    if (entity === "student") {
      this.showStudentResults = false;
    } else {
      this.showTemplateResults = false;
    }
    // Update chart after selection state is updated
    this.onCompareClick();
  }

  /**
   * Clears all selected users/templates
   */
  clearSelected(entity: SearchType): void {
    const state = this.getState(entity);
    state.selected = [];
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
   * Fetches anonymised response data and prepares navigation state (questions, years)
   * @param templateId Questionnaire/template GUID
   * @param userId User GUID (optional)
   * @param groupId Group GUID (optional)
   */
  fetchChartData(templateId: string, userId?: string, groupId?: string) {
    this.DataCompareService.getAnonymisedResponses(
      templateId,
      userId,
      groupId
    ).subscribe({
      next: (apiData) => {
        // DEBUG: log raw API response to help debug duplicate/merged questions
        // Remove or disable these logs after debugging
        console.debug('DataCompare: raw apiData', apiData);
        const datasets = apiData.anonymisedResponseDataSet || [];

        // Build ordered list of questions by position (don't dedupe by text)
        // Some questions may have identical text (e.g. 'N/A') but represent
        // different question positions. Using position prevents merging them.
        const maxLen = datasets.length
          ? Math.max(...datasets.map((d: any) => d.anonymisedResponses?.length || 0))
          : 0;
        const questionsByPosition: string[] = [];
        const detectedQuestionIds: Array<string | null> = [];
        for (let i = 0; i < maxLen; i++) {
          // Prefer the label from the first dataset that has a value at this index
          let label = "";
          let idVal: string | null = null;
          for (const d of datasets) {
            if (
              d.anonymisedResponses &&
              d.anonymisedResponses[i] &&
              d.anonymisedResponses[i].question
            ) {
              const q = d.anonymisedResponses[i];
              label = q.question;
              // detect stable id field if backend provided one
              idVal = q.id ?? q.questionId ?? null;
              break;
            }
          }
          questionsByPosition.push(label || `Question ${i + 1}`);
          detectedQuestionIds.push(idVal);
        }

        // remember detected question ids for matching later
        this.questionIds = detectedQuestionIds;

        // If multiple questions have the same label (e.g. 'N/A'), disambiguate
        // them for display by appending a short position suffix so the UI
        // treats them as distinct entries while keeping the original text.
        const labelCounts: Record<string, number> = {};
        const disambiguated = questionsByPosition.map((lbl, idx) => {
          const key = lbl || `Question ${idx + 1}`;
          labelCounts[key] = (labelCounts[key] || 0) + 1;
          return { label: key, idx };
        });

        // If any label occurs more than once, append a (n) suffix to each
        const duplicates = Object.keys(labelCounts).filter((k) => labelCounts[k] > 1);
        if (duplicates.length > 0) {
          const occurrence: Record<string, number> = {};
          this.questions = disambiguated.map(({ label, idx }) => {
            occurrence[label] = (occurrence[label] || 0) + 1;
            if (labelCounts[label] > 1) {
              return `${label} (${occurrence[label]})`;
            }
            return label;
          });
        } else {
          // DEBUG: inspect computed lists
          console.debug('DataCompare: questionsByPosition', questionsByPosition);
          this.questions = questionsByPosition;
        }

        // Build set of all unique years (extract from datasetTitle)
        const yearsSet = new Set<string>();
        datasets.forEach((d: any) => {
          const year = d.datasetTitle?.substring(0, 4);
          if (year) yearsSet.add(year);
        });
        this.years = Array.from(yearsSet).sort();

        // Save all datasets for later aggregation
        this.allAnswers = datasets;

        // Start at first question and last year (most recent)
        this.currentQuestionIndex = 0;
        this.currentYearIndex =
          this.years.length > 0 ? this.years.length - 1 : 0;

        // DEBUG: final computed questions and datasets summary
        console.debug('DataCompare: final questions', this.questions);
        console.debug('DataCompare: datasets count', datasets.length, 'allAnswers length', this.allAnswers.length);
        this.updateChartForCurrentQuestion();
      },
      error: (err) => {
        this.chartOptions = null;
        console.warn("Error fetching anonymised chart data", err);
      },
    });
  }

  /**
   * Handles chart update: fetches data for selected template and (optionally) user/group
   */
  onCompareClick() {
    const templateId = this.template.selected[0]?.id;
    let userId: string | undefined = undefined;
    let groupId: string | undefined = undefined;

    if (this.student.selected.length > 0) {
      const selected = this.student.selected[0];
      if (selected.type === "user") {
        userId = (selected as User).id;
      } else if (selected.type === "group") {
        groupId = selected.groupId;
      }
    }
    if (templateId) {
      this.fetchChartData(templateId, userId, groupId);
    }
  }

  // ---- NEW: build a strategy-agnostic input for the chart ----
  private buildChartInput(): ChartBuildInput | null {
    if (
      !Array.isArray(this.questions) ||
      !Array.isArray(this.years) ||
      !this.questions.length ||
      !this.years.length
    )
      return null;

    const questionIndex = this.currentQuestionIndex;
    const year = this.years[this.currentYearIndex];

    // Filter datasets for the selected year (datasetTitle starts with year)
    const datasetsForYear = this.allAnswers.filter((d: any) =>
      d.datasetTitle?.startsWith(year)
    );

    // Aggregate answer counts for the selected question across all datasets in the year.
    // Prefer matching by stable question id when the backend provides one; fall back to positional index.
    const answers: AnswerCounts = {};
    datasetsForYear.forEach((dataset: any) => {
      let questionObj: any = null;
      const qId = this.questionIds && this.questionIds[questionIndex];
      if (qId && Array.isArray(dataset.anonymisedResponses)) {
        questionObj = dataset.anonymisedResponses.find(
          (q: any) => q.id === qId || q.questionId === qId
        );
      }
      // Fallback to positional lookup if no id match
      if (!questionObj && dataset.anonymisedResponses) {
        questionObj = dataset.anonymisedResponses[questionIndex];
      }

      if (questionObj && Array.isArray(questionObj.answers)) {
        questionObj.answers.forEach((a: any) => {
          if (!answers[a.answer]) {
            answers[a.answer] = { count: 0, dates: [] };
          }
          answers[a.answer].count += a.count;
          answers[a.answer].dates.push(dataset.datasetTitle);
        });
      }
    });

    // DEBUG: inspect aggregated answers produced by buildChartInput
    console.debug('DataCompare: buildChartInput - questionIndex', questionIndex, 'year', year, 'answers', answers);

  const question = this.questions[this.currentQuestionIndex];
  return { question, year, answers };
  }

  /**
   * Updates chart for the currently selected question/year via the active strategy
   */
  updateChartForCurrentQuestion() {
    const input = this.buildChartInput();
    if (!input) return;

    const strategy = this.registry.get(this.chartType);
    this.chartOptions = strategy.buildOptions(input);
  }

  /**
   * Switch chart type at runtime (optional UI binding)
   */
  setChartType(type: ChartType | string) {
    if (!this.registry.has(type)) return;
    this.chartType = type as ChartType;
    this.updateChartForCurrentQuestion();
  }

  /**
   * Navigates to the next question
   */
  nextQuestion() {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.currentQuestionIndex++;
      this.updateChartForCurrentQuestion();
    }
  }

  /**
   * Navigates to the previous question
   */
  prevQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
      this.updateChartForCurrentQuestion();
    }
  }

  /**
   * Navigates to the next year
   */
  nextYear() {
    if (this.currentYearIndex < this.years.length - 1) {
      this.currentYearIndex++;
      this.updateChartForCurrentQuestion();
    }
  }

  /**
   * Navigates to the previous year
   */
  prevYear() {
    if (this.currentYearIndex > 0) {
      this.currentYearIndex--;
      this.updateChartForCurrentQuestion();
    }
  }
}
