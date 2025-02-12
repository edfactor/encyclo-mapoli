import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { Page, DSMAccordion } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import Pay450SearchFilters from "./Pay450SearchFilters";
import Pay450Grid from "./Pay450Grid";
import LabelValueSection from "components/LabelValueSection";

const Pay450Summary = () => {

    const updateSummarySection = [
        { label: "Employees Updated", value: "452" },
        { label: "Beneficiaries Updated", value: "6" }
    ];

    return (
        <Page label={CAPTIONS.PAY450_SUMMARY}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Filter">
                        <Pay450SearchFilters />
                    </DSMAccordion>
                </Grid2>

                <Grid2 paddingX="24px" xs={2}>
                    <LabelValueSection
                        data={updateSummarySection}
                    />
                </Grid2>
                <Grid2 width="100%">
                    <Pay450Grid />
                </Grid2>
            </Grid2>
        </Page>
    )


};

export default Pay450Summary;