import { Divider, Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { CAPTIONS } from "../../constants";
import { DSMAccordion, Page, TotalsGrid } from "smart-ui-library";
import Under21ReportGrid from "./Under21ReportGrid";
import Under21ReportSearchFilters from "./Under21SearchFilters";

const NewPSLabels = () => {
    return (
        <Page label={CAPTIONS.NEW_PS_LABELS}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2 width={"100%"}>
                    <DSMAccordion title="Filter">
                        <Under21ReportSearchFilters />
                    </DSMAccordion>
                </Grid2>

                <div className="flex sticky top-0 z-10 bg-white">
                    <TotalsGrid
                        displayData={[
                            ['Under 21 100% Vested', '1'],
                            ['Under 21 Partially Vested', '0'],
                            ['Under 21 with 1-2 PS Years', '20']
                        ]}
                        leftColumnHeaders={[]}
                        topRowHeaders={['', 'Active Employees']}
                    />

                    <TotalsGrid
                        displayData={[
                            ['Under 21 100% Vested', '0'],
                            ['Under 21 Partially Vested', '0'],
                            ['Under 21 with 1-2 PS Years', '20']
                        ]}
                        leftColumnHeaders={[]}
                        topRowHeaders={['', 'Terminated Employees']}
                    />

                    <TotalsGrid
                        displayData={[
                            ['Under 21 100% Vested', '1'],
                            ['Under 21 Partially Vested', '0'],
                            ['Under 21 with 1-2 PS Years', '0']
                        ]}
                        leftColumnHeaders={[]}
                        topRowHeaders={['', 'Inactive Employees']}
                    />
                </div>

                <Grid2 width="100%">
                    <Under21ReportGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default NewPSLabels;