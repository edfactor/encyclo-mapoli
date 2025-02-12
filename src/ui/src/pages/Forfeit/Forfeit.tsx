import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useParams } from "react-router-dom";
import { Page, DSMAccordion } from "smart-ui-library";

const Forfeit = () => {

    const { badgeNumber } = useParams<{
        badgeNumber: string;
    }>();

    return (
        <Page label={`Forfeit: ${badgeNumber}`}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Filter">
                        <></>
                    </DSMAccordion>
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default Forfeit;
