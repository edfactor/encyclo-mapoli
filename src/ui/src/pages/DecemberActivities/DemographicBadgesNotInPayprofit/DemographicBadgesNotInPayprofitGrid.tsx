import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { GetDemographicBadgesNotInPayprofitColumns } from "./DemographicBadgesNotInPayprofitGridColumns";

const DemographicBadgesNotInPayprofitGrid: React.FC = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: true
  });
  const { demographicBadges } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetDemographicBadgesNotInPayprofitQuery();
  const profitYear = useDecemberFlowProfitYear();

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const onSearch = useCallback(async () => {
    const request = {
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: _sortParams.sortBy,
        isSortDescending: _sortParams.isSortDescending
      },
      profitYear: profitYear
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, _sortParams, triggerSearch]);

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "badgeNumber";
      update.isSortDescending = true;
    }

    const request = {
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: update.sortBy,
        isSortDescending: update.isSortDescending
      },
      profitYear: profitYear
    };
    setSortParams(update);
    setPageNumber(0);

    triggerSearch(request, false);
  };

  useEffect(() => {
    if (hasToken) {
      onSearch();
    }
  }, [hasToken, pageNumber, pageSize, _sortParams, onSearch]);

  const columnDefs = useMemo(() => GetDemographicBadgesNotInPayprofitColumns(), []);

  return (
    <>
      {demographicBadges?.response && (
        <>
          <ReportSummary report={demographicBadges} />
          <DSMGrid
            preferenceKey={"DEMO_BADGES"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: demographicBadges?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {demographicBadges?.response && demographicBadges.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={demographicBadges.response.total}
        />
      )}
    </>
  );
};

export default DemographicBadgesNotInPayprofitGrid;
