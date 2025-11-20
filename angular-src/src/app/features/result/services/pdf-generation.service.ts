import { Injectable, inject } from '@angular/core';
import { Result } from '../../../shared/models/result.model';
import { TranslateService } from '@ngx-translate/core';
import * as pdfMake from 'pdfmake/build/pdfmake';

// Configure pdfMake with built-in fonts by importing the vfs_fonts
import 'pdfmake/build/vfs_fonts';

/**
 * Service for generating PDF reports from questionnaire results using PdfMake.
 * Uses i18n for localized strings - PDF strings are now located in the i18n files under the "PDF" key.
 */
@Injectable({
  providedIn: 'root'
})
export class PdfGenerationService {
  private readonly MAX_OPTIONS = 15;
  private readonly MAX_CUSTOM_WORDS = 500;
  private translate = inject(TranslateService);

  constructor() { }

  /**
   * Generates a PDF report from questionnaire results and downloads it.
   * Creates a matrix-style layout showing teacher and student responses.
   */
  generatePdf(result: Result): void {
    const docDefinition = this.buildDocDefinition(result);
    const filename = `questionnaire-matrix-${result.id}.pdf`;
    pdfMake.createPdf(docDefinition).download(filename);
  }

  /**
   * Generates a PDF and opens it in a new tab instead of downloading.
   */
  openPdf(result: Result): void {
    const docDefinition = this.buildDocDefinition(result);
    pdfMake.createPdf(docDefinition).open();
  }

  private buildDocDefinition(result: Result): any {
    const content = [
      this.createHeaderTable(result),
      this.createMetaNote(),
      ...this.createQuestionBlocks(result)
    ];

    return {
      pageOrientation: 'landscape' as const,
      pageMargins: [36, 60, 36, 54],
      header: (currentPage: number, pageCount: number) => this.createHeader(result, currentPage, pageCount),
      footer: (currentPage: number, pageCount: number) => this.createFooter(currentPage, pageCount),
      content,
      styles: { 
        hdr: { fontSize: 16, bold: true } 
      },
      defaultStyle: {
        fontSize: 10
      }
    };
  }

  private createHeaderTable(result: Result): any {
    return {
      table: {
        widths: ['*', '*'],
        body: [
          [
            { text: this.translate.instant('PDF.TABLE_TEMPLATE'), bold: true }, 
            `${result.title}  (ID: ${result.id})`
          ],
          [
            { text: this.translate.instant('PDF.TABLE_STUDENT'), bold: true }, 
            result.student.user.fullName
          ],
          [
            { text: this.translate.instant('PDF.TABLE_TEACHER'), bold: true }, 
            result.teacher.user.fullName
          ],
        ],
      },
      layout: 'lightHorizontalLines',
      margin: [0, 0, 0, 10],
    };
  }

  private createMetaNote(): any {
    return {
      text: this.translate.instant('PDF.META_NOTE'),
      fontSize: 9,
      color: '#475569',
      margin: [0, 0, 0, 8],
    };
  }

  private createHeader(result: Result, currentPage: number, pageCount: number): any {
    return {
      margin: [36, 24, 36, 0],
      columns: [
        { text: this.translate.instant('PDF.TITLE'), style: 'hdr' },
        {
          text: `${new Date(result.student.completedAt).toLocaleString()}  •  ${this.translate.instant('PDF.PAGE')} ${currentPage}/${pageCount}`,
          alignment: 'right',
          fontSize: 9,
          color: '#4b6baf',
        },
      ],
    };
  }

  private createFooter(currentPage: number, pageCount: number): any {
    return {
      margin: [36, 0, 36, 24],
      columns: [
        { 
          text: this.translate.instant('PDF.FOOTER_LEFT'), 
          fontSize: 8, 
          color: '#64748b' 
        },
        { 
          text: `${this.translate.instant('PDF.PAGE')} ${currentPage} / ${pageCount}`, 
          alignment: 'right', 
          fontSize: 9 
        },
      ],
    };
  }

  private createQuestionBlocks(result: Result): any[] {
    const blocks: any[] = [];
    
    result.answers.forEach((answer, index) => {
      blocks.push(...this.createQuestionBlock(answer, index));
    });

    return blocks;
  }

  private createQuestionBlock(answer: any, index: number): any[] {
    const options = answer.options?.slice(0, this.MAX_OPTIONS) || [];
    const teacherResponse = this.formatAnswer(answer.teacherResponse, answer.isTeacherResponseCustom);
    const studentResponse = this.formatAnswer(answer.studentResponse, answer.isStudentResponseCustom);

    const block: any[] = [
      {
        text: `${index + 1}. ${answer.question}`,
        margin: [0, 8, 0, 4],
        fontSize: 10,
      }
    ];

    if (options.length > 0) {
      // Check if both responses are custom - if so, skip the matrix table
      const bothCustom = teacherResponse.isCustom && studentResponse.isCustom;
      
      if (!bothCustom) {
        // Create matrix table for questions with options (only if at least one response is not custom)
        const matrixTable = this.createMatrixTable(options, teacherResponse, studentResponse);
        block.push(matrixTable);
      }
    } else {
      // No options available - show note
      block.push({
        table: { 
          widths: ['*'], 
          body: [[{ 
            text: this.translate.instant('PDF.NO_OPTIONS_NOTE'), 
            fontSize: 9, 
            color: '#475569' 
          }]] 
        },
        layout: 'lightHorizontalLines',
      });
    }

    // Add custom text responses if any
    const customResponses = this.createCustomResponsesSection(teacherResponse, studentResponse);
    if (customResponses.length > 0) {
      block.push({ stack: customResponses });
    }

    // Add spacing
    block.push({ text: ' ', margin: [0, 2, 0, 4] });

    return block;
  }

  private createMatrixTable(options: any[], teacherResponse: any, studentResponse: any): any {
    const header = [
      { text: ' ', fillColor: '#eef2ff' },
      ...options.map(option => ({
        text: option.displayText,
        fontSize: 8,
        color: '#1e293b',
        alignment: 'center',
        fillColor: '#eef2ff',
        margin: [2, 3, 2, 3],
      })),
    ];

    const body = [header];

    // Only add student row if response is not custom
    if (!studentResponse.isCustom) {
      const studentRow = this.createMatrixRow(
        this.translate.instant('PDF.STUDENT_ROW'), 
        studentResponse, 
        options,
        'student'
      );
      body.push(studentRow);
    }

    // Only add teacher row if response is not custom
    if (!teacherResponse.isCustom) {
      const teacherRow = this.createMatrixRow(
        this.translate.instant('PDF.TEACHER_ROW'), 
        teacherResponse, 
        options,
        'teacher'
      );
      body.push(teacherRow);
    }

    return {
      table: {
        headerRows: 1,
        widths: [90, ...Array(options.length).fill('*')],
        body: body,
      },
      layout: {
        fillColor: (row: number) => (row === 0 ? '#eef2ff' : null),
        hLineColor: '#cbd5e1',
        vLineColor: '#cbd5e1',
      },
    };
  }

  private createMatrixRow(label: string, response: any, options: any[], role: 'student' | 'teacher'): any[] {
    const cells = [{ text: label, bold: true }];
    
    options.forEach((option, index) => {
      const isMarked = !response.isCustom && this.isOptionSelected(response, option, index, role);
      cells.push(this.createCheckboxCell(isMarked));
    });

    return cells;
  }

  private createCheckboxCell(isMarked: boolean): any {
    return { 
      text: isMarked ? 'X' : '', 
      alignment: 'center', 
      bold: isMarked 
    };
  }

  private isOptionSelected(response: any, option: any, index: number, role: 'student' | 'teacher'): boolean {
    // If the option has selection information, use it directly based on role
    if (role === 'student' && option.isSelectedByStudent !== undefined) {
      return option.isSelectedByStudent;
    }
    if (role === 'teacher' && option.isSelectedByTeacher !== undefined) {
      return option.isSelectedByTeacher;
    }
    
    // Fallback to text matching for backward compatibility
    if (response.text === option.displayText) return true;
    if (response.text === option.optionValue?.toString()) return true;
    if (response.text === (index + 1).toString()) return true; // Match 1-based index
    if (response.text === index.toString()) return true; // Match 0-based index
    
    // Try exact text match (case insensitive)
    if (response.text?.toLowerCase() === option.displayText?.toLowerCase()) return true;
    
    return false;
  }

  private createCustomResponsesSection(teacherResponse: any, studentResponse: any): any[] {
    const customStack: any[] = [];

    if (studentResponse.isCustom) {
      customStack.push({
        margin: [0, 6, 0, 0],
        columns: [
          { 
            text: this.translate.instant('PDF.STUDENT_CUSTOM'), 
            width: 120, 
            italics: true, 
            color: '#6b7280' 
          },
          { 
            text: studentResponse.text, 
            width: '*', 
            fontSize: 9 
          },
        ],
      });
    }

    if (teacherResponse.isCustom) {
      customStack.push({
        margin: [0, 2, 0, 0],
        columns: [
          { 
            text: this.translate.instant('PDF.TEACHER_CUSTOM'), 
            width: 120, 
            italics: true, 
            color: '#6b7280' 
          },
          { 
            text: teacherResponse.text, 
            width: '*', 
            fontSize: 9 
          },
        ],
      });
    }

    return customStack;
  }

  private formatAnswer(response: string, isCustom: boolean): any {
    if (!response) {
      return { 
        option: null, 
        text: '—', 
        isCustom: false 
      };
    }

    if (isCustom) {
      return { 
        option: null, 
        text: this.capWords(response), 
        isCustom: true 
      };
    }

    return { 
      option: response, 
      text: String(response), 
      isCustom: false 
    };
  }

  private capWords(text: string, max: number = this.MAX_CUSTOM_WORDS): string {
    if (!text) return '';
    const words = text.trim().split(/\s+/);
    return words.length <= max ? text.trim() : words.slice(0, max).join(' ') + ' …';
  }
}
