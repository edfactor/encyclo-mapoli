import { Divider, MenuItem, Select, SelectChangeEvent, Stack, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import DuplicateNamesAndBirthdaysGrid from "pages/DecemberActivities/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";
import DuplicateSSNsOnDemographicsGrid from "pages/DecemberActivities/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";
import { useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";
import { MENU_LABELS, CAPTIONS, ROUTES } from "../../../constants";
import NegativeETVA from "./NegativeETVA";
import {
  checkDecemberParamsAndGridsProfitYears,
  setSelectedProfitYearForDecemberActivities
} from "reduxstore/slices/yearsEndSlice";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

const DecemberProcessAccordion = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { selectedProfitYearForDecemberActivities } = useSelector((state: RootState) => state.yearsEnd);
  const thisYear = new Date().getFullYear();

  const ProfitYearSelector = () => {
    const handleChange = (event: SelectChangeEvent) => {
      dispatch(setSelectedProfitYearForDecemberActivities(Number(event.target.value)));
      dispatch(checkDecemberParamsAndGridsProfitYears(Number(event.target.value)));
    };

    const profitYear = useDecemberFlowProfitYear();

    return (
      <div className="flex items-center gap-2 h-10 min-w-[174px]">
        <Select
          labelId="demo-simple-select-label"
          id="demo-simple-select"
          defaultValue={profitYear?.toString()}
          value={selectedProfitYearForDecemberActivities.toString()}
          size="small"
          fullWidth
          onChange={handleChange}>
          <MenuItem value={thisYear - 1}>{thisYear - 1}</MenuItem>
          <MenuItem value={thisYear}>{thisYear}</MenuItem>
          <MenuItem value={thisYear + 1}>{thisYear + 1}</MenuItem>
        </Select>
      </div>
    );
  };

  return (
    <Page
      label={MENU_LABELS.DECEMBER_ACTIVITIES}
      actionNode={<ProfitYearSelector />}>
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
                  {CAPTIONS.MISSING_COMMA}
                </Typography>
                <Stack
                  sx={{ alignContent: "center" }}
                  direction="row">
                  <Typography sx={{ margin: 0, padding: 0 }}>This has not been created yet. </Typography>
                  {/* 
                  <Button>Create Report</Button>
                    */}
                </Stack>
              </Stack>
            </Grid2>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.MILITARY_CONTRIBUTIONS}
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
            title={CAPTIONS.REHIRE_FORFEITURES}
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            onActionClick={() => navigate(`/${ROUTES.REHIRE_FORFEITURES}`)}
            actionButtonText="START">
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title={CAPTIONS.TERMINATIONS}
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
            title={CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}
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
            title={CAPTIONS.MANAGE_EXECUTIVE_HOURS}
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
            title={CAPTIONS.PROFIT_SHARE_REPORT}
            expandable={false}
            status={{
              label: "Not Started",
              color: "secondary"
            }}
            onActionClick={() => navigate("/profit-share-report")}
            actionButtonText="VIEW TOTALS"
            isCollapsedOnRender={true}>
            <></>
          </DSMCollapsedAccordion>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DecemberProcessAccordion;
