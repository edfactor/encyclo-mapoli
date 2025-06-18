import React from "react";
import { Typography } from "@mui/material";
import { PagedReportResponse } from "../reduxstore/types";
import { mmDDYYFormat } from "../utils/dateUtils";
import EnvironmentUtils from "../utils/environmentUtils";
import { formatNumberWithComma } from "smart-ui-library";


interface ReportSummaryProps<T> {
  report: PagedReportResponse<T>;
}

export const shouldShowDataSource = (): boolean => {
  return EnvironmentUtils.isDevelopmentOrQA;
};

export function ReportSummary<T>({ report }: ReportSummaryProps<T>) {
  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography variant="h2" sx={{ color: "#0258A5" }}>
          {`${report.reportName || ""} (${formatNumberWithComma(report.response.total) || 0} ${
            report.response.total === 1 ? "Record" : "Records"
          })`}
        </Typography>
        <Typography sx={{ color: "#0258A5" }}>
          {`Report Range: ${mmDDYYFormat(report.startDate)} - ${mmDDYYFormat(report.endDate)}`}
          {shouldShowDataSource() && ` || Data Source: ${report.dataSource}`}
        </Typography>

      </div>
    </>
  );
}

export default ReportSummary;