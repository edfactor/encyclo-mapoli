import { Typography } from "@mui/material";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { Path, useNavigate } from "react-router";
import { GetProfitShareGrossReportColumns } from "./ProfitShareGrossReportColumns";
import { useLazyGetGrossWagesReportQuery } from "reduxstore/api/YearsEndApi";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { GrossWagesReportDto } from "reduxstore/types";

const totalsRow = {
  psWages: 0,
  psAmount: 0,
  loans: 0,
  forfeitures: 0
};

interface ProfitShareGrossReportGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const ProfitShareGrossReportGrid: React.FC<ProfitShareGrossReportGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { grossWagesReport, grossWagesReportQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isLoading }] = useLazyGetGrossWagesReportQuery();
  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const navigate = useNavigate();

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(
    () => GetProfitShareGrossReportColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const onSearch = useCallback(async () => {
    const request: GrossWagesReportDto = {
      profitYear: grossWagesReportQueryParams?.profitYear ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, triggerSearch, grossWagesReportQueryParams?.profitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  return (
    <>
      {!!grossWagesReport && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT SHARE GROSS REPORT (QPAY501) (${grossWagesReport?.response.results.length || 0} ${grossWagesReport?.response.results.length === 1 ? 'Record' : 'Records'})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"PROFIT_SHARE_GROSS_REPORT"}
            isLoading={false}
            handleSortChanged={(_params) => { }}
            providedOptions={{
              rowData: grossWagesReport?.response.results,
              pinnedBottomRowData: [
                {
                  grossWages: grossWagesReport?.totalGrossWages,
                  profitSharingAmount: grossWagesReport?.totalProfitSharingAmount,
                  loans: grossWagesReport?.totalLoans,
                  forfeitures: grossWagesReport?.totalForfeitures,
                }],
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!grossWagesReport && grossWagesReport.response.results.length && (
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
          recordCount={grossWagesReport.response.total}
        />
      )}
    </>
  );
};

export default ProfitShareGrossReportGrid;
