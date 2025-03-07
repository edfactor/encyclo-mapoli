import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router";
import { useLazyGetTerminationReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetTerminationColumns } from "./TerminationGridColumn";

interface TerminationGridSearchProps {
  profitYearCurrent: number | null;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const TerminationGrid: React.FC<TerminationGridSearchProps> = ({
  profitYearCurrent,
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
  const navigate = useNavigate();

  const onSearch = useCallback(async () => {

    if (!profitYearCurrent) {
      console.error("profitYearCurrent is missing or invalid, defaulting to 0");
    }

    const request = {
      profitYear: profitYearCurrent ?? 0,
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [profitYearCurrent, pageNumber, pageSize, triggerSearch]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  // Wrapper to pass react function to non-react class
  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(() => GetTerminationColumns(handleNavigationForButton), [handleNavigationForButton]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  return (
    <>
      {termination?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`TERMINATIONS REPORT (${termination.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: termination?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!termination && termination.response.results.length > 0 && (
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
          recordCount={termination.response.total}
        />
      )}
    </>
  );
};

export default TerminationGrid;
