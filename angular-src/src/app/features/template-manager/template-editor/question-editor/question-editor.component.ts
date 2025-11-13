import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from "@angular/core";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { Question, Option } from "../../../../shared/models/template.model";
import { TranslateModule } from "@ngx-translate/core";
import {
  DragDropModule,
  CdkDragDrop,
  moveItemInArray,
} from "@angular/cdk/drag-drop";

/**
 * Question editor component.
 *
 * Provides an interface for editing a single question within a template.
 *
 * Handles:
 * - Editing the question prompt and options.
 * - Adding and deleting options.
 * - Validating required fields before save.
 * - Emitting save or cancel events back to the parent.
 * - Disabling edits when in readonly mode.
 */
@Component({
  selector: "app-question-editor",
  standalone: true,
  imports: [FormsModule, CommonModule, TranslateModule, DragDropModule],
  templateUrl: "./question-editor.component.html",
  styleUrls: ["./question-editor.component.css"],
})
export class QuestionEditorComponent {
  @Input() question!: Question;

  /** If true, disables all editing actions. */
  @Input() readonly = false;

  @Output() save = new EventEmitter<Question>();
  @Output() cancel = new EventEmitter<void>();

  validationErrors: string[] = [];
  questionOptionsMaxCount: number = 10;

  /** Reference to error message container for scrolling into view. */
  @ViewChild("errorContainer") errorContainer!: ElementRef;

  addOption(): void {
    if (this.readonly) {
      return;
    }
    // if over 10 options then do not add more
    if (this.question.options.length >= this.questionOptionsMaxCount) {
      return;
    }
    const newOption: Option = {
      id: -1 * (this.question.options.length + 1),
      displayText: "New Option",
      optionValue: 0,
    };
    this.question.options.push(newOption);
  }

  deleteOption(optionId: number): void {
    if (this.readonly) {
      return;
    }
    this.question.options = this.question.options.filter(
      (option) => option.id !== optionId
    );
  }

  /**
   * Validates the current question.
   * - Prompt must not be empty.
   * - Must allow custom answers or have at least one option.
   * - All options must have non-empty labels.
   *
   * @returns `true` if valid, else `false`.
   */
  validateQuestion(): boolean {
    console.log(this.question);
    this.validationErrors = []; // Clear previous errors

    // 1. Ensure the title is not empty
    if (!this.question.prompt || this.question.prompt.trim() === "") {
      this.validationErrors.push("The question title cannot be empty.");
    }

    // 2. Ensure there are options or a custom answer is allowed
    if (
      !this.question.allowCustom &&
      (!this.question.options || this.question.options.length === 0)
    ) {
      this.validationErrors.push(
        "The question must allow a custom answer or have at least one option."
      );
    }

    // If question options are over 10 in length then add a validation error
    if (this.question.options.length > this.questionOptionsMaxCount) {
      this.validationErrors.push(
        `The question cannot have more than ${this.questionOptionsMaxCount} options.`
      );
    }

    // 3. Ensure options have non-empty labels
    const hasEmptyLabel = this.question.options.some(
      (option) => option.displayText.trim() === ""
    );
    if (hasEmptyLabel) {
      this.validationErrors.push("All options must have non-empty labels.");
    }

    return this.validationErrors.length === 0; // Return true if no errors
  }

  onSave(): void {
    if (this.readonly) {
      return;
    }
    if (this.validateQuestion()) {
      this.save.emit(this.question);
    } else {
      // Delay scrolling to allow the error container to render or else requries clicking twice
      setTimeout(() => {
        if (this.errorContainer) {
          this.errorContainer.nativeElement.scrollIntoView({
            behavior: "smooth",
            block: "start",
          });
        }
      }, 0);
    }
  }

  // Emit cancel event
  onCancel(): void {
    this.cancel.emit();
  }

  // Helper functions
  overOptionCount(): boolean {
    return this.question.options.length >= this.questionOptionsMaxCount;
  }

  // Handle drop for reordering options
  dropOption(event: CdkDragDrop<Option[]>) {
    moveItemInArray(
      this.question.options,
      event.previousIndex,
      event.currentIndex
    );
  }
}
