import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

/**
 * ModalComponent
 *
 * A reusable confirmation modal with a title, text, and configurable confirm/cancel buttons.
 *
 * Features:
 * - Opens when `isOpen` is `true`.
 * - Accepts customizable title, message text, and button labels.
 * - Emits `confirm` or `cancel` events for parent components to handle actions.
 *
 * @example
 * ```html
 * <app-modal
 *   [isOpen]="showDeleteModal"
 *   title="Confirm delete"
 *   text="Are you sure you want to delete this item?"
 *   confirmText="Delete"
 *   cancelText="Cancel"
 *   (confirm)="handleDelete()"
 *   (cancel)="showDeleteModal = false">
 * </app-modal>
 * ```
 */
@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.css'
})
export class ModalComponent {
  /** Modal title displayed at the top. */
  @Input() title: string = '';

  /** Message text shown inside the modal (supports HTML). */
  @Input() text: string = '';

  /** Whether the modal is currently open. */
  @Input() isOpen: boolean = false;

  /** Label for the confirm button. */
  @Input() confirmText: string = 'Yes';

  /** Label for the cancel button. */
  @Input() cancelText: string = 'No';

  /** Emits when the user confirms the action. */
  @Output() confirm = new EventEmitter<void>();

  /** Emits when the user cancels the action. */
  @Output() cancel = new EventEmitter<void>();

  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }
}