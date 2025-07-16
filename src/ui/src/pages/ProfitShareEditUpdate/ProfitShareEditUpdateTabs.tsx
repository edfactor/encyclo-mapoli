import TabContext from "@mui/lab/TabContext";
import TabList from "@mui/lab/TabList";
import TabPanel from "@mui/lab/TabPanel";
import Box from "@mui/material/Box";
import Tab from "@mui/material/Tab";
import { SyntheticEvent, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import ProfitShareEditGrid from "./ProfitShareEditGrid";
import ProfitShareUpdateGrid from "./ProfitShareUpdateGrid";

interface ProfitShareEditUpdateTabsProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumberReset: boolean;
  setPageNumberReset: (reset: boolean) => void;
}

const ProfitShareEditUpdateTabs = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  pageNumberReset,
  setPageNumberReset
}: ProfitShareEditUpdateTabsProps) => {
  const [value, setValue] = useState("1");
  const { profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);

  const handleChange = (_event: SyntheticEvent, newValue: string) => {
    setValue(newValue);
  };

  return (
    <>
      {profitSharingUpdate?.reportName && (
        <Box sx={{ width: "100%", typography: "body1" }}>
          <TabContext value={value}>
            <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
              <TabList
                onChange={handleChange}
                aria-label="Category Tabs">
                <Tab
                  label="Preview Updates"
                  value="1"
                />
                <Tab
                  label="Preview Details"
                  value="2"
                />
              </TabList>
            </Box>
            <TabPanel value="1">
              <ProfitShareUpdateGrid
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
                pageNumberReset={pageNumberReset}
                setPageNumberReset={setPageNumberReset}
              />
            </TabPanel>
            <TabPanel value="2">
              <ProfitShareEditGrid
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
                pageNumberReset={pageNumberReset}
                setPageNumberReset={setPageNumberReset}
              />
            </TabPanel>
          </TabContext>
        </Box>
      )}
    </>
  );
};

export default ProfitShareEditUpdateTabs;
