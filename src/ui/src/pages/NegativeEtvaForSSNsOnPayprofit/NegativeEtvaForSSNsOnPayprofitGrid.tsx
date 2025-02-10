import { Link, Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetNegativeEtvaForSSNsOnPayProfitColumns } from "./NegativeEtvaForSSNsOnPayprofitGridColumn";
import { NegativeEtvaForSSNsOnPayProfit } from "reduxstore/types";
import { ICellRendererParams } from "ag-grid-community";

const NegativeEtvaForSSNsOnPayprofitGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });
  
  const viewBadge = ({ value }: ICellRendererParams) => {
    return (
      value && (
        <Link
          style={{ height: "20px", textDecoration: "underline", textTransform: "none" }}
          href={`/master-inquiry/${value}`}
        >
          {value}
        </Link>
      )
    );
  };

  const dispatch = useDispatch();
  const { negativeEtvaForSSNsOnPayprofit } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetNegativeEVTASSNQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetNegativeEtvaForSSNsOnPayProfitColumns(viewBadge), []);


  const dummyETVAData: NegativeEtvaForSSNsOnPayProfit[] = [
    {
      employeeBadge: 700127,
      employeeSsn: 123456789,
      etvaValue: -1234.56
    },
    {
      employeeBadge: 234567,
      employeeSsn: 234567890,
      etvaValue: -42.10
    },
    {
      employeeBadge: 345678,
      employeeSsn: 345678901,
      etvaValue: -999.99
    },
    {
      employeeBadge: 456789,
      employeeSsn: 456789012,
      etvaValue: -5000.00
    },
    {
      employeeBadge: 567890,
      employeeSsn: 567890123,
      etvaValue: -1.50
    },
    {
      employeeBadge: 678901,
      employeeSsn: 678901234,
      etvaValue: -750.25
    },
    {
      employeeBadge: 789012,
      employeeSsn: 789012345,
      etvaValue: -3333.33
    },
    {
      employeeBadge: 890123,
      employeeSsn: 890123456,
      etvaValue: -15.75
    },
    {
      employeeBadge: 901234,
      employeeSsn: 901234567,
      etvaValue: -2500.00
    },
    {
      employeeBadge: 12345,
      employeeSsn: 12345678,
      etvaValue: -175.80
    }
  ];

  return (
    <>
      {negativeEtvaForSSNsOnPayprofit?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Negative ETVA For SSNs On Payprofit (${negativeEtvaForSSNsOnPayprofit?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: dummyETVAData,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!negativeEtvaForSSNsOnPayprofit && negativeEtvaForSSNsOnPayprofit.response.results.length > 0 && (
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
          recordCount={negativeEtvaForSSNsOnPayprofit.response.total}
        />
      )}
    </>
  );
};

export default NegativeEtvaForSSNsOnPayprofitGrid;
