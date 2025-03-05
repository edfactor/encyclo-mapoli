import { Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetDemographicBadgesNotInPayprofitColumns } from "./DemographicBadgesNotInPayprofitGridColumns";

interface DemographicBadgesNotInPayprofitGridSearchProps {
  profitYearCurrent: number | null;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const DemographicBadgesNotInPayprofitGrid: React.FC<DemographicBadgesNotInPayprofitGridSearchProps> = ({
  profitYearCurrent,
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  const { demographicBadges } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isLoading }] = useLazyGetDemographicBadgesNotInPayprofitQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: profitYearCurrent ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [profitYearCurrent, pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  const columnDefs = useMemo(() => GetDemographicBadgesNotInPayprofitColumns(), []);

  return (
    <>
      {demographicBadges?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`DEMOGRAPHICS BADGES NOT ON PAYPROFIT (${demographicBadges?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DEMO_BADGES"}
            isLoading={false}
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
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={demographicBadges.response.total}
        />
      )}
    </>
  );
};

export default DemographicBadgesNotInPayprofitGrid;
