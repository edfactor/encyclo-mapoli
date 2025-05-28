import React from "react";
import { Typography } from "@mui/material";
import { PagedReportResponse } from "../reduxstore/types";
import { mmDDYYFormat } from "../utils/dateUtils";

interface ReportSummaryProps<T> {
  report: PagedReportResponse<T>;
}

export function ReportSummary<T>({ report }: ReportSummaryProps<T>) {
  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography variant="h2" sx={{ color: "#0258A5" }}>
          {`${report.reportName || ""} (${report.response.total || 0} ${
            report.response.total === 1 ? "Record" : "Records"
          })`}
        </Typography>
        <Typography sx={{ color: "#0258A5" }}>
          {`Report Range: ${mmDDYYFormat(report.startDate)} - ${mmDDYYFormat(report.endDate)} || Data Source: ${
            report.dataSource
          }`}
        </Typography>
      </div>
    </>
  );
}

export default ReportSummary;