/**
 * Generic response model for paginated API results.
 */
export interface PaginationResponse<T> {
  /** The list of items for the current page. */
  items: T[];

  /** Total number of items available across all pages. */
  totalItems: number;

  /** The current page number (1-based). */
  currentPage: number;

  /** Number of items per page. */
  pageSize: number;

  /** Total number of pages (calculated from `totalItems` / `pageSize`). */
  totalPages: number;
}
