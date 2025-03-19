import { Button, Divider, MenuItem, Select, SelectChangeEvent, Stack, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import DuplicateNamesAndBirthdaysGrid from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";
import DuplicateSSNsOnDemographicsGrid from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";
import { useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { MENU_LABELS } from "../../constants";
import NegativeETVA from "./NegativeETVA";
import { setSelectedProfitYearForDecemberActivities } from "reduxstore/slices/yearsEndSlice";

const DecemberProcessAccordion = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { selectedProfitYearForDecemberActivities } = useSelector((state: RootState) => state.yearsEnd);

  const ProfitYearSelector = () => {
    const handleChange = (event: SelectChangeEvent) => {
      dispatch(setSelectedProfitYearForDecemberActivities(Number(event.target.value)));
    };

    return (
      <div className="flex items-center gap-2 h-10 min-w-[174px]">
        <Select
          labelId="demo-simple-select-label"
          id="demo-simple-select"
          defaultValue="2024"
          value={selectedProfitYearForDecemberActivities.toString()}
          size="small"
          fullWidth
          onChange={handleChange}
        >
          <MenuItem value={2024}>2024</MenuItem>
          <MenuItem value={2025}>2025</MenuItem>
          <MenuItem value={2026}>2026</MenuItem>
        </Select>
        </div>
    );
  }

  return (
    <Page label={MENU_LABELS.DECEMBER_ACTIVITIES} actionNode={<ProfitYearSelector />}>
      <Grid2 container>
        <Grid2
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Clean Up Reports"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "In Progress",
              color: "success"
            }}
            onActionClick={() => navigate("/clean-up-summary")}
            isCollapsedOnRender={true}>
            <Grid2 width="100%">
              <NegativeETVA />
            </Grid2>
            <Grid2 width="100%">
              <DuplicateSSNsOnDemographicsGrid />
            </Grid2>
            <Grid2 width="100%">
              <DuplicateNamesAndBirthdaysGrid
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
              />
            </Grid2>
            <Grid2 width="100%">
              <Stack paddingX="24px">
                <Typography
                  variant="h2"
                  sx={{ color: "#0258A5" }}>
                  MISSING COMMA IN FULL NAME
                </Typography>
                <Stack
                  sx={{ alignContent: "center" }}
                  direction="row">
                  <Typography sx={{ margin: 0, padding: 0 }}>This has not been created yet. </Typography>
                  <Button>Create Report</Button>
                </Stack>
              </Stack>
            </Grid2>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Military"
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            onActionClick={() => navigate("/military-entry-and-modification")}
            actionButtonText="START">
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Rehire"
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            onActionClick={() => navigate("/military-and-rehire-forfeitures")}
            actionButtonText="START">
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Terminations"
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            actionButtonText="START"
            onActionClick={() => navigate("/prof-term")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Distributions and Forfeitures (QPAY129)"
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            actionButtonText="START"
            onActionClick={() => navigate("/distributions-and-forfeitures")}
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Manage Executives"
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            onActionClick={() => navigate("/manage-executive-hours-and-dollars")}
            actionButtonText="START"
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Totals Report (PAY426)"
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            onActionClick={() => navigate("/profit-share-report")}
            actionButtonText="START">
            <></>
          </DSMCollapsedAccordion>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DecemberProcessAccordion;
