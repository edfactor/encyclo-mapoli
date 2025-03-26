import { Button, Stack, Typography } from "@mui/material";
import { GetNegativeEtvaForSSNsOnPayProfitColumns } from "pages/DecemberActivities/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitGridColumn";
import React, { useCallback, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Path, useNavigate } from "react-router";
import { setNegativeEtvaForSSNsOnPayprofit } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { NegativeEtvaForSSNsOnPayProfit, PagedReportResponse } from "reduxstore/types";
import { DSMGrid } from "smart-ui-library";

const staticData: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> = {
  reportName: "Negative ETVA",
  reportDate: new Date().toISOString(),
  response: {
    results: [
      {
        badgeNumber: 700127,
        ssn: 123456789,
        etvaValue: -500
      },
      {
        badgeNumber: 234567,
        ssn: 987654321,
        etvaValue: -750
      },
      {
        badgeNumber: 345678,
        ssn: 456789123,
        etvaValue: -1000
      },
      {
        badgeNumber: 456789,
        ssn: 789123456,
        etvaValue: -250
      },
      {
        badgeNumber: 567890,
        ssn: 321654987,
        etvaValue: -800
      }
    ],
    total: 5,
    totalPages: 1,
    pageSize: 10,
    currentPage: 1
  }
};

const STORAGE_KEY = "negative_etva_report";

const NegativeETVA: React.FC = () => {
  const dispatch = useDispatch();
  const negativeEtvaData = useSelector((state: RootState) => state.yearsEnd.negativeEtvaForSSNsOnPayprofit);

  useEffect(() => {
    const storedData = localStorage.getItem(STORAGE_KEY);
    if (storedData && !negativeEtvaData) {
      try {
        const parsedData = JSON.parse(storedData) as PagedReportResponse<NegativeEtvaForSSNsOnPayProfit>;
        dispatch(setNegativeEtvaForSSNsOnPayprofit(parsedData));
      } catch (error) {
        console.error("Error parsing stored ETVA data:", error);
        localStorage.removeItem(STORAGE_KEY);
      }
    }
  }, [dispatch, negativeEtvaData]);

  const handleRunReport = () => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(staticData));
    dispatch(setNegativeEtvaForSSNsOnPayprofit(staticData));
  };
  const navigate = useNavigate();
  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  return (
    <Stack spacing={2}>
      <Typography
        variant="h6"
        sx={{ color: "#0258A5", paddingLeft: "24px" }}>
        NEGATIVE ETVA FOR SSNs ON PAYPROFIT
      </Typography>

      {!negativeEtvaData ? (
        <Stack
          direction="row"
          spacing={2}
          paddingX="24px"
          alignItems="center">
          <Typography>Report not run yet</Typography>
          <Button
            variant="contained"
            onClick={handleRunReport}>
            Run Report
          </Button>
        </Stack>
      ) : (
        <DSMGrid
          preferenceKey={"DUPE_SSNS"}
          isLoading={false}
          handleSortChanged={() => {}}
          providedOptions={{
            rowData: negativeEtvaData.response.results,
            columnDefs: GetNegativeEtvaForSSNsOnPayProfitColumns(handleNavigationForButton)
          }}
        />
      )}
    </Stack>
  );
};

export default NegativeETVA;
