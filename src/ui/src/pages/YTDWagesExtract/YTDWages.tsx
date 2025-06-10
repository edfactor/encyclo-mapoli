import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import YTDWagesGrid from "./YTDWagesGrid";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";

const YTDWages: React.FC = () => {
  const [triggerSearch] = useLazyGetEmployeeWagesForYearQuery();

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);  
  const { employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);

  const componentRef = useRef<HTMLDivElement>(null);
  const renderActionNode = () => {
      return <StatusDropdownActionNode />;
    };

  return (
    <Page label={`YTD Wages Extract (PROF-DOLLAR-EXTRACT) (${employeeWagesForYear?.response&& employeeWagesForYear.response.total} records)` } actionNode={renderActionNode()}>
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
