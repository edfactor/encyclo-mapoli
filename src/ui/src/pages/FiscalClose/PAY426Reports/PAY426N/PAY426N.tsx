import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Divider, Grid } from "@mui/material";
import FrozenYearWarning from "components/FrozenYearWarning";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StatusReadOnlyInfo from "components/StatusReadOnlyInfo";
import React, { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import {
    clearYearEndProfitSharingReportFrozen,
    clearYearEndProfitSharingReportLive
} from "reduxstore/slices/yearsEndSlice";
import { ReportPreset } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS, ROUTES } from "../../../../constants";
import { useGridExpansion } from "../../../../hooks/useGridExpansion";
import { useIsProfitYearFrozen } from "../../../../hooks/useIsProfitYearFrozen";
import { useIsReadOnlyByStatus } from "../../../../hooks/useIsReadOnlyByStatus";
import useNavigationYear from "../../../../hooks/useNavigationYear";
import ProfitSummary from "../ProfitSummary/ProfitSummary";
import FilterSection from "./FilterSection";
import presets from "./presets";
import ReportGrid from "./ReportGrid";

const PAY426N: React.FC<{ isFrozen: boolean }> = () => {
  const [currentPreset, setCurrentPreset] = useState<ReportPreset | null>(null);
  const [showSummaryReport, setShowSummaryReport] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTrigger, setSearchTrigger] = useState(0);
  const { isGridExpanded, handleToggleGridExpand } = useGridExpansion();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const profitYear = useNavigationYear();
  const isFrozen = useIsProfitYearFrozen(profitYear);
  const isReadOnlyByStatus = useIsReadOnlyByStatus();

  const { presetNumber } = useParams<{
    presetNumber: string;
  }>();

  // Apply preset from URL parameter
  useEffect(() => {
    if (presetNumber) {
      const preset = presets.find((p) => p.id === presetNumber);
      if (preset) {
        setCurrentPreset(preset);
        setShowSummaryReport(preset.id === "9");
      }
    } else {
      // Clear preset when URL parameter is removed
      setCurrentPreset(null);
      setShowSummaryReport(false);
    }
  }, [presetNumber]);

  const handlePresetChange = (preset: ReportPreset | null) => {
    if (isFrozen) {
      dispatch(clearYearEndProfitSharingReportFrozen());
    } else {
      dispatch(clearYearEndProfitSharingReportLive());
    }
    setCurrentPreset(preset);
    if (preset) {
      setShowSummaryReport(preset.id === "9");
    } else {
      setShowSummaryReport(false);
    }
  };

  const handleReset = () => {
    // Clear Redux state
    if (isFrozen) {
      dispatch(clearYearEndProfitSharingReportFrozen());
    } else {
      dispatch(clearYearEndProfitSharingReportLive());
    }

    // Clear component state
    setCurrentPreset(null);
    setShowSummaryReport(false);

    // Clear URL parameter by navigating to the base route
    const baseRoute = isFrozen ? ROUTES.PAY426N_FROZEN : ROUTES.PAY426N_LIVE;
    navigate(`/${baseRoute}`);
  };

  const handleLoadingChange = (loading: boolean) => {
    setIsLoading(loading);
  };

  const handleSearch = () => {
    // Toggle searchTrigger to force re-search
    setSearchTrigger((prev) => prev + 1);
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <PageErrorBoundary pageName="PAY426N">
      <Page
        label={isGridExpanded ? "" : `${CAPTIONS.PAY426N}`}
        actionNode={isGridExpanded ? undefined : renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          {isFrozen && <FrozenYearWarning profitYear={profitYear} />}
          {isReadOnlyByStatus && <StatusReadOnlyInfo />}
          {!isGridExpanded && (
            <>
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
                    onSearch={handleSearch}
                    isLoading={isLoading}
                  />
                </DSMAccordion>
              </Grid>
            </>
          )}

          <Grid width="100%">
            {currentPreset && !showSummaryReport && (
              <ReportGrid
                params={currentPreset.params}
                onLoadingChange={handleLoadingChange}
                isFrozen={isFrozen}
                searchTrigger={searchTrigger}
                isGridExpanded={isGridExpanded}
                onToggleExpand={handleToggleGridExpand}
                profitYear={profitYear}
              />
            )}

            {showSummaryReport && (
              <ProfitSummary
                frozenData={isFrozen}
                externalIsGridExpanded={isGridExpanded}
                externalOnToggleExpand={handleToggleGridExpand}
              />
            )}
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default PAY426N;
