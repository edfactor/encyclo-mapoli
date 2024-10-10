import React, { useEffect, useMemo, useState } from 'react';
import { Typography } from '@mui/material';
import { useSelector, useDispatch } from 'react-redux';
import { DSMGrid, ISortParams, Pagination } from 'smart-ui-library';
import { RootState } from 'reduxstore/store';
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from 'reduxstore/api/YearsEndApi'; 
import { GetDemographicBadgesNotInPayprofitColumns } from './DemographicBadgesNotInPayprofitGridColumns';
import { setDemographicBadgesNotInPayprofitData } from 'reduxstore/slices/yearsEndSlice';
import { ImpersonationRoles } from 'reduxstore/types';

const DemographicBadgesNotInPayprofitGrid: React.FC = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  const { demographicBadges } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetDemographicBadgesNotInPayprofitQuery();

  const columnDefs = useMemo(() => GetDemographicBadgesNotInPayprofitColumns(), []);

  return (
    <>
      <div style={{ padding: '0 24px 0 24px' }}>
        <Typography variant="h2" sx={{ color: '#0258A5' }}>
          {`DEMOGRAPHICS BADGES NOT ON PAYPROFIT (${demographicBadges?.response.total || 0})`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"DEMO_BADGES"}
        isLoading={isLoading}
        providedOptions={{
          rowData: demographicBadges?.response.results,
          columnDefs: columnDefs
        }}
      />
      {demographicBadges?.response && demographicBadges.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
          }}
          recordCount={demographicBadges.response.total}
        />
      )}
    </>
  );
};

export default DemographicBadgesNotInPayprofitGrid;