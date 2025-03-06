import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumn";

interface ProfitShareReportRow {
  badgeNumber: string;
  fullName: string;
  oracleHcmId: string;
}


const ProfitShareReportGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  const {yearEndProfitSharingReport} = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);
  
  return (
    <>
      {!!yearEndProfitSharingReport && (
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT-ELIGIBLE REPORT (5)`}
            </Typography>
          </div>
      )}
      {!!yearEndProfitSharingReport && yearEndProfitSharingReport.response.results.length && (
          <DSMGrid
            preferenceKey={"ProfitShareReportGrid"}
            isLoading={isLoading}
            providedOptions={{
                rowData: yearEndProfitSharingReport?.response.results,
                columnDefs: columnDefs
            }}
        />
      )}
    </>
  );
};

export default ProfitShareReportGrid;
