import { Box, Divider } from "@mui/material";
import { useCallback, useEffect, useRef } from "react";
import { Page } from "smart-ui-library";
import { useClearDemographicSyncAudit, useGetOracleHcmSyncMetadata, useLazyGetDemographicSyncAudit } from "../../../reduxstore/api/hcmSyncApi";
import AuditGrid from "./AuditGrid";
import OracleHcmMetadata from "./OracleHcmMetadata";

const OracleHcmDiagnostics = () => {
  const { data: metadataData, isLoading: metadataLoading, refetch: refetchMetadata } = useGetOracleHcmSyncMetadata();
  const [triggerGetAudit, { data: auditData, isLoading: auditLoading }] = useLazyGetDemographicSyncAudit();
  const [, { isLoading: clearLoading }] = useClearDemographicSyncAudit();
  const hasInitiallyFetched = useRef(false);

  const fetchAuditData = useCallback((pageNumber: number = 1, pageSize: number = 50) => {
    triggerGetAudit({ pageNumber, pageSize }, false);
  }, [triggerGetAudit]);

  useEffect(() => {
    if (!hasInitiallyFetched.current) {
      hasInitiallyFetched.current = true;
      fetchAuditData();
    }
  }, [fetchAuditData]);

  const handleClearSuccess = () => {
    refetchMetadata();
    fetchAuditData();
  };

  const handlePageChange = (page: number, pageSize: number) => {
    fetchAuditData(page, pageSize);
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
        />
      </Box>
    </Page>
  );
};


export default OracleHcmDiagnostics;
