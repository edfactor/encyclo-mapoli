import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMAccordion, numberToCurrency, Page } from "smart-ui-library";
import { TotalsGrid } from "./TotalsGrid";
import VestedAmountsByAgeSearchFilter from "./VestedAmountsByAgeSearchFilter";
import VestedAmountsByAgeTabs from "./VestedAmountsByAgeTabs";

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
  const { vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);

  return (
    <Page label="Vested Amounts by Age">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <VestedAmountsByAgeSearchFilter />
          </DSMAccordion>
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
