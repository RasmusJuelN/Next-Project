

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

interface UserSearchEntity<T> extends SearchEntity<T> {
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
  imports: [
    TranslateModule,
    CommonModule,
    FormsModule,
    AgCharts,
    //ActiveAnonymousBuilderComponent,
  ],
  template: `    <ag-charts-angular
      [options]="chartOptions"
    ></ag-charts-angular> `,
  templateUrl: "./data-compare.component.html",
  styleUrl: "./data-compare.component.css",
})
export class DataCompareComponent implements OnInit, OnDestroy {
  // State for question navigation
  public currentQuestionIndex: number = 0;
  public questions: string[] = [];
  private allAnswers: any[] = [];
  @ViewChild("studentSearchArea", { static: false })
  studentSearchArea!: ElementRef;
  @ViewChild("templateSearchArea", { static: false })
  templateSearchArea!: ElementRef;
  constructor(
    public DataCompareService: DataCompareService
  ) {}

  public showStudentResults = false;
  public showTemplateResults = false;

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
  private activeService = inject(ActiveService);
  public groupName: string = "";
  public isAnonymousMode = false;

  public student: UserSearchEntity<User> = {
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

  // Set the page size (10 results per search)
  searchAmount = 10;

  @Output() backToListEvent = new EventEmitter<void>();

  ngOnInit(): void {
    // Subscribe to debounced search subjects.
    this.student.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.fetch("student", term);
      });
    this.template.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.fetch("template", term);
      });

    document.addEventListener("click", this.handleDocumentClick, true);
  }
  ngOnDestroy(): void {
    document.removeEventListener("click", this.handleDocumentClick, true);
  }

  // Returns the proper state based on the entity.
  private getState(entity: SearchType): SearchEntity<any> {
    if (entity === "student") {
      return this.student;
    } else if (entity === "template") {
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
    if (entity !== "template") {
      (state as UserSearchEntity<User>).sessionId = undefined;
    } else {
      (state as TemplateSearchEntity).queryCursor = undefined;
    }
    state.isLoading = true;
    state.errorMessage = null;

    if (entity === "template") {
      // For templates, pass the fixed searchAmount. (Assumes activeService.searchTemplates is updated accordingly.)
      const templateState = state as TemplateSearchEntity;
      this.activeService
        .searchTemplates(term, templateState.queryCursor)
        .subscribe({
          next: (response) => {
            templateState.searchResults = response.templateBases;
            // Disable load-more functionality by setting totalPages to 1.
            templateState.totalPages = 1;
            state.isLoading = false;
          },
          error: () => {
            state.errorMessage = "Failed to load templates.";
            state.isLoading = false;
          },
        });
    } else {
      const userState = state as UserSearchEntity<User>;
      // Use searchAmount (10) as the number of results.
      this.activeService
        .searchUsers(term, entity, this.searchAmount, userState.sessionId)
        .subscribe({
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
          },
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
      state.selected = state.selected.slice(-1); // Keep only the last selecteds
    } else {
      state.selected.splice(idx, 1);
    }
    // Do NOT clear search results so user can select multiple
    state.searchInput = "";
    // state.searchResults = [];

    // Optionally close results after selection
    if (entity === "student") {
      this.showStudentResults = false;
    } else {
      this.showTemplateResults = false;
    }
  }

  clearSelected(entity: SearchType): void {
    const state = this.getState(entity);
    state.selected = [];
  }

  onInputChange(entity: SearchType, value: string): void {
    const state = this.getState(entity);
    state.searchInput = value;
    state.searchSubject.next(value);

    // Show results when typing
    if (entity === "student") {
      this.showStudentResults = true;
    } else {
      this.showTemplateResults = true;
    }
  }

  onBackToList(): void {
    this.backToListEvent.emit();
  }

  // Fetch and transform API data for the chart
  fetchChartData(studentId: string, templateId: string) {
    this.DataCompareService.canGetData(studentId, templateId).subscribe({
      next: (apiData) => {
        // Flatten all answers from all DataCompare objects
        const allAnswers = apiData.flatMap(dc => dc.answers.map(ans => ({
          question: ans.question,
          studentResponse: ans.studentResponse,
          year: dc.studentCompletedAt.getFullYear(),
          month: dc.studentCompletedAt.toLocaleString('default', { month: 'short' }),
        })));
        console.log("allAnswers", allAnswers)
        // Get unique questions for x-axis
        const questions = Array.from(new Set(allAnswers.map(a => a.question)));
        // Get unique years for series (or use another grouping if needed)
        const years = Array.from(new Set(allAnswers.map(a => a.year))).sort();

        // Build chart data: one entry per question, each year as a value
        const chartData: any[] = [];
        questions.forEach(question => {
          const entry: any = { question };
          years.forEach(year => {
            const found = allAnswers.find(a => a.year === year && a.question === question);
            entry[year] = found ? Number(found.studentResponse) : null;
          });
          chartData.push(entry);
        });

        // Build series for each year
        const series = years.map(year => ({
          type: 'bar',
          xKey: 'question',
          yKey: year,
          yName: String(year),
          tooltip: {
            renderer: (params: any) => {
              return { content: `Svar: ${params.datum[year]}` };
            }
          }
        }));

        this.chartOptions = {
          data: chartData,
          title: {
            text: "Elev Data Sammenligning",
          },
          series,
          axes: [
            { type: 'category', position: 'bottom', title: { text: 'Spørgsmål' } },
            { type: 'number', position: 'left', title: { text: 'Svar' } },
          ],
        };
      },
      error: (err) => {
        this.chartOptions = null;
        // Optionally handle error (show message, etc)
        console.warn("Error fetching chart data", err);
        
      }
    });
  }



  
  onCompareClick() {
    const studentId = this.student.selected[0]?.id;
    const templateId = this.template.selected[0]?.id;
    if (studentId && templateId) {
      this.DataCompareService.canGetData(studentId, templateId).subscribe({
        next: (apiData) => {
          // Flatten all answers from all DataCompare objects
          const allAnswers = apiData.flatMap(dc => dc.answers.map(ans => ({
            question: ans.question,
            studentResponse: ans.studentResponse,
            year: String(new Date(dc.studentCompletedAt).getFullYear()),
            date: new Date(dc.studentCompletedAt).toLocaleDateString(),
          })));
          // Get unique questions
          this.allAnswers = allAnswers;
          this.questions = Array.from(new Set(allAnswers.map(a => a.question)));
          this.currentQuestionIndex = 0;
          this.updateChartForCurrentQuestion();
        },
        error: (err) => {
          this.chartOptions = null;
          console.warn("Error fetching chart data", err);
        }
      });
    }
  }

  // Update chart for the currently selected question
  updateChartForCurrentQuestion() {
    if (!this.questions.length) return;
    const question = this.questions[this.currentQuestionIndex];
    // Get all answers for this question
    const answersForQuestion = this.allAnswers.filter(a => a.question === question);
    // Group answers by year, and keep dates for tooltips
    const yearGroups: { [year: string]: { answer: string, date: string }[] } = {};
    answersForQuestion.forEach(a => {
      if (!yearGroups[a.year]) yearGroups[a.year] = [];
      yearGroups[a.year].push({ answer: a.studentResponse, date: a.date });
    });
    const years = Object.keys(yearGroups).sort();
    // Get all possible answers for this question (from all years)
    const answerLabels = Array.from(new Set(answersForQuestion.map(a => a.studentResponse)));
    // Build chart data: one entry per year, each answer as a value (count of times chosen that year), and store dates for tooltips
    const chartData = years.map(year => {
      const entry: any = { year };
      answerLabels.forEach(ans => {
        // Find all dates for this answer in this year
        const matches = yearGroups[year].filter(val => val.answer === ans);
        entry[ans] = matches.length;
        entry[`${ans}_dates`] = matches.map(m => m.date);
      });
      return entry;
    });
    // Build series for each answer
    const series = answerLabels.map(ans => ({
      type: 'bar',
      xKey: 'year',
      yKey: ans,
      yName: ans,
      stacked: true,
      tooltip: {
        renderer: (params: any) => {
          if (params.datum[ans]) {
            const dates = params.datum[`${ans}_dates`];
            return { content: `Svar: ${ans} (${params.datum[ans]})<br>Dato: ${Array.isArray(dates) ? dates.join(', ') : ''}` };
          }
          return { content: '' };
        }
      }
    }));
    this.chartOptions = Object.assign({}, {
      data: chartData,
      title: { text: question },
      series,
      axes: [
        { type: 'category', position: 'bottom', title: { text: 'År' } },
        { type: 'number', position: 'left', title: { text: 'Antal svar' }, min: 0 },
      ],
    });
  }

  // Navigation for questions
  nextQuestion() {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.currentQuestionIndex++;
      this.updateChartForCurrentQuestion();
    }
  }
  prevQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
      this.updateChartForCurrentQuestion();
    }
  }
} 




