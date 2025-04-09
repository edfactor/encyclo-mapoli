import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useSelector, useDispatch } from "react-redux";
import { RootState } from "reduxstore/store";
import { numberToCurrency, Page, TotalsGrid } from "smart-ui-library";
import VestedAmountsByAgeTabs from "./VestedAmountsByAgeTabs";
import { useState, useEffect } from "react";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetVestingAmountByAgeQuery } from "reduxstore/api/YearsEndApi";
import { setVestedAmountsByAgeQueryParams } from "reduxstore/slices/yearsEndSlice";

const options: Intl.DateTimeFormatOptions = {
  month: "2-digit",
  day: "2-digit",
  year: "numeric"
};

function toCapitalCase(str: string): string {
  return str.toLowerCase().replace(/(?:^|\s)\S/g, function (match) {
    return match.toUpperCase();
  });
}

const VestedAmountsByAge = () => {
  const [hasInitialSearchRun, setHasInitialSearchRun] = useState(false);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetVestingAmountByAgeQuery();
  const { vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);

  useEffect(() => {
    if (hasToken && profitYear && !hasInitialSearchRun) {
      setHasInitialSearchRun(true);
      
      triggerSearch(
        {
          profitYear: profitYear,
          acceptHeader: "application/json"
        },
        false
      )
        .then((result: any) => {
          if (result.data) {
            dispatch(setVestedAmountsByAgeQueryParams(profitYear));
          }
        })
        .catch((error: any) => {
          console.error("Initial vested amounts by age search failed:", error);
        });
    }
  }, [hasToken, profitYear, hasInitialSearchRun, triggerSearch, dispatch]);

  return (
    <Page label="Vested Amounts by Age">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2
          width={"100%"}
          sx={{ overflowX: "inherit" }}>
          {vestedAmountsByAge?.response && (
            <div style={{ overflowX: "inherit" }}>
              <div className="px-[24px]">
                <h2 className="text-dsm-secondary">Summary</h2>
                <h3 className="text-dsm-secondary">
                  {toCapitalCase(vestedAmountsByAge.reportName)}
                  {"  -   "}
                  {new Date(vestedAmountsByAge.reportDate).toLocaleDateString("en-US", options)}
                </h3>
              </div>

              <TotalsGrid
                displayData={[
                  [
                    numberToCurrency(vestedAmountsByAge?.totalFullTime100PercentAmount ?? 0),
                    numberToCurrency(vestedAmountsByAge?.totalPartTimePartialAmount ?? 0),
                    numberToCurrency(vestedAmountsByAge?.totalFullTimeNotVestedAmount ?? 0),
                    numberToCurrency(vestedAmountsByAge?.totalPartTime100PercentAmount ?? 0),
                    numberToCurrency(vestedAmountsByAge?.totalPartTimePartialAmount ?? 0),
                    numberToCurrency(vestedAmountsByAge?.totalPartTimeNotVestedAmount ?? 0),
                    vestedAmountsByAge?.totalBeneficiaryCount ?? 0,
                    numberToCurrency(vestedAmountsByAge?.totalBeneficiaryAmount ?? 0),
                    vestedAmountsByAge?.totalFullTimeCount ?? 0,
                    vestedAmountsByAge?.totalNotVestedCount ?? 0,
                    vestedAmountsByAge?.totalPartialVestedCount ?? 0
                  ]
                ]}
                leftColumnHeaders={[""]}
                topRowHeaders={[
                  "FT 100%",
                  "FT Partial Vested",
                  "FT Not Vested",
                  "PT 100% Vested",
                  "PT Partial Vested",
                  "PT Not Vested",
                  "Beneficiaries",
                  "Beneficiary Amount",
                  "FT Total Count",
                  "Not Vested",
                  "Partial Vested"
                ]}
              />

              <Grid2 width={"100%"}>
                <VestedAmountsByAgeTabs />
              </Grid2>
            </div>
          )}
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default VestedAmountsByAge;
