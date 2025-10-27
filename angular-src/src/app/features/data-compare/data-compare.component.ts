// Angular component for comparing anonymised questionnaire data
import { CommonModule } from "@angular/common";
import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
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
import { DataCompareOvertimeComponent } from "../data-compare-overtime/data-compare-overtime.component";

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
  standalone: true,
  imports: [TranslateModule, CommonModule, FormsModule, AgCharts,DataCompareOvertimeComponent],
  template: `
    <ag-charts-angular [options]="chartOptions"></ag-charts-angular>
  `,
  templateUrl: "./data-compare.component.html",
  styleUrl: "./data-compare.component.css",
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
  // All datasets returned from the API (used for aggregation)
  private allAnswers: any[] = [];
  // List of all years found in the dataset
  public years: string[] = [];
  // Index of the currently selected year
  public currentYearIndex: number = 0;
  // Chart type: 'pie' or 'bar'
  public chartType: string = "pie";
  // References to search input areas for click-outside logic
  @ViewChild("studentSearchArea", { static: false })
  studentSearchArea!: ElementRef;
  @ViewChild("templateSearchArea", { static: false })
  templateSearchArea!: ElementRef;
  // Inject DataCompareService for API calls
  // Inject HttpClient for group search
  constructor(
    public DataCompareService: DataCompareService,
    private http: HttpClient
  ) {}

  // Controls visibility of student/template search results dropdowns
  public showStudentResults = false;
  public showTemplateResults = false;

  // Chart options for ag-charts-angular
  public chartOptions: any = {
    data: [],
    title: {
      text: "Elev Data Sammenligning",
    },
    series: [
      {
        type: "area",
        xKey: "month",
        yKey: "subscriptions",
        yName: "Subscriptions",
      },
      {
        type: "area",
        xKey: "month",
        yKey: "services",
        yName: "Services",
      },
      {
        type: "area",
        xKey: "month",
        yKey: "products",
        yName: "Products",
      },
    ],
  };

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
  // Inject ActiveService for searching users/templates
  private activeService = inject(ActiveService);
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
   * Handles search input for students/templates and updates results
   */
  /**
   * Fetches users and groups for the search term and merges results
   */
  private fetch(entity: SearchType, term: string): void {
    const state = this.getState(entity);
    // Ignore empty search terms
    if (!term.trim()) return;

    // Always treat this as a new search (reset pagination and results)
    state.page = 1;
    state.searchResults = [];
    // Reset sessionId for user search, queryCursor for template search
    if (entity !== "template") {
      (state as UserSearchEntity).sessionId = undefined;
    } else {
      (state as TemplateSearchEntity).queryCursor = undefined;
    }
    state.isLoading = true;
    state.errorMessage = null;

    if (entity === "template") {
      const templateState = state as TemplateSearchEntity;
      // Search for templates (only first page, disables load-more)
      this.activeService
        .searchTemplates(term, templateState.queryCursor)
        .subscribe({
          next: (response) => {
            templateState.searchResults = response.templateBases;
            // Only one page of results
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
      // Search for users
      this.activeService
        .searchUsers(term, entity, this.searchAmount, userState.sessionId)
        .subscribe({
          next: (response) => {
            const userBases = response.userBases || [];
            userState.sessionId = response.sessionId;
            // Now also search for groups
            this.http
              .get<any[]>(`${this.DataCompareService.apiUrl}/groupsbasic`)
              .subscribe({
                next: (groups) => {
                  // Filter groups by search term
                  const filteredGroups = groups.filter((g) =>
                    g.name.toLowerCase().includes(term.toLowerCase())
                  );
                  // Mark type for rendering
                  const groupResults = filteredGroups.map((g) => ({
                    ...g,
                    type: "group",
                  }));
                  const userResults = userBases.map((u) => ({
                    ...u,
                    type: "user",
                  }));
                  // Merge users and groups
                  userState.searchResults = [...userResults, ...groupResults];
                  userState.hasMore = false;
                  state.isLoading = false;
                },
                error: () => {
                  // If group search fails, just show users
                  userState.searchResults = userBases.map((u) => ({
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
   * Selects or deselects a user/template from the search results
   */
  /**
   * Selects or deselects a user or group from the search results
   */
  select(entity: SearchType, item: any): void {
    const state = this.getState(entity);
    if (!Array.isArray(state.selected)) {
      state.selected = [];
    }
    // Only allow one selected item (keep last selected)
    // Use id for users, groupId for groups
    const idKey = item.type === "group" ? "groupId" : "id";
    const idx = state.selected.findIndex(
      (u: any) => u.type === item.type && u[idKey] === item[idKey]
    );
    if (idx === -1) {
      state.selected.push(item);
      // Only keep the last selected item
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
   * @param studentId User GUID (optional)
   */
  fetchChartData(templateId: string, userId?: string, groupId?: string) {
    this.DataCompareService.getAnonymisedResponses(
      templateId,
      userId,
      groupId
    ).subscribe({
      next: (apiData) => {
        // Defensive: fallback to empty array if API response is missing
        const datasets = apiData.anonymisedResponseDataSet || [];
        // Build set of all unique questions
        const questionsSet = new Set<string>();
        datasets.forEach((dataset: any) => {
          dataset.anonymisedResponses.forEach((q: any) =>
            questionsSet.add(q.question)
          );
        });
        this.questions = Array.from(questionsSet);
        // Build set of all unique years (extract from datasetTitle)
        const yearsSet = new Set<string>();
        datasets.forEach((d: any) => {
          // Defensive: substring(0, 4) assumes datasetTitle is a date string
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
        this.updateChartForCurrentQuestion();
      },
      error: (err) => {
        this.chartOptions = null;
        console.warn("Error fetching anonymised chart data", err);
      },
    });
  }

  /**
   * Handles chart update: fetches data for selected template and (optionally) user
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

  /**
   * Updates chart for the currently selected question and year
   * Aggregates all answers for the question in the selected year
   */
  updateChartForCurrentQuestion() {
    if (
      !Array.isArray(this.questions) ||
      !Array.isArray(this.years) ||
      !this.questions.length ||
      !this.years.length
    )
      return;
    const question = this.questions[this.currentQuestionIndex];
    const year = this.years[this.currentYearIndex];
    // Filter all datasets for the selected year (datasetTitle starts with year)
    const datasetsForYear = this.allAnswers.filter((d: any) =>
      d.datasetTitle?.startsWith(year)
    );
    // Aggregate answer counts for the selected question across all datasets in the year
    const answerCounts: Record<string, { count: number; dates: string[] }> = {};
    datasetsForYear.forEach((dataset: any) => {
      // Find the question object in this dataset
      const questionObj = dataset.anonymisedResponses.find(
        (q: any) => q.question === question
      );
      if (questionObj) {
        // For each answer, sum counts and collect dates
        questionObj.answers.forEach((a: any) => {
          if (!answerCounts[a.answer]) {
            answerCounts[a.answer] = { count: 0, dates: [] };
          }
          answerCounts[a.answer].count += a.count;
          answerCounts[a.answer].dates.push(dataset.datasetTitle);
        });
      }
    });
    const chartData: any[] = [];
    // Build chart data row: each answer is a yKey, question is xKey
    const entry: Record<string, any> = { question };
    Object.entries(answerCounts).forEach(([answer, obj]) => {
      entry[answer] = obj.count;
      // Store all dates for tooltip display
      entry[`${answer}_dates`] = obj.dates;
    });
    chartData.push(entry);
    const theme = {
      baseTheme: "ag-polychroma",
      overrides: {
        bar: { series: { label: { enabled: true } } },
      },
    };
    if (this.chartType === "pie") {
      // Pie chart: each answer is a slice, value is count
      const pieData = Object.entries(answerCounts).map(([ans, obj]) => {
        return {
          answer: ans,
          count: obj.count,
          dates: obj.dates,
        };
      });
      this.chartOptions = {
        data: pieData,
        title: { text: `${question} (${year})` },
        theme,
        series: [
          {
            type: "pie",
            calloutLabelKey: "answer",
            sectorLabelKey: "count",
            angleKey: "count",
            calloutLabel: { offset: 20 },
            sectorLabel: {
              positionOffset: 30,
              // Only show percentage label if >= 5%
              formatter: ({
                datum,
                angleKey,
              }: {
                datum: any;
                angleKey: string;
              }) => {
                const value = datum[angleKey];
                const total = pieData.reduce((sum, d) => sum + d.count, 0);
                const percentage = total
                  ? ((value / total) * 100).toFixed(1)
                  : "0";
                return parseFloat(percentage) >= 5 ? `${percentage}%` : "";
              },
            },
            strokeWidth: 1,
            tooltip: {
              enabled: true,
              renderer: (params: { datum: any; angleKey: string }) => {
                const { datum, angleKey } = params;
                const value = datum[angleKey];
                const total = pieData.reduce((sum, d) => sum + d.count, 0);
                const percentage = total
                  ? ((value / total) * 100).toFixed(1)
                  : "0";
                // Tooltip shows answer, count, percentage, and all dates
                return {
                  title: datum.answer,
                  content: `Antal: ${value}<br> Dato: ${datum.dates.join(
                    ", "
                  )}`,
                };
              },
            },
          },
        ],
        legend: { enabled: true },
        animation: { enabled: true, duration: 800 },
      };
    } else {
      // Bar chart: each answer is a bar, value is count
      const series = Object.keys(answerCounts).map((answer) => ({
        type: "bar",
        xKey: "question",
        yKey: answer,
        yName: `Svar: ${answer}`,
        stacked: true,
        label: { enabled: true },
        tooltip: {
          enabled: true,
          renderer: (params: { datum: any }) => {
            // Tooltip shows answer, count, and all dates
            const dates = params.datum[`${answer}_dates`] || [];
            return {
              title: answer,
              content: `Antal: ${params.datum[answer]}<br>Dato: ${dates.join(
                ", "
              )}`,
            };
          },
        },
      }));
      this.chartOptions = {
        data: chartData,
        theme,
        title: { text: `${question} (${year})`, fontSize: 18 },
        series,
        axes: [
          {
            type: "category",
            position: "bottom",
            title: { text: "Spørgsmål" },
          },
          { type: "number", position: "left", title: { text: "Antal" } },
        ],
        legend: { enabled: true },
        animation: { enabled: true, duration: 800 },
      };
    }
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
