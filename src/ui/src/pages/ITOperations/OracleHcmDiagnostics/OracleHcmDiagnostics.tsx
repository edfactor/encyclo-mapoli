import { Box, Divider } from "@mui/material";
import { useCallback, useEffect, useRef, useState } from "react";
import { Page } from "smart-ui-library";
import { useClearDemographicSyncAudit, useGetOracleHcmSyncMetadata, useLazyGetDemographicSyncAudit } from "../../../reduxstore/api/hcmSyncApi";
import AuditGrid from "./AuditGrid";
import OracleHcmMetadata from "./OracleHcmMetadata";

const OracleHcmDiagnostics = () => {
  const { data: metadataData, isLoading: metadataLoading, refetch: refetchMetadata } = useGetOracleHcmSyncMetadata();
  const [triggerGetAudit, { data: auditData, isLoading: auditLoading }] = useLazyGetDemographicSyncAudit();
  const [, { isLoading: clearLoading }] = useClearDemographicSyncAudit();
  const hasInitiallyFetched = useRef(false);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const fetchAuditData = useCallback((page: number, size: number) => {
    triggerGetAudit({ pageNumber: page + 1, pageSize: size }, false);
  }, [triggerGetAudit]);

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

  return (
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
          onPageChange={handlePageChange}
        />
      </Box>
    </Page>
  );
};


export default OracleHcmDiagnostics;
