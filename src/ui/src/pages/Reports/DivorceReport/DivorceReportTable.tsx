import {
  Box,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { DivorceReportResponse, ReportResponseBase } from "../../../types/reports/DivorceReportTypes";

interface DivorceReportTableProps {
  data: ReportResponseBase<DivorceReportResponse> | undefined;
  isLoading: boolean;
  error: Error | undefined;
  onLoadingChange?: (isLoading: boolean) => void;
}

const DivorceReportTable: React.FC<DivorceReportTableProps> = ({ data, isLoading, error, onLoadingChange }) => {
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const gridMaxHeight = useDynamicGridHeight();

  useEffect(() => {
    onLoadingChange?.(isLoading);
  }, [isLoading, onLoadingChange]);

  useEffect(() => {
    if (error) {
      console.error("DivorceReport API error:", error);
      setErrorMessage("Failed to load divorce report data. Please try again.");
    } else if (!isLoading && data) {
      setErrorMessage(null);
    }
  }, [error, isLoading, data]);

  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "400px" }}>
        <CircularProgress />
      </Box>
    );
  }

  if (errorMessage) {
    return (
      <Box sx={{ padding: "24px" }}>
        <Typography color="error">{errorMessage}</Typography>
      </Box>
    );
  }

  const rows = data?.response?.results || [];

  if (rows.length === 0) {
    return (
      <Box sx={{ padding: "24px" }}>
        <Typography>No divorce report data found.</Typography>
      </Box>
    );
  }

  const formatCurrency = (value: number): string => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value);
  };

  const formatSSN = (ssn: string): string => {
    // SSN should already be masked from backend, but ensure format
    return ssn || "***-**-****";
  };

  return (
    <TableContainer
      component={Paper}
      sx={{
        maxHeight: gridMaxHeight,
        overflow: "auto",
        margin: "24px"
      }}>
      <Table stickyHeader>
        <TableHead sx={{ backgroundColor: "#f5f5f5" }}>
          <TableRow>
            <TableCell sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>Badge Number</TableCell>
            <TableCell sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>Full Name</TableCell>
            <TableCell sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>SSN</TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Profit Year
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Total Contributions
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Total Withdrawals
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Total Distributions
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Total Dividends
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Total Forfeitures
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Ending Balance
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Cumulative Contributions
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Cumulative Withdrawals
            </TableCell>
            <TableCell
              align="right"
              sx={{ fontWeight: "bold", backgroundColor: "#f5f5f5" }}>
              Cumulative Distributions
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {rows.map((row: DivorceReportResponse, index: number) => (
            <TableRow
              key={index}
              hover>
              <TableCell>{row.badgeNumber}</TableCell>
              <TableCell>{row.fullName}</TableCell>
              <TableCell>{formatSSN(row.ssn)}</TableCell>
              <TableCell align="right">{row.profitYear}</TableCell>
              <TableCell align="right">{formatCurrency(row.totalContributions)}</TableCell>
              <TableCell align="right">{formatCurrency(row.totalWithdrawals)}</TableCell>
              <TableCell align="right">{formatCurrency(row.totalDistributions)}</TableCell>
              <TableCell align="right">{formatCurrency(row.totalDividends)}</TableCell>
              <TableCell align="right">{formatCurrency(row.totalForfeitures)}</TableCell>
              <TableCell align="right">{formatCurrency(row.endingBalance)}</TableCell>
              <TableCell align="right">{formatCurrency(row.cumulativeContributions)}</TableCell>
              <TableCell align="right">{formatCurrency(row.cumulativeWithdrawals)}</TableCell>
              <TableCell align="right">{formatCurrency(row.cumulativeDistributions)}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default DivorceReportTable;
