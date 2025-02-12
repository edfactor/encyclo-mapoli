import { Divider, Typography } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { Page, DSMAccordion, TotalsGrid } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import Pay450Grid from "./Pay450Grid";
import LabelValueSection from "components/LabelValueSection";
import ProfCtrlSheetSearchFilters from "./ProfCtrlSheetSearchFilters";

const ProfCtrlSheet = () => {


    return (
        <Page label={CAPTIONS.PROF_CTRLSheet}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Filter">
                        <ProfCtrlSheetSearchFilters />
                    </DSMAccordion>
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
                                    ['Payprofit', 'P/S Total', '$XX,XXX,XXX.XX'],
                                    ['PayBen Non-Emp', 'P/S Total', '$XXX,XXX.XX'],
                                    ['PayBen-Emp', 'P/S Total', '$0.00'],
                                    ['Profit Sharing', 'Total', '$XX,XXX,XXX.XX']
                                ]}
                                leftColumnHeaders={[
                                ]}
                                topRowHeaders={[
                                    
                                ]}
                            />
                        </div>
                    </>
                </Grid2>
            </Grid2>
        </Page>
    )


};

export default ProfCtrlSheet;