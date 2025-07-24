export type ISortParams = {
  sortBy: string;
  isSortDescending: boolean;
};

export const SortDefault: ISortParams = {
  sortBy: "",
  isSortDescending: false
};

export type Paged<T> = {
  total: number;
  totalPages: number;
  pageSize: number;
  currentPage: number;
  results: T[];
};

export type PaginationParams = {
  skip: number;
  take: number;
};

export type BuildInfoData = {
  versionNumber: string;
  buildNumber: string;
  planName: string;
  planRepository: string;
};
