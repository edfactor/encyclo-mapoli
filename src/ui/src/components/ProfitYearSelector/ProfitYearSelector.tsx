import { FormControl, MenuItem, Select, SelectChangeEvent } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import { useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";
import { RootState } from "reduxstore/store";
import InputLabel from "@mui/material/InputLabel";
import { mmDDYYFormat } from "../../utils/dateUtils";

export interface ProfitYearSelectorProps {
  selectedProfitYear: number;
  handleChange: (event: SelectChangeEvent) => void;
  showDates?: boolean;
  disabled?: boolean;
  disabledWhileLoading?: boolean;
  defaultValue?: string;
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
  const [triggerFrozenStateSearch, { isLoading: isFrozenLoading }] = useLazyGetHistoricalFrozenStateResponseQuery();
  const thisYear = new Date().getFullYear();

  // Get frozen profit year if available
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

  // Build years to display
  const yearsToDisplay: number[] = [];
  if (hasFrozenData && activeFrozenState?.profitYear) {
    yearsToDisplay.push(activeFrozenState.profitYear);
  }
  if (!yearsToDisplay.includes(thisYear)) {
    yearsToDisplay.push(thisYear);
  }
  if (yearsToDisplay.length === 0) {
    yearsToDisplay.push(thisYear);
  }

  // Fetch accounting year data for each year
  const [accountingYearData, setAccountingYearData] = useState<Record<number, { startDate: string; endDate: string }>>({});
  const [fetchAccountingYear] = useLazyGetAccountingYearQuery();

  useEffect(() => {
    yearsToDisplay.forEach((year) => {
      if (!accountingYearData[year]) {
        fetchAccountingYear({ profitYear: year }).unwrap().then((data) => {
          setAccountingYearData((prev) => ({
            ...prev,
            [year]: {
              startDate: data.fiscalBeginDate,
              endDate: data.fiscalEndDate
            }
          }));
        });
      }
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [yearsToDisplay.join(","), fetchAccountingYear]);

  const initialSelectionMadeRef = useRef(false);

  useEffect(() => {
    if (
      !isFrozenLoading &&
      yearsToDisplay.length === 1 &&
      selectedProfitYear !== yearsToDisplay[0] &&
      !defaultValue &&
      !initialSelectionMadeRef.current
    ) {
      const syntheticEvent = {
        target: { value: yearsToDisplay[0].toString() }
      } as SelectChangeEvent;
      initialSelectionMadeRef.current = true;
      handleChange(syntheticEvent);
    }
  }, [yearsToDisplay, selectedProfitYear, handleChange, isFrozenLoading, defaultValue]);

  useEffect(() => {
    return () => {
      initialSelectionMadeRef.current = false;
    };
  }, []);

  return (
    <div className="flex items-center gap-2 h-10 min-w-[174px]">
      <FormControl fullWidth size="small">
        <InputLabel id="profit-year-label" sx={{ color: "black" }}>
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
          disabled={disabled || (disabledWhileLoading && isFrozenLoading)}
          onChange={handleChange}
          sx={{ fontSize: '0.9rem' }} // Shrinks font in the select input
          MenuProps={{
            PaperProps: {
              sx: {
                fontSize: '0.9rem', // Shrinks font in the dropdown menu
              },
            },
          }}
        >
        {yearsToDisplay.map((year) => (
            <MenuItem key={year} value={year}>
              {year} -
              {showDates && accountingYearData[year] && (
                <span className="text-gray-500 ml-1">
                  {mmDDYYFormat(accountingYearData[year].startDate)} - {mmDDYYFormat(accountingYearData[year].endDate)}
                </span>
              )}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </div>
  );
};

export default ProfitYearSelector;