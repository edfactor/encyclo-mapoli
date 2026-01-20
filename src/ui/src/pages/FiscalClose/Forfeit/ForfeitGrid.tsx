import { ValidationIcon, ValidationResultsDialog } from "@/components/ValidationIcon";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import { IconButton, Typography } from "@mui/material";
import { useCallback, useMemo, useState } from "react";
import { numberToCurrency } from "smart-ui-library";
import DSMPaginatedGrid from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import ReportSummary from "../../../components/ReportSummary";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { ForfeituresAndPointsResponse } from "../../../types";
import { GetProfitShareForfeitColumns } from "./ForfeitGridColumns";

interface ForfeitGridProps {
  searchResults: ForfeituresAndPointsResponse | null;
  pagination: ReturnType<typeof useGridPagination>;
  isSearching: boolean;
  isGridExpanded?: boolean;
  onToggleExpand?: () => void;
}

const ForfeitGrid: React.FC<ForfeitGridProps> = ({
  searchResults,
  pagination,
  isSearching,
  isGridExpanded = false,
  onToggleExpand
}) => {
  const [dialogState, setDialogState] = useState<{
    isOpen: boolean;
    fieldName: string | null;
  }>({ isOpen: false, fieldName: null });

  const handleValidationClick = useCallback((fieldName: string) => {
    setDialogState({ isOpen: true, fieldName });
  }, []);

  const columnDefs = useMemo(
    () =>
      GetProfitShareForfeitColumns({
        onValidationClick: handleValidationClick
      }),
    [handleValidationClick]
  );

  // Custom sort handler for compound sort on badgeOrPsn column
  const handleSortChange = useCallback(
    (update: SortParams) => {
      // if field is badgeOrPsn, we need to make sortBy equal to badgeNumber,beneficiaryPsn
      // to get a compound sort
      if (update.sortBy === "badgeOrPsn") {
        const newUpdate = {
          ...update,
          sortBy: update.isSortDescending ? "beneficiaryPsn" : "badgeNumber"
        };
        pagination.handleSortChange(newUpdate);
        return;
      }
      pagination.handleSortChange(update);
    },
    [pagination]
  );

  // Some API responses may return numeric totals as strings; coerce safely before formatting.
  const safeNumber = (val: unknown) => {
    const n = typeof val === "number" ? val : parseFloat(val as string);
    return Number.isFinite(n) ? n : 0;
  };
  const totalEarningPoints = safeNumber(searchResults?.totalEarningPoints);

  if (!searchResults?.response) return null;

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.FORFEIT}
      data={searchResults.response.results}
      columnDefs={columnDefs}
      totalRecords={searchResults.response.total}
      isLoading={isSearching}
      pagination={pagination}
      onSortChange={handleSortChange}
      header={<ReportSummary report={searchResults} />}
      headerActions={
        onToggleExpand && (
          <IconButton
            onClick={onToggleExpand}
            sx={{ zIndex: 1 }}
            title={isGridExpanded ? "Exit fullscreen" : "Expand fullscreen"}>
            {isGridExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
          </IconButton>
        )
      }
      heightConfig={{
        mode: "content-aware",
        heightPercentage: isGridExpanded ? 0.85 : 0.4
      }}
      beforeGrid={
        !isGridExpanded ? (
          <>
            <ValidationResultsDialog
              open={dialogState.isOpen}
              onClose={() => setDialogState({ isOpen: false, fieldName: null })}
              validationGroup={searchResults?.crossReferenceValidation?.validationGroups[0]}
              fieldName={dialogState.fieldName}
            />
            <div className="sticky top-0 z-10 bg-white">
              <Typography
                variant="h6"
                sx={{ mb: 1, px: 3 }}>
                Totals
              </Typography>
              {/* One-off inline styles for this simple table. If we reuse this pattern, move to a shared CSS module. */}
              <style>{`
                .forfeit-totals-table {
                  width: 100%;
                  border-collapse: collapse;
                }
                .forfeit-totals-table thead tr {
                  background-color: #E8E8E8;
                }
                .forfeit-totals-table th,
                .forfeit-totals-table td {
                  padding: 0.5rem 1rem;
                  text-align: right;
                  font-size: 0.875rem;
                }
                .forfeit-totals-table th {
                  font-weight: 500;
                }
              `}</style>
              <div className="rounded border border-gray-300 mx-3">
                <table className="forfeit-totals-table">
                  <thead>
                    <tr>
                      <th>Profit Sharing Amount</th>
                      <th>Distribution Amount</th>
                      <th>Allocation To</th>
                      <th>Allocation From</th>
                      <th>Earning Points</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>{numberToCurrency(searchResults.totalProfitSharingBalance || 0)}</td>
                      <td>
                        <ValidationIcon
                          validationGroup={searchResults.crossReferenceValidation?.validationGroups[0]}
                          fieldName="QPAY129_DistributionTotals"
                          onClick={() => {
                            setDialogState({
                              isOpen: !dialogState.isOpen,
                              fieldName: "QPAY129_DistributionTotals"
                            });
                          }}
                        />
                        {numberToCurrency(searchResults.distributionTotals || 0)}
                      </td>
                      <td>{numberToCurrency(searchResults.allocationToTotals || 0)}</td>
                      <td>{numberToCurrency(searchResults.allocationsFromTotals || 0)}</td>
                      <td>{totalEarningPoints.toLocaleString()}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </>
        ) : (
          <ValidationResultsDialog
            open={dialogState.isOpen}
            onClose={() => setDialogState({ isOpen: false, fieldName: null })}
            validationGroup={searchResults?.crossReferenceValidation?.validationGroups[0]}
            fieldName={dialogState.fieldName}
          />
        )
      }
      className="relative"
    />
  );
};

export default ForfeitGrid;
