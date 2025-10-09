import { Box, Chip, Typography } from "@mui/material";
import { formatNumberWithComma } from "smart-ui-library";
import { PagedReportResponse } from "../reduxstore/types";
import { mmDDYYFormat, mmYYFormat } from "../utils/dateUtils";
import EnvironmentUtils from "../utils/environmentUtils";

interface ReportSummaryProps<T> {
  report: PagedReportResponse<T>;
}

export const shouldShowDataSource = (): boolean => {
  return EnvironmentUtils.isDevelopmentOrQA;
};

function renderDateRangeLabel<T>(report: ReportSummaryProps<T>) {
  // At the moment, we only have one report that has a month date range.
  // So we handle that here, if this pattern becomes unwieldy. then refactor the things.
  if (report.reportName == "Distributions and Forfeitures") {
    return `Report range: ${mmYYFormat(report.startDate)} to ${mmYYFormat(report.endDate)}`;
  }
  return `Report range: ${mmDDYYFormat(report.startDate)} to ${mmDDYYFormat(report.endDate)}`;
}

export function ReportSummary<T>({ report }: ReportSummaryProps<T>) {
  return (
    <Box
      className="flex flex-wrap items-center gap-2"
      sx={{ padding: "0px 24px" }}>
      {report && (
        <Typography
          variant="h2"
          className="text-dsm-secondary">
          {`${report.reportName || ""} (${formatNumberWithComma(report.response.total) || 0})`}
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
