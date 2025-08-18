import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Typography, Box, CircularProgress } from "@mui/material";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { useNavigate, Path } from "react-router-dom";
import { ReprintCertificatesFilterParams } from "./ReprintCertificatesFilterSection";
import { GetReprintCertificatesGridColumns, ReprintCertificateEmployee } from "./ReprintCertificatesGridColumns";

interface ReprintCertificatesGridProps {
  filterParams: ReprintCertificatesFilterParams;
  onLoadingChange?: (isLoading: boolean) => void;
}

const ReprintCertificatesGrid: React.FC<ReprintCertificatesGridProps> = ({ filterParams, onLoadingChange }) => {
  const navigate = useNavigate();

  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badge",
    isSortDescending: false
  });
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);
  const [isFetching, setIsFetching] = useState(false);

  const mockData = useMemo(() => {
    if (!filterParams) return { results: [], total: 0 };

    return {
      results: [
        {
          badge: 47425,
          name: "BACHELDER, BRAD R",
          ssn: "123-45-7425",
          eoyBalance: 45232.12,
          forfeitures2024: 0,
          withdrawals2024: 0,
          balance: 45232.12,
          vestedPortion: 45232.12,
          singleLifeAnnuity: 45232.12,
          qualifiedJoinAndSurvivor: 40708.91
        },
        {
          badge: 82424,
          name: "BRAD, STEVEN",
          ssn: "123-45-2424",
          eoyBalance: 38901.45,
          forfeitures2024: 0,
          withdrawals2024: 0,
          balance: 38901.45,
          vestedPortion: 38901.45,
          singleLifeAnnuity: 38901.45,
          qualifiedJoinAndSurvivor: 35011.31
        },
        {
          badge: 85744,
          name: "BRADLEY, ZACHARY W",
          ssn: "123-45-5744",
          eoyBalance: 29876.33,
          forfeitures2024: 0,
          withdrawals2024: 0,
          balance: 29876.33,
          vestedPortion: 29876.33,
          singleLifeAnnuity: 29876.33,
          qualifiedJoinAndSurvivor: 26888.7
        },
        {
          badge: 94861,
          name: "COCHRAN, BRADIAN E",
          ssn: "123-45-4861",
          eoyBalance: 52103.78,
          forfeitures2024: 0,
          withdrawals2024: 0,
          balance: 52103.78,
          vestedPortion: 52103.78,
          singleLifeAnnuity: 52103.78,
          qualifiedJoinAndSurvivor: 46893.4
        }
      ] as ReprintCertificateEmployee[],
      total: 4
    };
  }, [filterParams]);

  useEffect(() => {
    onLoadingChange?.(isFetching);
  }, [isFetching, onLoadingChange]);

  useEffect(() => {
    if (filterParams) {
      setIsFetching(true);
      const timer = setTimeout(() => {
        setIsFetching(false);
      }, 800);

      return () => clearTimeout(timer);
    }
  }, [filterParams]);

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
  };

  const addRowToSelectedRows = useCallback((id: number) => {
    setSelectedRowIds((prev) => [...prev, id]);
  }, []);

  const removeRowFromSelectedRows = useCallback((id: number) => {
    setSelectedRowIds((prev) => prev.filter((rowId) => rowId !== id));
  }, []);

  const columnDefs = useMemo(
    () => GetReprintCertificatesGridColumns(selectedRowIds, addRowToSelectedRows, removeRowFromSelectedRows),
    [selectedRowIds, addRowToSelectedRows, removeRowFromSelectedRows]
  );

  const onSelectionChanged = useCallback((event: any) => {
    const selectedNodes = event.api.getSelectedNodes();
    const selectedIds = selectedNodes.map((node: any) => node.data.badge);
    setSelectedRowIds(selectedIds);
  }, []);

  const selectedCount = selectedRowIds.length;

  return (
    <>
      <div style={{ padding: "0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Print Profit Certificates ({mockData.total})
        </Typography>
      </div>

      {isFetching ? (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          py={4}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <DSMGrid
            preferenceKey="REPRINT_CERTIFICATES_GRID"
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: mockData.results || [],
              columnDefs: columnDefs,
              rowSelection: "multiple",
              suppressRowClickSelection: true,
              onSelectionChanged: onSelectionChanged
            }}
          />
          {mockData.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber + 1}
              setPageNumber={(value: number) => setPageNumber(value - 1)}
              pageSize={pageSize}
              setPageSize={setPageSize}
              recordCount={mockData.total}
            />
          )}
        </>
      )}
    </>
  );
};

export default ReprintCertificatesGrid;
