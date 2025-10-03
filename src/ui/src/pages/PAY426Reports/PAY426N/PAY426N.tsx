import { Divider, Grid } from "@mui/material";
import FrozenYearWarning from "components/FrozenYearWarning";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StatusReadOnlyInfo from "components/StatusReadOnlyInfo";
import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { useParams } from "react-router-dom";
import { clearYearEndProfitSharingReport } from "reduxstore/slices/yearsEndSlice";
import { ReportPreset } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useIsProfitYearFrozen } from "../../../hooks/useIsProfitYearFrozen";
import { useIsReadOnlyByStatus } from "../../../hooks/useIsReadOnlyByStatus";
import ProfitSummary from "../ProfitSummary/ProfitSummary";
import FilterSection from "./FilterSection";
import presets from "./presets";
import ReportGrid from "./ReportGrid";

const PAY426N: React.FC = () => {
  const [currentPreset, setCurrentPreset] = useState<ReportPreset | null>(null);
  const [showSummaryReport, setShowSummaryReport] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const dispatch = useDispatch();
  const profitYear = useDecemberFlowProfitYear();
  const isFrozen = useIsProfitYearFrozen(profitYear);
  const isReadOnlyByStatus = useIsReadOnlyByStatus();

  const { presetNumber } = useParams<{
    presetNumber: string;
  }>();

  if (presetNumber && !currentPreset) {
    const preset = presets.find((p) => p.id === presetNumber);
    if (preset) {
      setCurrentPreset(preset);
      setShowSummaryReport(preset.id === "9");
    }
  }

  const handlePresetChange = (preset: ReportPreset | null) => {
    dispatch(clearYearEndProfitSharingReport());
    setCurrentPreset(preset);
    if (preset) {
      setShowSummaryReport(preset.id === "9");
    } else {
      setShowSummaryReport(false);
    }
  };

  const handleReset = () => {
    setCurrentPreset(null);
    setShowSummaryReport(false);
  };

  const handleLoadingChange = (loading: boolean) => {
    setIsLoading(loading);
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.PAY426N}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        {isFrozen && <FrozenYearWarning profitYear={profitYear} />}
        {isReadOnlyByStatus && <StatusReadOnlyInfo />}
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <FilterSection
              presets={presets}
              currentPreset={currentPreset}
              onPresetChange={handlePresetChange}
              onReset={handleReset}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          {currentPreset && !showSummaryReport && (
            <ReportGrid
              params={currentPreset.params}
              onLoadingChange={handleLoadingChange}
            />
          )}

          {showSummaryReport && <ProfitSummary />}
        </Grid>
      </Grid>
    </Page>
  );
};

export default PAY426N;
