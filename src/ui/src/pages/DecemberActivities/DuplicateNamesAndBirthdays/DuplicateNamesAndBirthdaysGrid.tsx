import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDuplicateNamesAndBirthdaysQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { CAPTIONS } from "../../../constants";
import { GetDuplicateNamesAndBirthdayColumns } from "./DuplicateNamesAndBirthdaysGridColumns";

interface DuplicateNamesAndBirthdaysGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const DuplicateNamesAndBirthdaysGrid: React.FC<DuplicateNamesAndBirthdaysGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "name",
    isSortDescending: false
  });

  const { duplicateNamesAndBirthdays } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetDuplicateNamesAndBirthdaysQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYear || 0,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending,
        profitYear: profitYear
      }
    };

    await triggerSearch(request, false);
  }, [profitYear, pageNumber, pageSize, sortParams, triggerSearch]);

  useEffect(() => {
    if (hasToken && (!initialSearchLoaded || pageNumber || pageSize !== 25 || sortParams)) {
      onSearch();
      if (!initialSearchLoaded) {
        setInitialSearchLoaded(true);
      }
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken, setInitialSearchLoaded]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetDuplicateNamesAndBirthdayColumns(), []);

  return (
    <>
      {duplicateNamesAndBirthdays?.response && (
        <>
          <ReportSummary report={duplicateNamesAndBirthdays} />
          <DSMGrid
            preferenceKey={CAPTIONS.DUPLICATE_NAMES}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: duplicateNamesAndBirthdays?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!duplicateNamesAndBirthdays && duplicateNamesAndBirthdays.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={duplicateNamesAndBirthdays.response.total}
        />
      )}
    </>
  );
};

export default DuplicateNamesAndBirthdaysGrid;
