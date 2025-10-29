import { useMemo } from "react";
import { DSMGrid } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { useDynamicGridHeight } from "../../../hooks/useDynamicGridHeight";
import { DivorceReportResponse, ReportResponseBase } from "../../../types/reports/DivorceReportTypes";
import { GetDivorceReportColumns } from "./DivorceReportGridColumns";

interface DivorceReportTableProps {
  data: ReportResponseBase<DivorceReportResponse> | undefined;
  isLoading: boolean;
  error: unknown;
  showData: boolean;
}

const DivorceReportTable: React.FC<DivorceReportTableProps> = ({ data, isLoading, showData }) => {
  const gridMaxHeight = useDynamicGridHeight();
  const columnDefs = useMemo(() => GetDivorceReportColumns(), []);

  return (
    <>
      {showData && data?.response && (
        <>
          <ReportSummary report={data} />
          <DSMGrid
            preferenceKey="DIVORCE_REPORT"
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

export default DivorceReportTable;
