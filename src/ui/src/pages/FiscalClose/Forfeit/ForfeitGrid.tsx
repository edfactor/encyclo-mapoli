import { ValidationIcon, ValidationResultsDialog } from "@/components/ValidationIcon";
import { Table, TableBody, TableCell, TableHead, TableRow } from "@mui/material";
import { useCallback, useMemo, useState } from "react";
import { Path, useNavigate } from "react-router";
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
}

const ForfeitGrid: React.FC<ForfeitGridProps> = ({ searchResults, pagination, isSearching }) => {
  const navigate = useNavigate();

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [validationDialogField, setValidationDialogField] = useState<string | null>(null);  

  const handleValidationClick = useCallback((fieldName: string) => {
    setValidationDialogField(fieldName);
    setIsDialogOpen(true);
  }, []);

  const columnDefs = useMemo(
    () => GetProfitShareForfeitColumns({
      navFunction: handleNavigationForButton,
      onValidationClick: handleValidationClick
    }),
    [handleNavigationForButton, handleValidationClick]
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
  const totalForfeituresRaw = safeNumber(searchResults?.totalForfeitures);
  const totalForfeitPoints = safeNumber(searchResults?.totalForfeitPoints);
  const totalEarningPoints = safeNumber(searchResults?.totalEarningPoints);

  const totalsRow = useMemo(
    () => ({
      forfeitures: (totalForfeituresRaw.toFixed(2)),
      contForfeitPoints: totalForfeitPoints,
      earningPoints: totalEarningPoints,
      validation: searchResults?.crossReferenceValidation?.validationGroups[0] || null
    }),
    [totalForfeituresRaw, totalForfeitPoints, totalEarningPoints]
  );

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
      beforeGrid={
        <>
          <ValidationResultsDialog open={isDialogOpen} 
                                   onClose={()=> setIsDialogOpen(false)} 
                                   validationGroup={searchResults?.crossReferenceValidation?.validationGroups[0]}
                                   fieldName = {validationDialogField} />
          <div className="sticky top-0 z-10 flex bg-white">
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell className="align-middle text-center">Profit Sharing Amount</TableCell>
                  <TableCell className="align-middle text-center">Distribution Amount</TableCell>
                  <TableCell className="align-middle text-center">Allocation To</TableCell>
                  <TableCell className="align-middle text-center">Allocation From</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                <TableRow>
                  <TableCell className="align-middle text-center">{numberToCurrency(searchResults.totalProfitSharingBalance || 0)}</TableCell>
                  <TableCell className="align-middle text-center">
                    <ValidationIcon 
                      validationGroup={searchResults.crossReferenceValidation?.validationGroups[0]}
                      fieldName="QPAY129_DistributionTotals"
                      onClick={()=>
                        {
                          setValidationDialogField("QPAY129_DistributionTotals")
                          setIsDialogOpen(!isDialogOpen);
                        }
                      }
                    />
                    {numberToCurrency(searchResults.distributionTotals || 0)}
                  </TableCell>
                  <TableCell className="align-middle text-center">{numberToCurrency(searchResults.allocationToTotals || 0)}</TableCell>
                  <TableCell className="align-middle text-center">{numberToCurrency(searchResults.allocationsFromTotals || 0)}</TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </div>
          <ReportSummary report={searchResults} />
        </>
      }
      gridOptions={{
        pinnedTopRowData: [totalsRow]
      }}
      className="relative"
    />
  );
};

export default ForfeitGrid;
