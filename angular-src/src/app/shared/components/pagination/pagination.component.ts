
import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface PageChangeEvent {
  page: number;
  direction: 'forward' | 'backward' | 'jump';
}

@Component({
    selector: 'app-pagination',
    imports: [],
    templateUrl: './pagination.component.html',
    styleUrls: ['./pagination.component.css']
})
export class PaginationComponent {
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 1;
  // New input to control whether page numbers are shown.
  @Input() showPageNumbers: boolean = true;
  @Output() pageChange = new EventEmitter<PageChangeEvent>();

  get pages(): number[] {
    const maxVisiblePages = 5;
    const pages: number[] = [];

    if (this.totalPages <= maxVisiblePages) {
      return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }

    const startPage = Math.max(1, this.currentPage - Math.floor(maxVisiblePages / 2));
    const endPage = Math.min(this.totalPages, startPage + maxVisiblePages - 1);

    if (startPage > 1) {
      pages.push(1);
      if (startPage > 2) {
        pages.push(-1);
      }
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    if (endPage < this.totalPages) {
      if (endPage < this.totalPages - 1) {
        pages.push(-1);
      }
      pages.push(this.totalPages);
    }

    return pages;
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      const newPage = this.currentPage - 1;
      this.pageChange.emit({ page: newPage, direction: 'backward' });
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      const newPage = this.currentPage + 1;
      this.pageChange.emit({ page: newPage, direction: 'forward' });
    }
  }

  jumpToPage(page: number): void {
    if (page !== this.currentPage && page > 0 && page <= this.totalPages) {
      this.pageChange.emit({ page, direction: 'jump' });
    }
  }
}
