import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Result } from '../models/result.model';

export interface ShowResultConfig {
  showTemplate?: boolean;
  showStudent?: boolean;
  showTeacher?: boolean;
  showCompletionDates?: boolean;
  customHeaderTitle?: string;
  hideTemplateName?: boolean; // When true, hides the template title and ID
  showActions?: boolean; // When true, shows action buttons
  useCardStyling?: boolean; // When true, shows full card styling (background, shadow, etc.)
}

@Component({
  selector: 'app-show-result',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './show-result.component.html',
  styleUrl: './show-result.component.css'
})
export class ShowResultComponent {
  @Input() result: Result | null = null;
  @Input() isFullView: boolean = false;
  @Input() config: ShowResultConfig = {};

  constructor(public translate: TranslateService) {}

  getTemplateQuestionOptions(questionPrompt: string): any[] {
    if (!this.result) return [];
    const answer = this.result.answers.find(a => a.question === questionPrompt);
    return answer?.options?.slice(0, 15) || [];
  }

  isOptionSelected(response: string, isCustom: boolean, option: any, index: number, role?: 'student' | 'teacher'): boolean {
    if (isCustom || !response) return false;
    
    // If the option has selection information and we know the role, use it directly
    if (role === 'student' && option.isSelectedByStudent !== undefined) {
      return option.isSelectedByStudent;
    }
    if (role === 'teacher' && option.isSelectedByTeacher !== undefined) {
      return option.isSelectedByTeacher;
    }
    
    // Fallback to text matching for backward compatibility
    if (response === option.displayText) return true;
    if (response === option.optionValue?.toString()) return true;
    if (response === (index + 1).toString()) return true; // Match 1-based index
    if (response === index.toString()) return true; // Match 0-based index
    
    // Try exact text match (case insensitive)
    if (response?.toLowerCase() === option.displayText?.toLowerCase()) return true;
    
    return false;
  }
}
