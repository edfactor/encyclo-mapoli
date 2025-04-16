import { useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumn";

interface ProfitShareReportGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const ProfitShareReportGrid: React.FC<ProfitShareReportGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { yearEndProfitSharingReport, yearEndProfitSharingReportQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isLoading }] = useLazyGetYearEndProfitSharingReportQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);

  return (
    <>
      {!!yearEndProfitSharingReport && (
        <DSMGrid
          preferenceKey={"ProfitShareReportGrid"}
          isLoading={isLoading}
          providedOptions={{
            rowData: yearEndProfitSharingReport?.response.results,
            columnDefs: columnDefs
          }}
        />
      )}
      {!!yearEndProfitSharingReport && yearEndProfitSharingReport.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={yearEndProfitSharingReport.response.total}
        />
      )}
    </>
  );
};

export default ProfitShareReportGrid;
