import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyPayBenReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { PayBenReportRequest, PayBenReportResponse } from "reduxstore/types";
import { DSMGrid, ISortParams, Page, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { PayBenReportGridColumn } from "./PayBenReportGridColumns";

const PayBenReport = () => {
  const { token, appUser, username: stateUsername } = useSelector((state: RootState) => state.security);
  const [triggerReport, { isFetching, isSuccess }] = useLazyPayBenReportQuery();
  const [payBenReportResponse, setPayBenReportResponse] = useState<PayBenReportResponse>();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "ssn",
    isSortDescending: true
  });

  // Create column definitions with expand/collapse functionality
  const columnDefs = useMemo(() => {
    const column = PayBenReportGridColumn();
    return column;
  }, []);

  const createPayBenReportRequest = (
    skip: number,
    sortBy: string,
    isSortDescending: boolean,
    take: number,
    id?: number
  ): PayBenReportRequest => {
    const request: PayBenReportRequest = {
      id: id,
      isSortDescending: isSortDescending,
      skip: skip,
      sortBy: sortBy,
      take: take
    };
    return request;
  };

  useEffect(() => {
    if (token) {
      const request = createPayBenReportRequest(pageNumber, _sortParams.sortBy, _sortParams.isSortDescending, pageSize);
      triggerReport(request)
        .unwrap()
        .then((res) => {
          setPayBenReportResponse(res);
        })
        .catch((err: any) => {
          console.error(err);
        });
    }
  }, [token, _sortParams, pageSize, pageNumber]);

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "ssn";
      update.isSortDescending = true;
    }
    setSortParams(update);
    setPageNumber(0);
  };

  return (
    <Page label="PAYROLL BENEFICIARY REPORT (PAYBEN/PAY495)">
      <Grid
        container
        rowSpacing="24px">
        <Grid
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
        </Grid>
        <Grid
          size={{ xs: 12 }}
          width="100%">
          {isSuccess && (
            <>
              <div>
                <DSMGrid
                  preferenceKey={CAPTIONS.PAYBEN_REPORT}
                  isLoading={isFetching}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
                    rowData: payBenReportResponse?.results,
                    columnDefs: columnDefs,
                    suppressMultiSort: true,
                    masterDetail: true
                  }}
                />
                {payBenReportResponse && payBenReportResponse.results.length > 0 && (
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
                    recordCount={payBenReportResponse.total}
                  />
                )}
              </div>
            </>
          )}

          {/**Render Report here! */}
        </Grid>
      </Grid>
    </Page>
  );
};

export default PayBenReport;
