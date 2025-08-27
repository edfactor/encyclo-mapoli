import TabContext from "@mui/lab/TabContext";
import TabList from "@mui/lab/TabList";
import TabPanel from "@mui/lab/TabPanel";
import Box from "@mui/material/Box";
import Tab from "@mui/material/Tab";
import { SyntheticEvent, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import VestedAmountsByAgeGrid from "./VestedAmountsByAgeGrid";

const VestedAmountByAgeTabs: React.FC = () => {
  const [value, setValue] = useState("1");
  const { vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);

  const handleChange = (_event: SyntheticEvent, newValue: string) => {
    setValue(newValue);
  };

  return (
    <>
      {vestedAmountsByAge?.response && (
        <Box sx={{ width: "100%", typography: "body1" }}>
          <TabContext value={value}>
            <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
              <TabList
                onChange={handleChange}
                aria-label="Category Tabs">
                <Tab
                  label="FULL TIME 100% VESTED"
                  value="1"
                />
                <Tab
                  label="FULL TIME PARTIALLY VESTED"
                  value="2"
                />
                <Tab
                  label="PART TIME NOT VESTED"
                  value="3"
                />
                <Tab
                  label="PART TIME 100% VESTED"
                  value="4"
                />
                <Tab
                  label="PART TIME PARTIALLY VESTED"
                  value="5"
                />
                <Tab
                  label="BENEFICIARIES"
                  value="6"
                />
              </TabList>
            </Box>
            <TabPanel value="1">
              <VestedAmountsByAgeGrid
                gridTitle="Full Time 100% Vested"
                countColName="fullTime100PercentCount"
                amountColName="fullTime100PercentAmount"
                totalCount="totalFullTimeCount"
              />
            </TabPanel>
            <TabPanel value="2">
              <VestedAmountsByAgeGrid
                gridTitle="Full Time Partially Vested"
                countColName="fullTimePartialCount"
                amountColName="fullTimePartialAmount"
                totalCount="totalPartialVestedCount"
              />
            </TabPanel>
            <TabPanel value="3">
              <VestedAmountsByAgeGrid
                gridTitle="Part Time Not Vested"
                countColName="partTimeNotVestedCount"
                amountColName="partTimeNotVestedAmount"
              />
            </TabPanel>
            <TabPanel value="4">
              <VestedAmountsByAgeGrid
                gridTitle="Part Time 100% Vested"
                countColName="partTime100PercentCount"
                amountColName="partTime100PercentAmount"
              />
            </TabPanel>
            <TabPanel value="5">
              <VestedAmountsByAgeGrid
                gridTitle="Part Time Partially Vested"
                countColName="partTimePartialCount"
                amountColName="partTimePartialAmount"
              />
            </TabPanel>
            <TabPanel value="6">
              <VestedAmountsByAgeGrid
                gridTitle="Beneficiaries"
                countColName="beneficiaryCount"
                amountColName="beneficiaryAmount"
                totalCount="totalBeneficiaryCount"
              />
            </TabPanel>
          </TabContext>
        </Box>
      )}
    </>
  );
};

export default VestedAmountByAgeTabs;
