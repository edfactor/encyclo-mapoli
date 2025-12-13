import { Box, Divider } from "@mui/material";
import { useEffect } from "react";
import { Page } from "smart-ui-library";
import { useClearDemographicSyncAudit, useGetOracleHcmSyncMetadata, useLazyGetDemographicSyncAudit } from "../../../reduxstore/api/hcmSyncApi";
import AuditGrid from "./AuditGrid";
import OracleHcmMetadata from "./OracleHcmMetadata";

const OracleHcmDiagnostics = () => {
  const { data: metadataData, isLoading: metadataLoading, refetch: refetchMetadata } = useGetOracleHcmSyncMetadata();
  const [triggerGetAudit, { data: auditData, isLoading: auditLoading }] = useLazyGetDemographicSyncAudit();
  const [, { isLoading: clearLoading }] = useClearDemographicSyncAudit();

  useEffect(() => {
    triggerGetAudit({ pageNumber: 1, pageSize: 50 });
  }, [triggerGetAudit]);

  const handleClearSuccess = () => {
    refetchMetadata();
    triggerGetAudit({ pageNumber: 1, pageSize: 50 });
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
