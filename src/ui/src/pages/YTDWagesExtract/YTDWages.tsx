import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useRef, useState } from "react";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, ISortParams, Page } from "smart-ui-library";
import YTDWagesGrid from "./YTDWagesGrid";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const YTDWages: React.FC = () => {
  const [triggerSearch] = useLazyGetEmployeeWagesForYearQuery();

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);  

  const componentRef = useRef<HTMLDivElement>(null);

  return (
    <Page>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"} hidden={true}>
          <DSMAccordion title="Filter">
            <YTDWagesSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <YTDWagesGrid
            innerRef={componentRef}
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default YTDWages;
