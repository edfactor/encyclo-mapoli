import { GridKeys } from "reduxstore/types";
import { getStoredSort } from "smart-ui-library";

interface SortableRequest {
  sortBy?: string;
  isSortDescending?: boolean;
  [key: string]: any;
}

export const retrievePageSorting = (
  gridKey: GridKeys, 
  request: SortableRequest, 
  defaultSortField?: string
) => {
  const { sortBy, isSortDescending } = getStoredSort(gridKey);

  return {
    ...request,
    sortBy: sortBy || defaultSortField || request.sortBy || '',
    isSortDescending: !sortBy ? !!request?.isSortDescending : isSortDescending
  };
}