import { mmDDYYFormat, mmYYFormat } from "../utils/dateUtils";
import EnvironmentUtils from "../utils/environmentUtils";
import { PagedReportResponse } from "../reduxstore/types";

export const shouldShowDataSource = (): boolean => {
  return EnvironmentUtils.isDevelopmentOrQA;
};

export function renderDateRangeLabel<T>(report: PagedReportResponse<T>) {
  // At the moment, we only have one report that has a month date range.
  // So we handle that here, if this pattern becomes unwieldy. then refactor the things.
  if (report.reportName == "Distributions and Forfeitures") {
    return `Report range: ${mmYYFormat(report.startDate)} to ${mmYYFormat(report.endDate)}`;
  }
  return `Report range: ${mmDDYYFormat(report.startDate)} to ${mmDDYYFormat(report.endDate)}`;
}
