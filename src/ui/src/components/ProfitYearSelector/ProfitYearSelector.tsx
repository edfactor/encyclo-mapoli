import { MenuItem, Select, SelectChangeEvent } from "@mui/material";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/ItOperations";
import { RootState } from "reduxstore/store";
import { mmDDYYFormat } from "../../utils/dateUtils";

/**
 * TODO/Future improvement to be made:
 * This current implementation has some assumptions about which years to show and
 * how to display dates for different years (current year shows today's date, past years show
 * frozen data if available). This is likely fine for now, but for better long-term maintainability, consider:
 * 
 * 1. Making the available years API-driven instead of calculating relative to current year
 * 2. API also provides the frozen date for each year for display in the selector.
 * 3. ProfitYearSelector component then just receives the list of years and the frozen date for each year and sets in redux.
 */


export interface ProfitYearSelectorProps {
  selectedProfitYear: number;
  handleChange: (event: SelectChangeEvent) => void;
  showDates?: boolean;
  disabled?: boolean;
  disabledWhileLoading?: boolean;
  defaultValue?: string;
  displayYears?: number[];
}

const ProfitYearSelector = ({
  selectedProfitYear,
  handleChange,
  showDates = true,
  disabled = false,
  disabledWhileLoading = true,
  defaultValue,
  displayYears
}: ProfitYearSelectorProps) => {
  const frozenStateCollectionData = useSelector((state: RootState) => state.frozen.frozenStateCollectionData);
  const token = useSelector((state: RootState) => state.security.token);
  const [triggerFrozenStateSearch, { isLoading }] = useLazyGetHistoricalFrozenStateResponseQuery();
  const currentYear = new Date().getFullYear();
  
  const effectiveDisplayYears = displayYears || [currentYear - 1, currentYear, currentYear + 1]; // TODO: This can be driven by the API response
  
  useEffect(() => {
    if (showDates && token) {
      triggerFrozenStateSearch({
        skip: 0,
        take: 1,
        sortBy: "createdDateTime",
        isSortDescending: true
      });
    }
    // token in dependencies for issues where profit year selector is on first page mounted
  }, [showDates, triggerFrozenStateSearch, token]);
  
  const activeFrozenState = frozenStateCollectionData?.results?.find(state => state.isActive);
  const activeProfitYear = activeFrozenState?.profitYear;
  const formattedAsOfDate = activeFrozenState?.asOfDateTime 
    ? ` - ${mmDDYYFormat(activeFrozenState.asOfDateTime)}` 
    : '';
  
  const todayFormattedString = ` - ${mmDDYYFormat(new Date())}`;

  // TODO: the above formatting could also be provided by the API response
  
  return (
    <div className="flex items-center gap-2 h-10 min-w-[174px]">
      <Select
        defaultValue={defaultValue || selectedProfitYear.toString()}
        value={selectedProfitYear.toString()}
        size="small"
        fullWidth
        disabled={disabled || (disabledWhileLoading && isLoading)}
        onChange={handleChange}>
        {effectiveDisplayYears.map(year => (
          <MenuItem key={year} value={year}>
            {year}
            {showDates && (
              <>
                {year === currentYear && todayFormattedString}
                {year < currentYear && (activeProfitYear === year) && formattedAsOfDate}
              </>
            )}
          </MenuItem>
        ))}
      </Select>
    </div>
  );
};

export default ProfitYearSelector; 