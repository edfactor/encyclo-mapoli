import React, { useEffect } from "react";
import { Stack, Typography, Button } from "@mui/material";
import { AgGridReact } from "ag-grid-react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { PagedReportResponse, NegativeEtvaForSSNsOnPayProfit } from "reduxstore/types";
import { GetNegativeEtvaForSSNsOnPayProfitColumns } from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitGridColumn";
import { setNegativeEtvaForSSNsOnPayprofit } from "reduxstore/slices/yearsEndSlice";
import { DSMGrid } from "smart-ui-library";

const staticData: PagedReportResponse<NegativeEtvaForSSNsOnPayProfit> = {
  reportName: "Negative ETVA",
  reportDate: new Date().toISOString(),
  response: {
    results: [
      {
        employeeBadge: 700127,
        employeeSsn: 123456789,
        etvaValue: -500
      },
      {
        employeeBadge: 234567,
        employeeSsn: 987654321,
        etvaValue: -750
      },
      {
        employeeBadge: 345678,
        employeeSsn: 456789123,
        etvaValue: -1000
      },
      {
        employeeBadge: 456789,
        employeeSsn: 789123456,
        etvaValue: -250
      },
      {
        employeeBadge: 567890,
        employeeSsn: 321654987,
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
            columnDefs: GetNegativeEtvaForSSNsOnPayProfitColumns()
          }}
        />
      )}
    </Stack>
  );
};

export default NegativeETVA;
