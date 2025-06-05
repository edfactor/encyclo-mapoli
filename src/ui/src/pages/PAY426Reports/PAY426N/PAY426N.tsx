import React, { useState } from 'react';
import { Box, Typography, Divider } from '@mui/material';
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from 'smart-ui-library';
import ProfitSummary from '../PAY426-9/ProfitSummary';
import FilterSection from './FilterSection';
import presets from './presets';
import ReportGrid from './ReportGrid';
import { CAPTIONS } from '../../../constants';
import StatusDropdownActionNode from 'components/StatusDropdownActionNode';
import { FilterParams, ReportPreset } from 'reduxstore/types';
import { clearYearEndProfitSharingReport } from 'reduxstore/slices/yearsEndSlice';
import { useDispatch } from 'react-redux';

const PAY426N: React.FC = () => {
  const [currentPreset, setCurrentPreset] = useState<ReportPreset | null>(null);
  const [showSummaryReport, setShowSummaryReport] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const dispatch = useDispatch();
  
  const handlePresetChange = (preset: ReportPreset | null) => {
    dispatch(clearYearEndProfitSharingReport());
    setCurrentPreset(preset);
    if (preset) {
      setShowSummaryReport(preset.id === 'PAY426-9');
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
    return (
      <StatusDropdownActionNode />
    );
  };

  return (
    <Page label={CAPTIONS.PAY426N} actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <FilterSection 
              presets={presets}
              currentPreset={currentPreset}
              onPresetChange={handlePresetChange}
              onReset={handleReset}
              isLoading={isLoading}
            />
          </DSMAccordion>
        </Grid2>
        
        <Grid2 width="100%">
          {currentPreset && !showSummaryReport && (
            <ReportGrid 
              params={currentPreset.params} 
              onLoadingChange={handleLoadingChange}
            />
          )}
          
          {showSummaryReport && (
            <ProfitSummary />
          )}
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default PAY426N; 