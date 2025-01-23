import { Button, Link, Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetEligibleEmployeesQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetTerminationColumns } from "./TerminationGridColumn";
import { ICellRendererParams } from "ag-grid-community";
import { useNavigate } from "react-router";

const TerminationGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  const { terminattion } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetEligibleEmployeesQuery();
  const navigate = useNavigate();

  const viewBadge = (params: ICellRendererParams) => {
    return (
      params.value && (
        <Button
          variant="text"
          onClick={() => navigate(`/forfeit/${params.value}`)}
        >
          {params.value}
        </Button>
      )
    );
  };

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetTerminationColumns(viewBadge), []);

  return (
    <>
      {terminattion?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT-TERM REPORT (${terminattion.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"TERM"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: terminattion?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!terminattion && terminattion.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={terminattion.response.total}
        />
      )}
    </>
  );
};

export default TerminationGrid;
