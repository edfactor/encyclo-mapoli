import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid, numberToCurrency } from "smart-ui-library";
import { GetBalanceByAgeColumns } from "./BalanceByAgeGridColumns";
import { Grid } from "@mui/material";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";

interface BalanceByAgeGridProps {
  initialSearchLoaded: boolean;
}

const BalanceByAgeGrid: React.FC<BalanceByAgeGridProps> = ({ initialSearchLoaded }) => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { balanceByAgeTotal, balanceByAgeFullTime, balanceByAgePartTime, balanceByAgeQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [triggerSearch, { isFetching }] = useLazyGetBalanceByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const onSearch = useCallback(async () => {
    await triggerSearch(
      {
        profitYear: balanceByAgeQueryParams?.profitYear ?? 0,
        reportType: FrozenReportsByAgeRequestType.Total,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    await triggerSearch(
      {
        profitYear: balanceByAgeQueryParams?.profitYear ?? 0,
        reportType: FrozenReportsByAgeRequestType.FullTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    await triggerSearch(
      {
        profitYear: balanceByAgeQueryParams?.profitYear ?? 0,
        reportType: FrozenReportsByAgeRequestType.PartTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
  }, [triggerSearch, balanceByAgeQueryParams?.profitYear]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded && balanceByAgeQueryParams?.profitYear) {
      onSearch();
    }
  }, [balanceByAgeQueryParams?.profitYear, hasToken, initialSearchLoaded, onSearch]);

  return (
    <>
      {balanceByAgeTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="sticky top-0 z-10 flex bg-white">
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
          <Grid
            size={{ xs: 12 }}
            container>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_Total"}
                isLoading={isFetching}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByAgeTotal?.response.results,

                  columnDefs: columnDefsTotal
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={isFetching}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByAgeFullTime?.response.results,

                  columnDefs: columnDefsFullTime
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={isFetching}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByAgePartTime?.response.results,
                  columnDefs: columnDefsPartTime
                }}
              />
            </Grid>
          </Grid>
        </>
      )}
    </>
  );
};

export default BalanceByAgeGrid;
