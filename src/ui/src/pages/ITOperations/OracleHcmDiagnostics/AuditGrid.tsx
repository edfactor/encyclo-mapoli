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
import { DSMGrid, Paged, Pagination } from "smart-ui-library";
import { SortParams } from "../../../hooks/useGridPagination";
import { GRID_KEYS } from "../../../constants";
import { useClearDemographicSyncAudit } from "../../../reduxstore/api/hcmSyncApi";
import type { DemographicSyncAuditRecord } from "../../../types";
import { GetAuditGridColumns } from "./AuditGridColumns";

interface AuditGridProps {
  data?: Paged<DemographicSyncAuditRecord>;
  isLoading: boolean;
  onClearSuccess: () => void;
  pageNumber: number;
  pageSize: number;
  onPageChange: (page: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const AuditGrid: React.FC<AuditGridProps> = ({
  data,
  isLoading,
  onClearSuccess,
  pageNumber,
  pageSize,
  onPageChange,
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
      setClearError((error as Error)?.message || "Failed to clear audit records");
    }
  };

  return (
    <Box sx={{ padding: "0 24px" }}>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "16px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Demographic Sync Audit
        </Typography>
        <Button
          variant="contained"
          color="error"
          startIcon={<DeleteIcon />}
          onClick={handleClearClick}
          disabled={isLoading || isClearing || (data?.results?.length ?? 0) === 0}>
          Clear Audit Records
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
        <>
          <DSMGrid
            preferenceKey={GRID_KEYS.DEMOGRAPHIC_SYNC_AUDIT}
            isLoading={isLoading}
            handleSortChanged={onSortChange}
            providedOptions={{
              rowData: data.results,
              columnDefs: columnDefs,
              suppressMoveWhenRowDragging: true,
              enableCellTextSelection: true
            }}
          />
          {data && (data.results?.length ?? 0) > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                onPageChange(value - 1, pageSize);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                onPageChange(0, value);
              }}
              recordCount={data.total}
            />
          )}
        </>
      ) : (
        <Typography
          variant="body1"
          sx={{ color: "#666", textAlign: "center", padding: "32px" }}>
          No audit records found
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
              Are you sure you want to delete all {data?.total || 0} audit records?
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
