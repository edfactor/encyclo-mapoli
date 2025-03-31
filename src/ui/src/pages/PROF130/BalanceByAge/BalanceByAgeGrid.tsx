import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetBalanceByAgeColumns } from "./BalanceByAgeGridColumns";
import Grid2 from "@mui/material/Grid2";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { numberToCurrency } from "smart-ui-library";

interface BalanceByAgeGridProps {
  initialSearchLoaded: boolean;
}

const BalanceByAgeGrid: React.FC<BalanceByAgeGridProps> = ({ initialSearchLoaded }) => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { balanceByAgeTotal, balanceByAgeFullTime, balanceByAgePartTime, balanceByAgeQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [triggerSearch, { isLoading }] = useLazyGetBalanceByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  const onSearch = useCallback(async () => {
    triggerSearch(
      {
        profitYear: balanceByAgeQueryParams?.profitYear ?? 0,
        reportType: FrozenReportsByAgeRequestType.Total,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    triggerSearch(
      {
        profitYear: balanceByAgeQueryParams?.profitYear ?? 0,
        reportType: FrozenReportsByAgeRequestType.FullTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    triggerSearch(
      {
        profitYear: balanceByAgeQueryParams?.profitYear ?? 0,
        reportType: FrozenReportsByAgeRequestType.PartTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
  }, [triggerSearch, balanceByAgeQueryParams?.profitYear]);

  useEffect(() => {
    if (initialSearchLoaded && balanceByAgeQueryParams?.profitYear) {
      onSearch();
    }
  }, [balanceByAgeQueryParams?.profitYear, initialSearchLoaded, onSearch]);

  return (
    <>
      {balanceByAgeTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[
                [
                  balanceByAgeTotal?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByAgeTotal?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByAgeTotal?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByAgeTotal?.totalEmployee || 0,
                  numberToCurrency(balanceByAgeTotal?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByAgeTotal?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByAgeTotal?.totalMembers || 0,
                  numberToCurrency(balanceByAgeTotal?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByAgeTotal?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["All", "Count", "Balance", "Vested"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  balanceByAgeFullTime?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByAgeFullTime?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByAgeFullTime?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByAgeFullTime?.totalEmployee || 0,
                  numberToCurrency(balanceByAgeFullTime?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByAgeFullTime?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByAgeFullTime?.totalMembers || 0,
                  numberToCurrency(balanceByAgeFullTime?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByAgeFullTime?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["FullTime", "Count", "Balance", "Vested"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  balanceByAgePartTime?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByAgePartTime?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByAgePartTime?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByAgePartTime?.totalEmployee || 0,
                  numberToCurrency(balanceByAgePartTime?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByAgePartTime?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByAgePartTime?.totalMembers || 0,
                  numberToCurrency(balanceByAgePartTime?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByAgePartTime?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["PartTime", "Count", "Balance", "Vested"]}></TotalsGrid>
          </div>
          <Grid2
            size={{ xs: 12 }}
            container>
            <Grid2 size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_Total"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByAgeTotal?.response.results,

                  columnDefs: [
                    {
                      headerName: columnDefsTotal.headerName,
                      children: columnDefsTotal.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByAgeFullTime?.response.results,

                  columnDefs: [
                    {
                      headerName: columnDefsFullTime.headerName,
                      children: columnDefsFullTime.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByAgePartTime?.response.results,

                  columnDefs: [
                    {
                      headerName: columnDefsPartTime.headerName,
                      children: columnDefsPartTime.children
                    }
                  ]
                }}
              />
            </Grid2>
          </Grid2>
        </>
      )}
    </>
  );
};

export default BalanceByAgeGrid;
