import { Box, Chip, Typography } from "@mui/material";
import { formatNumberWithComma } from "smart-ui-library";
import { PagedReportResponse } from "../reduxstore/types";
import { renderDateRangeLabel, shouldShowDataSource } from "./ReportSummaryUtils";

interface ReportSummaryProps<T> {
  report: PagedReportResponse<T>;
  variant?: "default" | "compact";
}

function ReportSummary<T>({ report, variant = "default" }: ReportSummaryProps<T>) {
  const isCompact = variant === "compact";
  const reportTitle = `${report.reportName || ""} (${formatNumberWithComma(report.response.total) || 0})`;
  return (
    <Box
      className="flex flex-wrap items-center gap-2"
      sx={{ padding: "0px 24px" }}>
      {report && (
        <Typography
          variant={isCompact ? "subtitle1" : "h2"}
          className="text-dsm-secondary"
          sx={isCompact ? { fontWeight: 500 } : undefined}>
          {isCompact ? `Report: ${reportTitle}` : reportTitle}
        </Typography>
      )}
      <Box className="flex flex-wrap gap-1">
        <Chip
          label={renderDateRangeLabel(report)}
          className="bg-dsm-grey-hover"
        />
        {shouldShowDataSource() && (
          <Chip
            label={`Data Source: ${report.dataSource}`}
            className="bg-dsm-blue"
          />
        )}
      </Box>
    </Box>
  );
}

export default ReportSummary;
