import { useMemo } from "react";
import { DSMGrid } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { AccountHistoryReportResponse, ReportResponseBase } from "../../../types/reports/AccountHistoryReportTypes";
import { GetAccountHistoryReportColumns } from "./AccountHistoryReportGridColumns";

interface AccountHistoryReportTableProps {
  data: ReportResponseBase<AccountHistoryReportResponse> | undefined;
  isLoading: boolean;
  error: unknown;
  showData: boolean;
}

const AccountHistoryReportTable: React.FC<AccountHistoryReportTableProps> = ({ data, isLoading, showData }) => {
  const gridMaxHeight = useDynamicGridHeight();
  const columnDefs = useMemo(() => GetAccountHistoryReportColumns(), []);

  return (
    <>
      {showData && data?.response && (
        <>
          <ReportSummary report={data} />
          <DSMGrid
            preferenceKey="Account_History_Report"
            isLoading={isLoading}
            maxHeight={gridMaxHeight}
            providedOptions={{
              rowData: data.response.results ?? [],
              columnDefs: columnDefs
            }}
          />
        </>
      )}
    </>
  );
};

export default AccountHistoryReportTable;
