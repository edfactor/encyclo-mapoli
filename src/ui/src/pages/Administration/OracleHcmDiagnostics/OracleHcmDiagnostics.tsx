import { Box, Divider } from "@mui/material";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { SortParams } from "../../../hooks/useGridPagination";
import { useGetOracleHcmSyncMetadata, useLazyGetDemographicSyncAudit } from "../../../reduxstore/api/hcmSyncApi";
import AuditGrid from "./AuditGrid";
import OracleHcmMetadata from "./OracleHcmMetadata";

const OracleHcmDiagnostics = () => {
  const { data: metadataData, isLoading: metadataLoading, refetch: refetchMetadata } = useGetOracleHcmSyncMetadata();
  const [triggerGetAudit, { data: auditData, isLoading: auditLoading }] = useLazyGetDemographicSyncAudit();
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortBy, setSortBy] = useState<string>("Created");
  const [isSortDescending, setIsSortDescending] = useState<boolean>(true);

  const pageNumberRef = useRef(pageNumber);
  const pageSizeRef = useRef(pageSize);

  useEffect(() => {
    pageNumberRef.current = pageNumber;
  }, [pageNumber]);

  useEffect(() => {
    pageSizeRef.current = pageSize;
  }, [pageSize]);

  const fetchAuditData = useCallback(
    (page: number, size: number, sort?: string, desc?: boolean) => {
      triggerGetAudit(
        {
          skip: Math.max(0, page) * size,
          take: size,
          sortBy: sort ?? sortBy,
          isSortDescending: desc ?? isSortDescending
        },
        false
      );
    },
    [triggerGetAudit, sortBy, isSortDescending]
  );

  const refreshAll = useCallback(
    (pageOverride?: number) => {
      refetchMetadata();

      const pageToFetch = pageOverride ?? pageNumberRef.current;
      fetchAuditData(pageToFetch, pageSizeRef.current);
    },
    [fetchAuditData, refetchMetadata]
  );

  useEffect(() => {
    fetchAuditData(pageNumber, pageSize);
  }, [fetchAuditData, pageNumber, pageSize]);

  const handleClearSuccess = () => {
    pageNumberRef.current = 0;
    setPageNumber(0);

    refreshAll(0);
  };

  const handlePageNumberChange = useCallback(
    (page: number) => {
      pageNumberRef.current = page;
      setPageNumber(page);
      fetchAuditData(page, pageSizeRef.current);
    },
    [fetchAuditData]
  );

  const handlePageSizeChange = useCallback(
    (size: number) => {
      pageNumberRef.current = 0;
      pageSizeRef.current = size;

      setPageNumber(0);
      setPageSize(size);
      fetchAuditData(0, size);
    },
    [fetchAuditData]
  );

  const handleSortChange = (sortParams: SortParams) => {
    setSortBy(sortParams.sortBy);
    setIsSortDescending(sortParams.isSortDescending);

    pageNumberRef.current = 0;
    setPageNumber(0);
    fetchAuditData(0, pageSizeRef.current, sortParams.sortBy, sortParams.isSortDescending);
  };

  const currentSortParams = useMemo(() => ({ sortBy, isSortDescending }), [sortBy, isSortDescending]);

  return (
    <PageErrorBoundary pageName="Oracle HCM Diagnostics">
      <Page label="OracleHcm Diagnostics">
        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
          <Divider />

          {/* Metadata Section */}
          <OracleHcmMetadata
            metadata={metadataData}
            isLoading={metadataLoading}
            onRefresh={refreshAll}
          />

          <Divider />

          {/* Audit Grid Section */}
          <AuditGrid
            data={auditData}
            isLoading={auditLoading}
            onClearSuccess={handleClearSuccess}
            pageNumber={pageNumber}
            pageSize={pageSize}
            sortParams={currentSortParams}
            onPageNumberChange={handlePageNumberChange}
            onPageSizeChange={handlePageSizeChange}
            onSortChange={handleSortChange}
          />
        </Box>
      </Page>
    </PageErrorBoundary>
  );
};

export default OracleHcmDiagnostics;
