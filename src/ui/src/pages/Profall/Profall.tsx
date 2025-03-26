import { Button, Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { CAPTIONS } from "../../constants";
import { Page } from "smart-ui-library";
import ProfallGrid from "./ProfallGrid";

const Profall = () => {
    return (
        <Page 
            label={CAPTIONS.PROFALL}
        >
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2 width="100%">
                    <ProfallGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default Profall;