import { Divider, Typography, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Page, TotalsGrid } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetControlSheetQuery } from "reduxstore/api/YearsEndApi";
const ProfCtrlSheet = () => {
  const profitYear = useFiscalCloseProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerFetch, { isFetching }] = useLazyGetControlSheetQuery();
  const controlSheet = useSelector((state: RootState) => state.yearsEnd.controlSheet);

  useEffect(() => {
    if (!hasToken) return;

    triggerFetch({
      profitYear,
      pagination: {
        skip: 0,
        take: 255,
        isSortDescending: true,
        sortBy: "type"
      }
    });
  }, [triggerFetch, profitYear, hasToken]);

  const formatCurrency = (amount: number | undefined) => {
    if (amount === undefined) return "$XX,XXX,XXX.XX";
    return `$${amount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  };

  return (
    <Page label={CAPTIONS.PROF_CTRLSHEET}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width="100%">
          <>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {CAPTIONS.PROFIT_SHARING_CONTROL_SHEET}
              </Typography>
              <TotalsGrid
                displayData={[
                  ["Payprofit", "P/S Total", formatCurrency(controlSheet?.employeeContributionProfitSharingAmount)],
                  ["PayBen Non-Emp", "P/S Total", formatCurrency(controlSheet?.nonEmployeeProfitSharingAmount)],
                  ["PayBen-Emp", "P/S Total", formatCurrency(controlSheet?.employeeBeneficiaryAmount)],
                  ["Profit Sharing", "Total", formatCurrency(controlSheet?.profitSharingAmount)]
                ]}
                leftColumnHeaders={[]}
                topRowHeaders={[]}
              />
            </div>
          </>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfCtrlSheet;
