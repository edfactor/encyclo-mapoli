import DeleteIcon from "@mui/icons-material/Delete";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography
} from "@mui/material";
import { useMemo, useState } from "react";
import { Paged } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { SortParams } from "../../../hooks/useGridPagination";
import { useClearDemographicSyncAudit } from "../../../reduxstore/api/hcmSyncApi";
import type { DemographicSyncAuditRecord } from "../../../types";
import { GetAuditGridColumns } from "./AuditGridColumns";

interface AuditGridProps {
  data?: Paged<DemographicSyncAuditRecord>;
  isLoading: boolean;
  onClearSuccess: () => void;
  pageNumber: number;
  pageSize: number;
  sortParams: SortParams;
  onPageNumberChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const AuditGrid: React.FC<AuditGridProps> = ({
  data,
  isLoading,
  onClearSuccess,
  pageNumber,
  pageSize,
  sortParams,
  onPageNumberChange,
  onPageSizeChange,
  onSortChange
}) => {
  const [showClearConfirmation, setShowClearConfirmation] = useState(false);
  const [clearError, setClearError] = useState<string | null>(null);

  const [triggerClear, { isLoading: isClearing }] = useClearDemographicSyncAudit();

  const columnDefs = useMemo(() => GetAuditGridColumns(), []);

  const handleClearClick = () => {
    setShowClearConfirmation(true);
    setClearError(null);
  };

  const handleConfirmClear = async () => {
    try {
      setClearError(null);
      await triggerClear().unwrap();
      setShowClearConfirmation(false);
      onClearSuccess();
    } catch (error: unknown) {
      setClearError((error as Error)?.message || "Failed to clear sync error records");
    }
  };

  return (
    <Box sx={{ padding: "0 24px" }}>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "16px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Demographic Sync Errors
        </Typography>
        <Button
          variant="contained"
          color="error"
          startIcon={<DeleteIcon />}
          onClick={handleClearClick}
          disabled={isLoading || isClearing || (data?.results?.length ?? 0) === 0}>
          Clear Sync Error Records
        </Button>
      </Box>

      {clearError && (
        <Alert
          severity="error"
          sx={{ marginBottom: "16px" }}
          onClose={() => setClearError(null)}>
          {clearError}
        </Alert>
      )}

      {isLoading ? (
        <CircularProgress />
      ) : data && (data.results?.length ?? 0) > 0 ? (
        <DSMPaginatedGrid
          preferenceKey={GRID_KEYS.DEMOGRAPHIC_SYNC_AUDIT}
          data={data.results ?? []}
          columnDefs={columnDefs}
          totalRecords={data.total}
          isLoading={isLoading || isClearing}
          pagination={{
            pageNumber,
            pageSize,
            sortParams,
            handlePageNumberChange: onPageNumberChange,
            handlePageSizeChange: onPageSizeChange,
            handleSortChange: onSortChange
          }}
          showPagination={(data.results?.length ?? 0) > 0}
          gridOptions={{
            suppressMoveWhenRowDragging: true,
            enableCellTextSelection: true
          }}
        />
      ) : (
        <Typography
          variant="body1"
          sx={{ color: "#666", textAlign: "center", padding: "32px" }}>
          No sync errors found
        </Typography>
      )}

      {/* Clear Confirmation Dialog */}
      <Dialog
        open={showClearConfirmation}
        onClose={() => setShowClearConfirmation(false)}
        maxWidth="xs"
        fullWidth>
        <DialogTitle>Clear Demographic Sync Errors</DialogTitle>
        <DialogContent>
          <Box sx={{ paddingTop: "16px" }}>
            <Typography variant="body1">
              Are you sure you want to delete all {data?.total || 0} sync error records?
            </Typography>
            <Typography
              variant="body2"
              sx={{ color: "#666", marginTop: "8px" }}>
              This action cannot be undone.
            </Typography>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setShowClearConfirmation(false)}
            disabled={isClearing}>
            Cancel
          </Button>
          <Button
            onClick={handleConfirmClear}
            variant="contained"
            color="error"
            disabled={isClearing}>
            {isClearing ? <CircularProgress size={20} /> : "Delete"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AuditGrid;
