import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { Path, useNavigate } from "react-router";
import { DSMGrid } from "smart-ui-library";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

const _sampleData = [
  {
    badgeNumber: 47425,
    employeeName: "BACHELDER, JAKE R",
    ssn: "***-**-7425",
    forfeitures: 5000.0,
    contForfeitPoints: 565,
    earningsPoints: 317,
    benNumber: "12345"
  },
  {
    badgeNumber: 82424,
    employeeName: "BATISTA, STEVEN",
    ssn: "***-**-2424",
    forfeitures: 2500.0,
    contForfeitPoints: 23,
    earningsPoints: 23,
    benNumber: "12346"
  }
];

const totalsRow = {
  forfeitures: "0.00",
  contForfeitPoints: 0,
  earningsPoints: 0
};

const ForfeitGrid = () => {
  const navigate = useNavigate();
  const { forfeituresAndPoints } = useSelector((state: RootState) => state.yearsEnd);
  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetProfitShareForfeitColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  return (
    <>
      {forfeituresAndPoints?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT SHARE FORFEIT [PAY443] (${forfeituresAndPoints?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"PROFIT_SHARE_FORFEIT"}
            isLoading={false}
            handleSortChanged={(_params) => {}}
            providedOptions={{
              rowData: forfeituresAndPoints?.response.results,
              pinnedBottomRowData: [totalsRow],
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      ;
    </>
  );
};

export default ForfeitGrid;
