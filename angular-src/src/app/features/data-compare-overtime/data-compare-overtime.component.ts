import { CommonModule } from '@angular/common';
import type { AgCartesianChartOptions } from 'ag-charts-community';
import { Component, OnInit, inject, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { AgCharts } from 'ag-charts-angular';
import { DataCompareService } from './services/data-compare-overtime.service';
import { TranslateModule } from "@ngx-translate/core";
import { FormsModule } from "@angular/forms";
import { User } from "../../shared/models/user.model";
import { SearchEntity } from "../active-questionnaire-manager/models/searchEntity.model";
import { debounceTime, distinctUntilChanged, Subject } from "rxjs";
import { ActiveService } from "../active-questionnaire-manager/services/active.service";
import { TemplateBase } from "../active-questionnaire-manager/models/active.models";
import { HttpClient } from "@angular/common/http";

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
  selector: 'app-data-compare-overtime',
  standalone: true,
  imports: [CommonModule, AgCharts, TranslateModule, FormsModule],
  templateUrl: './data-compare-overtime.component.html',
  styleUrls: ['./data-compare-overtime.component.css'],
})
export class DataCompareOvertimeComponent implements OnInit, OnDestroy {
  private dataCompare = inject(DataCompareService);
  private activeService = inject(ActiveService);
  private http = inject(HttpClient);


  public answers: string[] = [];
  public allQuestions: any[] = [];
  public allResponses: any[] = [];

  public currentQuestionIndex = 0;
  public totalQuestions = 0;

  // Search-related properties
  @ViewChild("studentSearchArea", { static: false })
  studentSearchArea!: ElementRef;
  @ViewChild("templateSearchArea", { static: false })
  templateSearchArea!: ElementRef;

  public showStudentResults = false;
  public showTemplateResults = false;
  searchAmount = 10;

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

  public options: AgCartesianChartOptions = {
    title: { text: 'Svar over tid' },
    data: [],
    series: [
      {
        type: 'line',
        xKey: 'date',
        yKey: 'TeacherAnswer',
        yName: 'Lærer',
        tooltip: {
          renderer: (params) => ({
            content: `Lærer: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
          }),
        },
      },
      {
        type: 'line',
        xKey: 'date',
        yKey: 'StudentAnswer',
        yName: 'Elev',
        tooltip: {
          renderer: (params) => ({
            content: `Elev: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
          }),
        },
      },
    ],
    axes: [
      { type: 'time', position: 'bottom', title: { text: 'Tidspunkt for besvarelse' } },
      {
        type: 'number',
        position: 'left',
        label: {
          formatter: (params) => this.answers[params.value] ?? '',
        },
      },
    ],
  };

  ngOnInit(): void {
    // demo IDs
    //const templateId = '3fa85f64-5717-4562-b3fc-2c963f66afa6';
    //const studentId = '6a6515fd-16b6-4961-91e4-b37d6f2c2c28';
    const teacherId = 'f3229540-03d1-4f55-919c-feac1ebe0dba';
    
    //this.getQuestionareOptions(templateId, studentId, teacherId);
    
    // Subscribe to search subjects
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
      .searchUsers(term, entity, this.searchAmount, userState.sessionId)
      .subscribe({
        next: (response) => {
          const userBases = response.userBases || [];
          userState.sessionId = response.sessionId;
          // Only show users, no groups for data-compare-overtime
          const userResults = userBases.map((u) => ({
            ...u,
            type: "user" as const, // Use 'as const' to make it a literal type
          }));
          userState.searchResults = userResults;
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



select(entity: SearchType, item: any): void {
  const state = this.getState(entity);
  if (!Array.isArray(state.selected)) {
    state.selected = [];
  }
  // Only allow user selection (no groups)
  const idKey = "id";
  const idx = state.selected.findIndex(
    (u: any) => u.type === item.type && u[idKey] === item[idKey]
  );
  if (idx === -1) {
    state.selected.push(item);
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
  
  this.onCompareClick();
}


  clearSelected(entity: SearchType): void {
    const state = this.getState(entity);
    state.selected = [];
  }

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


onCompareClick() {
  const templateId = this.template.selected[0]?.id;
  let userId: string | undefined = undefined;
  
  if (this.student.selected.length > 0) {
    const selected = this.student.selected[0];
    if (selected.type === "user") {
      userId = (selected as User).id;
    }
  }
  
  if (templateId && userId) {
    const teacherId = 'f3229540-03d1-4f55-919c-feac1ebe0dba';
    this.getQuestionareOptions(templateId, userId, teacherId);
  }

  this.currentQuestionIndex = 0;
  this.updateChartForQuestion(this.currentQuestionIndex);
}


  private getQuestionareOptions(templateId: string, studentId: string, teacherId: string){
    // get all questions
    this.dataCompare.getQuestionareOptiontsByID(templateId).subscribe({
      next: (res) => {
        if (res?.questions?.length) {
          this.allQuestions = res.questions;
          this.totalQuestions = res.questions.length;
        }
        this.loadResponses(templateId, studentId, teacherId);
      },
    });
}

private loadResponses(templateId: string, studentId: string, teacherId: string) {
  this.dataCompare.getResponsesByID(studentId, teacherId, templateId).subscribe({
    next: (res) => {
      console.log('Raw API responses:', res);
      this.allResponses = Array.isArray(res) ? res : [];
      this.updateChartForQuestion(this.currentQuestionIndex);
    },
    error: (err) => console.error('Error fetching responses:', err),
  });
}


private updateChartForQuestion(index: number) {
  const question = this.allQuestions[index];
  console.log(' Updating chart for question:', question?.text ?? question); 

  if (!question) return;

  this.answers = (question.options ?? [])
    .map((opt: any) => opt.displayText)
    .filter((t: string) => !!t);
  console.log(' Possible answers:', this.answers);

  const toNumeric = (text: string) => {
    const idx = this.answers.indexOf(text);
    return idx !== -1 ? idx : null;
  };

  const filtered = this.allResponses
    .flatMap((entry) => {
      const match = entry.answers?.filter(
    (a: any) =>
    a.question === question.prompt ||
    a.question === question.title ||
    a.question === question.text
      );
      return match?.map((a: any) => {
        const completedDate = new Date(
          entry.student?.completedAt ?? entry.teacher?.completedAt ?? Date.now()
        );
        return {
          date: completedDate,
          dateLabel: completedDate.toLocaleString('da-DK', { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric',
          }),
          TeacherAnswer: toNumeric(a.teacherResponse),
          StudentAnswer: toNumeric(a.studentResponse),
        };
      });
    })
    .filter((x) => x && (x.TeacherAnswer !== null || x.StudentAnswer !== null))
    .sort((a, b) => a.date.getTime() - b.date.getTime()); // Sort by date

  // Add unique index to each point to ensure equal spacing
  const indexedData = filtered.map((item, idx) => ({
    ...item,
    index: idx,
    displayLabel: `${item.dateLabel} (#${idx + 1})`
  }));

  console.log(' Filtered chart data:', indexedData);

  this.options = {
    ...this.options,
    title: { text: question.prompt || question.title || question.text || 'Spørgsmål' },
    data: indexedData,
    series: [
      {
        type: 'line',
        xKey: 'index',
        yKey: 'TeacherAnswer',
        yName: 'Lærer',
        tooltip: {
          renderer: (params) => ({
            content: `${params.datum.dateLabel}<br/>Lærer: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
          }),
        },
      },
      {
        type: 'line',
        xKey: 'index',
        yKey: 'StudentAnswer',
        yName: 'Elev',
        tooltip: {
          renderer: (params) => ({
            content: `${params.datum.dateLabel}<br/>Elev: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
          }),
        },
      },
    ],
    axes: [
      {
        type: 'category',
        position: 'bottom',
        title: { text: 'Tidspunkt for besvarelse' },
        label: {
          rotation: 0,
          formatter: (params) => {
            const dataPoint = indexedData[params.value];
            return dataPoint ? dataPoint.dateLabel : '';
          }
        }
      },
      {
        type: 'number',
        position: 'left',
        min: 0,
        max: this.answers.length - 1,
        nice: false,
        label: {
          avoidCollisions: false,
          formatter: (p) => {
            const v = p.value;
            if (Math.abs(v - Math.round(v)) > 1e-6) return '';
            const idx = Math.round(v);
            const text = this.answers[idx] ?? '';
            return text.length > 25 ? text.substring(0, 22) + '...' : text;
          },
        },
        tick: {
          // @ts-expect-error: supported at runtime
          step: 1,
        },
        title: { text: 'Svarmuligheder' },
      },
    ],
  };
}

  public nextQuestion() {
    if (this.currentQuestionIndex < this.totalQuestions - 1) {
      this.currentQuestionIndex++;
      this.updateChartForQuestion(this.currentQuestionIndex);
    }
  }

  public prevQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
      this.updateChartForQuestion(this.currentQuestionIndex);
    }
  }


  private wrapText(text: string, maxLength: number): string {
    if (text.length <= maxLength) return text;
    const words = text.split(' ');
    const lines: string[] = [];
    let currentLine = '';
    words.forEach(word => {
      if ((currentLine + word).length <= maxLength) {
        currentLine += (currentLine ? ' ' : '') + word;
      } else {
        if (currentLine) lines.push(currentLine);
        currentLine = word;
      }
    });
    if (currentLine) lines.push(currentLine);
    
    return lines.join('\n');
  }
}
