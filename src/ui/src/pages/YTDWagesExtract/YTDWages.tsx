import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import { downloadFileFromResponse } from "utils/fileDownload";
import { CAPTIONS } from "../../constants";
import YTDWagesGrid from "./YTDWagesGrid";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";

interface SearchData {
  profitYear: number;
}

const YTDWages: React.FC = () => {
  const [triggerSearch] = useLazyGetEmployeeWagesForYearQuery();

  const { employeeWagesForYearQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const thisYear = new Date().getFullYear();
  const lastYear = thisYear - 1;
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  const componentRef = useRef<HTMLDivElement>(null);

  const handleDownloadCSV = async (data: SearchData) => {
    try {
      const fetchCSVResult = await triggerSearch({
        profitYear: data.profitYear,
        acceptHeader: "text/csv",
        pagination: { skip: 0, take: 20000, sortBy: "badgeNumber", isSortDescending: false }
      });

      if (fetchCSVResult.data) {
        await downloadFileFromResponse(
          Promise.resolve({ data: fetchCSVResult.data instanceof Blob ? fetchCSVResult.data : new Blob() }),
          `ytd-employee-wage-extract-${employeeWagesForYearQueryParams?.profitYear || lastYear}.csv`
        );
      } else {
        console.error("Failed to fetch CSV data");
      }

      // We need to restore the grid with the original JSON, not the CSV blob
      // that we just downloaded that displaced the json we had before
      triggerSearch({
        profitYear: data.profitYear,
        acceptHeader: "text/json",
        pagination: { skip: 0, take: 255, sortBy: "badgeNumber", isSortDescending: false }
      });
    } catch (error) {
      console.error("Error reloading grid data after download", error);
    }
  };
  const renderActionNode = () => {
    return (
      <></>
      // <div className="flex items-center gap-2 h-10">
      //   <Button
      //     onClick={() => {
      //       handleDownloadCSV({ profitYear: employeeWagesForYearQueryParams?.profitYear || lastYear });
      //     }}
      //     variant="outlined"
      //     startIcon={<Download color={"primary"} />}
      //     className="h-10 whitespace-nowrap min-w-fit">
      //     Download
      //   </Button>
      // </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.YTD_WAGES_EXTRACT}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
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
