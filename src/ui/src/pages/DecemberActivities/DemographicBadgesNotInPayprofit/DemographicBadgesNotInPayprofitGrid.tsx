import { Typography } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetDemographicBadgesNotInPayprofitColumns } from "./DemographicBadgesNotInPayprofitGridColumns";

const DemographicBadgesNotInPayprofitGrid: React.FC = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const { demographicBadges } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isLoading }] = useLazyGetDemographicBadgesNotInPayprofitQuery();

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const onSearch = useCallback(async () => {
    const request = {
      pagination: { skip: pageNumber * pageSize, take: pageSize, sort: "badgeNumber", isSortDescending: true }
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (hasToken) {
      onSearch();
    }
  }, [hasToken, pageNumber, pageSize, onSearch]);

  const columnDefs = useMemo(() => GetDemographicBadgesNotInPayprofitColumns(), []);

  return (
    <>
      {demographicBadges?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`(${demographicBadges?.response.total || 0} records)`}
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
