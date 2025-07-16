import { ISortParams } from "smart-ui-library";

const sortEventHandler = (
  update: ISortParams,
  sortParams: ISortParams,
  setSortParams: (value: React.SetStateAction<ISortParams>) => void,
  setPageNumber: (value: React.SetStateAction<number>) => void,
  invokeTrigger: () => void
) => {
  if (update.sortBy === "employeeName") {
    if (sortParams.sortBy === "lastName") {
      update.isSortDescending = !sortParams.isSortDescending;
    }
    update.sortBy = "lastName";
  }

  if (update.sortBy === "") {
    update.sortBy = "lastName";
    update.isSortDescending = false;
  }

  setSortParams(update);
  setPageNumber(0);
  invokeTrigger();
};

const pay426Utils = {
  sortEventHandler
};

export default pay426Utils;
