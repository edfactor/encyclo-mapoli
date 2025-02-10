import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
// TODO: import { useLazyGetProfitShareReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetProfitShareReportColumns } from "./ProfitShareReportGridColumn";

interface ProfitShareReportRow {
  badgeNumber: string;
  fullName: string;
  oracleHcmId: string;
}


const ProfitShareReportGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  // const { profitShare } = useSelector((state: RootState) => state.yearsEnd);
  // const [_, { isLoading }] = useLazyGetProfitShareReportQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetProfitShareReportColumns(), []);

  const mockProfitShareReportData: ProfitShareReportRow[] = [
    {
      badgeNumber: "1234",
      fullName: "John Smith",
      oracleHcmId: "HCM001234",
    },
    {
      badgeNumber: "5678",
      fullName: "Jane Doe",
      oracleHcmId: "HCM005678",
    },
    {
      badgeNumber: "9012",
      fullName: "Bob Johnson",
      oracleHcmId: "HCM009012",
    },
    {
      badgeNumber: "3456",
      fullName: "Alice Williams",
      oracleHcmId: "HCM003456",
    },
    {
      badgeNumber: "7890",
      fullName: "David Brown",
      oracleHcmId: "HCM007890",
    }
  ];

  return (
    <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`PROFIT-ELIGIBLE REPORT (5)`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"ProfitShareReportGrid"}
            isLoading={false}
            providedOptions={{
                rowData: mockProfitShareReportData,
                columnDefs: columnDefs
            }}
        />
      
    </>
  );
};

export default ProfitShareReportGrid;
