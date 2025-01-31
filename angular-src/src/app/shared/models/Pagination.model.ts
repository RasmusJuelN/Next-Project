export interface PaginationResponse<T> {
    items: T[]; // The list of items for the current page
    totalItems: number; // Total number of items available
    currentPage: number; // The current page number
    pageSize: number; // Number of items per page
    totalPages: number; // Total number of pages
  }