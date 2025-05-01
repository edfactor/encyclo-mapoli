import { FormControl, MenuItem, Select, SelectChangeEvent } from "@mui/material";
import { useEffect, useRef } from "react";
import { useSelector } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import { RootState } from "reduxstore/store";
import { mmDDYYFormat } from "../../utils/dateUtils";
import InputLabel from "@mui/material/InputLabel";

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
  defaultValue
}: ProfitYearSelectorProps) => {
  const frozenStateCollectionData = useSelector((state: RootState) => state.frozen.frozenStateCollectionData);
  const token = useSelector((state: RootState) => state.security.token);
  const [triggerFrozenStateSearch, { isLoading }] = useLazyGetHistoricalFrozenStateResponseQuery();
  const thisYear = new Date().getFullYear();

  useEffect(() => {
    if (showDates && token) {
      triggerFrozenStateSearch({
        skip: 0,
        take: 1,
        sortBy: "createdDateTime",
        isSortDescending: true
      });
    }
  }, [showDates, triggerFrozenStateSearch, token]);

  const activeFrozenState = frozenStateCollectionData?.results?.find((state) => state.isActive);
  const hasFrozenData = frozenStateCollectionData?.results && frozenStateCollectionData.results.length > 0;

  const yearsToDisplay: number[] = [];
  if (hasFrozenData && activeFrozenState?.profitYear) {
    yearsToDisplay.push(activeFrozenState.profitYear);
  }

  if (!yearsToDisplay.includes(thisYear)) {
    yearsToDisplay.push(thisYear);
  }

  const formattedAsOfDate = activeFrozenState?.asOfDateTime ? ` - ${mmDDYYFormat(activeFrozenState.asOfDateTime)}` : "";

  const todayFormattedString = ` - ${mmDDYYFormat(new Date())}`;

  if (yearsToDisplay.length === 0) {
    yearsToDisplay.push(thisYear);
  }

  const initialSelectionMadeRef = useRef(false);

// Add useEffect to auto-select the only value if there's just one option
  useEffect(() => {
    // Only run auto-selection once, when API has completed and no defaultValue is provided
    if (!isLoading &&
      yearsToDisplay.length === 1 &&
      selectedProfitYear !== yearsToDisplay[0] &&
      !defaultValue &&
      !initialSelectionMadeRef.current) {

      // Create a synthetic event to mimic the Select's change event
      const syntheticEvent = {
        target: { value: yearsToDisplay[0].toString() }
      } as SelectChangeEvent;

      // Mark that we've done the initial selection
      initialSelectionMadeRef.current = true;
      handleChange(syntheticEvent);
    }
  }, [yearsToDisplay, selectedProfitYear, handleChange, isLoading, defaultValue]);

// Reset the ref when the component unmounts
  useEffect(() => {
    return () => {
      initialSelectionMadeRef.current = false;
    };
  }, []);


  return (
    <div className="flex items-center gap-2 h-10 min-w-[174px]">
      <FormControl
        fullWidth
        size="small">
        <InputLabel
          id="profit-year-label"
          sx={{ color: "black" }}>
          Profit Year
        </InputLabel>

        <Select
          labelId="profit-year-selector"
          id="profit-year-selector"
          defaultValue={defaultValue || selectedProfitYear.toString()}
          value={selectedProfitYear.toString()}
          label="Profit Year"
          size="small"
          fullWidth
          disabled={disabled || (disabledWhileLoading && isLoading)}
          onChange={handleChange}>
          {yearsToDisplay.map((year) => (
            <MenuItem
              key={year}
              value={year}>
              {year}
              {showDates && (
                <>
                  {year === thisYear && <span className="text-gray-600 ml-2">{todayFormattedString} (Live Data)</span>}
                  {year !== thisYear && activeFrozenState?.profitYear === year && (
                    <span className="text-gray-600 ml-2">{formattedAsOfDate} (Frozen)</span>
                  )}
                </>
              )}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </div>
  );
};

export default ProfitYearSelector;
