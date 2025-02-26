import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";
import YTDWagesGrid from "./YTDWagesGrid";
import { CAPTIONS } from "../../constants";
import { Download, Print } from "@mui/icons-material";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";
import { downloadFileFromResponse } from "utils/fileDownload";
import { useReactToPrint } from "react-to-print";
import { useRef, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

interface SearchData {
  profitYear: number;
}

const YTDWages: React.FC = () => {
  const [triggerSearch] = useLazyGetEmployeeWagesForYearQuery();
  const { employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);
  const thisYear = new Date().getFullYear();
  const lastYear = thisYear - 1;

  const componentRef = useRef<HTMLDivElement>(null);

  const handlePrint = useReactToPrint({
    contentRef: componentRef,
    documentTitle: "Print Document"
  });

  const [chosenYear, setChosenYear] = useState<number>(lastYear);

  const handleDownloadCSV = async (data: SearchData) => {
    try {
      const fetchCSVPromise = triggerSearch({
        profitYear: data.profitYear,
        acceptHeader: "text/csv",
        pagination: { skip: 0, take: 20000 }
      });
      await downloadFileFromResponse(fetchCSVPromise, `ytd-employee-wage-extract-${chosenYear}.csv`);

      // We need to restore the grid with the original JSON, not the CSV blob
      // that we just downloaded that displaced the json we had before
      triggerSearch({
        profitYear: data.profitYear,
        acceptHeader: "text/json",
        pagination: { skip: 0, take: 255 }
      });
    } catch (error) {
      console.error("Error reloading grid data after download", error);
    }
  };
  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <Button
          onClick={() => {
            handleDownloadCSV({ profitYear: chosenYear });
          }}
          variant="outlined"
          startIcon={<Download color={"primary"} />}
          className="h-10 whitespace-nowrap min-w-fit">
          Download
        </Button>
        <Button
          onClick={() => {
            handlePrint();
          }}
          disabled={
            employeeWagesForYear?.response?.results == undefined || employeeWagesForYear?.response?.results.length === 0
          }
          variant="outlined"
          startIcon={<Print color={"primary"} />}
          className="h-10 whitespace-nowrap min-w-fit">
          Print
        </Button>
      </div>
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
            <YTDWagesSearchFilter
              originalYear={lastYear}
              setChosenYear={setChosenYear}
            />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <YTDWagesGrid innerRef={componentRef} />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default YTDWages;
