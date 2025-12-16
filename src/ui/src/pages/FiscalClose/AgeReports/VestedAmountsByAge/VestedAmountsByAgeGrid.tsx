import { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid } from "smart-ui-library";
import ReportSummary from "../../../../components/ReportSummary";
import { GRID_KEYS } from "../../../../constants";
import { GetVestedAmountsByAgeColumns } from "./VestedAmountsByAgeGridColumns";

interface VestedAmountsByAgeGridProps {
  gridTitle: string;
  countColName: string;
  amountColName: string;
}

const VestedAmountsByAgeGrid: React.FC<VestedAmountsByAgeGridProps> = ({ gridTitle, amountColName, countColName }) => {
  const { vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);

  const columnDefs = useMemo(
    () => GetVestedAmountsByAgeColumns(countColName, amountColName),
    [countColName, amountColName]
  );

  return (
    <>
      {vestedAmountsByAge?.response && (
        <>
          <ReportSummary report={vestedAmountsByAge} />
          <DSMGrid
            preferenceKey={`${GRID_KEYS.VESTED_AMOUNTS_PREFIX}${gridTitle}`}
            isLoading={false}
            providedOptions={{
              rowData: vestedAmountsByAge?.response.results,
              columnDefs: columnDefs,
              suppressHorizontalScroll: true,
              suppressColumnVirtualisation: true
            }}
          />
        </>
      )}
    </>
  );
};

export default VestedAmountsByAgeGrid;
