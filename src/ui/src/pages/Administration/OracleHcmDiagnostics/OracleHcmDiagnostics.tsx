import { Box, Divider } from "@mui/material";
import { useCallback, useEffect, useRef, useState } from "react";
import { Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { SortParams } from "../../../hooks/useGridPagination";
import {
  useClearDemographicSyncAudit,
  useGetOracleHcmSyncMetadata,
  useLazyGetDemographicSyncAudit
} from "../../../reduxstore/api/hcmSyncApi";
import AuditGrid from "./AuditGrid";
import OracleHcmMetadata from "./OracleHcmMetadata";

const OracleHcmDiagnostics = () => {
  const { data: metadataData, isLoading: metadataLoading, refetch: refetchMetadata } = useGetOracleHcmSyncMetadata();
  const [triggerGetAudit, { data: auditData, isLoading: auditLoading }] = useLazyGetDemographicSyncAudit();
  const [, { isLoading: clearLoading }] = useClearDemographicSyncAudit();
  const hasInitiallyFetched = useRef(false);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortBy, setSortBy] = useState<string>("Created");
  const [isSortDescending, setIsSortDescending] = useState<boolean>(true);

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

  useEffect(() => {
    if (!hasInitiallyFetched.current) {
      hasInitiallyFetched.current = true;
      fetchAuditData(pageNumber, pageSize);
    }
  }, [fetchAuditData, pageNumber, pageSize]);

  const handleClearSuccess = () => {
    refetchMetadata();
    setPageNumber(0);
    fetchAuditData(0, pageSize);
  };

  const handlePageChange = (page: number, size: number) => {
    setPageNumber(page);
    setPageSize(size);
    fetchAuditData(page, size);
  };

  const handleSortChange = (sortParams: SortParams) => {
    setSortBy(sortParams.sortBy);
    setIsSortDescending(sortParams.isSortDescending);
    setPageNumber(0);
    fetchAuditData(0, pageSize, sortParams.sortBy, sortParams.isSortDescending);
  };

  return (
    <PageErrorBoundary pageName="Oracle HCM Diagnostics">
      <Page label="OracleHcm Diagnostics">
        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
          <Divider />

          {/* Metadata Section */}
          <OracleHcmMetadata
            metadata={metadataData}
            isLoading={metadataLoading}
            onRefresh={refetchMetadata}
          />

          <Divider />

          {/* Audit Grid Section */}
          <AuditGrid
            data={auditData}
            isLoading={auditLoading || clearLoading}
            onClearSuccess={handleClearSuccess}
            pageNumber={pageNumber}
            pageSize={pageSize}
            sortParams={{ sortBy, isSortDescending }}
            onPageChange={handlePageChange}
            onSortChange={handleSortChange}
          />
        </Box>
      </Page>
    </PageErrorBoundary>
  );
};

export default OracleHcmDiagnostics;
