import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetForfeituresByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { GetForfeituresByAgeColumns } from "./ForfeituresByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";

const ForfeituresByAgeGrid = () => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { forfeituresByAgeTotal, forfeituresByAgeFullTime, forfeituresByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [_discard1, { isLoading }] = useLazyGetForfeituresByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  return (
    <>
      {forfeituresByAgeTotal?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${forfeituresByAgeTotal.reportName}`}
            </Typography>
          </div>
          <Grid2
            container
            xs={12}>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_Total"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: forfeituresByAgeTotal?.response.results,
                  pinnedTopRowData: [
                    {
                      age: "FORF  TTL",
                      employeeCount: (forfeituresByAgeTotal?.totalEmployees || 0),
                      amount: forfeituresByAgeTotal?.distributionTotalAmount
                    }
                  ],
                  columnDefs: [
                    {
                      headerName: columnDefsTotal.headerName,
                      children: columnDefsTotal.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: forfeituresByAgeFullTime?.response.results,
                  pinnedTopRowData: [
                    {
                     age: "FORF  TTL",
                      employeeCount: (forfeituresByAgeFullTime?.totalEmployees || 0),
                      amount: forfeituresByAgeFullTime?.distributionTotalAmount
                    }
                  ],
                  columnDefs: [
                    {
                      headerName: columnDefsFullTime.headerName,
                      children: columnDefsFullTime.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: forfeituresByAgePartTime?.response.results,
                  pinnedTopRowData: [
                    {
                     age: "FORF  TTL",
                      employeeCount: (forfeituresByAgePartTime?.totalEmployees || 0),
                      amount: forfeituresByAgePartTime?.distributionTotalAmount
                    }
                  ],
                  columnDefs: [
                    {
                      headerName: columnDefsPartTime.headerName,
                      children: columnDefsPartTime.children
                    }
                  ]
                }}
              />
            </Grid2>
          </Grid2>
        </>
      )}
    </>
  );
};

export default ForfeituresByAgeGrid;
