import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Typography, Box, CircularProgress } from '@mui/material';
import { DSMGrid, ISortParams, Pagination } from 'smart-ui-library';
import { useNavigate, Path } from 'react-router-dom';
import { useLazyGetYearEndProfitSharingReportQuery } from 'reduxstore/api/YearsEndApi';
import { CAPTIONS } from '../../../constants';
import { useSelector } from 'react-redux';
import { RootState } from '../../../reduxstore/store';
import useFiscalCloseProfitYear from 'hooks/useFiscalCloseProfitYear';
import pay426Utils from '../Pay427Utils';
import { GetProfitSharingReportGridColumns } from '../PAY426-1/EighteenToTwentyGridColumns';
import presets from './presets';
import { FilterParams } from 'reduxstore/types';

interface ReportGridProps {
  params: FilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const ReportGrid: React.FC<ReportGridProps> = ({ params, onLoadingChange }) => {
  const navigate = useNavigate();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: 'employeeName',
    isSortDescending: false
  });
  
  const [trigger, { isFetching }] = useLazyGetYearEndProfitSharingReportQuery();
  const hasToken = useSelector((state: RootState) => !!state.security.token);
  const profitYear = useFiscalCloseProfitYear();

  const data = useSelector((state: RootState) => state.yearsEnd.yearEndProfitSharingReport);

  // Notify parent component about loading state changes
  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  const getReportTitle = () => {
    const matchingPreset = presets.find(preset => 
      JSON.stringify(preset.params) === JSON.stringify(params)
    );
    
    if (matchingPreset) {
      return matchingPreset.description.toUpperCase();
    }
    
    return 'N/A';
  };

  useEffect(() => {
    if (hasToken && params) {
      trigger({
        profitYear: profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        },
        ...params
      });
    }
  }, [trigger, hasToken, profitYear, pageNumber, pageSize, sortParams, params]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    const t = () => { 
        trigger({
          profitYear: profitYear,
          pagination: {
            skip: 0,
            take: pageSize,
            sortBy: update.sortBy,
            isSortDescending: update.isSortDescending
          },
          ...params
        }
      );
    }

    pay426Utils.sortEventHandler(
      update,
      sortParams,
      setSortParams,
      setPageNumber,
      t
    );
  };

  const columnDefs = useMemo(
    () => GetProfitSharingReportGridColumns(handleNavigationForButton),
    [handleNavigationForButton]
  );

  const pinnedTopRowData = useMemo(() => {
    if (!data) return [];
    
    if (params.includeBeneficiaries && !params.includeActiveEmployees && !params.includeInactiveEmployees) {
      const beneficiaryCount = data.numberOfEmployees || 0;
      const wagesTotal = data.wagesTotal || 0;
      
      let balanceTotal = 0;
      if (data.response?.results) {
        balanceTotal = data.response.results.reduce((total, curr) => total + (curr.balance || 0), 0);
      }

      return [
        {
          employeeName: `Total Non-EMPs Beneficiaries`,
          storeNumber: beneficiaryCount,
          wages: wagesTotal,
          balance: balanceTotal
        },
        {
          employeeName: "No Wages",
          storeNumber: 0,
          wages: 0,
          balance: 0
        }
      ];
    } else {
     console.log("API data:", data);
    return [
      {
        employeeName: `TOTAL EMPS: ${data.numberOfEmployees || 0}`,
        wages: data.wagesTotal || 0,
        hours: data.hoursTotal || 0,
        points: data.pointsTotal || 0,
        balance: data.balanceTotal || 0,
        isNew: data.numberOfNewEmployees || 0,
      },
      {
        employeeName: "No Wages",
        wages: 0,
        hours: 0,
        points: 0,
        balance: 0
      }
      ];
    }
  }, [data, params]);

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`${getReportTitle()} (${data?.response?.total || 0} records)`}
        </Typography>
      </div>
      
      {isFetching ? (
        <Box display="flex" justifyContent="center" alignItems="center" py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <DSMGrid
            preferenceKey="PAY426N_REPORT"
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: data?.response?.results || [],
              columnDefs: columnDefs,
              pinnedTopRowData: pinnedTopRowData
            }}
          />
          {!!data && data.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => setPageNumber(value - 1)}
              pageSize={pageSize}
              setPageSize={setPageSize}
              recordCount={data.response.total}
            />
          )}
        </>
      )}
    </>
  );
};

export default ReportGrid; 