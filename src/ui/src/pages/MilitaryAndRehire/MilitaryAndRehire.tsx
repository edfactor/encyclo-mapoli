import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireSearchFilter from "./MilitaryAndRehireSearchFilter";
import MilitaryAndRehireGrid from "./MilitaryAndRehireGrid";
import { useNavigate } from "react-router";

const MilitaryAndRehire = () => {
  const navigate = useNavigate();
  return (
    <Page label="Rehire" actionNode={<Button onClick={() => navigate('/december-process-accordion')} variant="outlined">December Flow</Button>}>
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <MilitaryAndRehireSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <MilitaryAndRehireGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default MilitaryAndRehire;
