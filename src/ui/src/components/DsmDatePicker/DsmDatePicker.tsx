import { FormLabel } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFnsV3";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { parseISO } from "date-fns";
import { FC, KeyboardEvent } from "react";

type DsmDatePickerProps = {
  id: string;
  onChange: (value: Date | null) => void;
  value: Date | null;
  disableFuture?: boolean;
  error?: string;
  setError?: (error: string) => void;
  required: boolean;
  label: string;
  onKeyDown?: (event: KeyboardEvent<HTMLElement>) => void;
  ref?: React.ForwardedRef<unknown>;
  views?: Array<"year" | "month" | "day">;
  disabled?: boolean;
  minDate?: Date;
  maxDate?: Date;
  shouldDisableMonth?: (month: Date) => boolean;
};

const DsmDatePicker: FC<DsmDatePickerProps> = ({
  error,
  views,
  onChange,
  value,
  disableFuture,
  required,
  label,
  onKeyDown,
  ref,
  id,
  disabled,
  minDate = new Date(new Date().getFullYear() - 6, 0, 1),
  maxDate,
  shouldDisableMonth
}) => {
  const isInvalid = error ? error?.length > 0 : false;
  const isYearOnly = views?.length === 1 && views[0] === "year";

  const handleTextFieldBlur = (e: React.FocusEvent<HTMLInputElement>) => {
    const value = e.target.value.trim();

    if (isYearOnly) {
      const year = parseInt(value);
      if (!isNaN(year) && year >= 1900 && year <= 2100) {
        onChange(new Date(year, 0, 1));
      }
    } else {
      const parts = value.split("/");
      if (parts.length === 3) {
        const swapped = [parts[2], parts[0], parts[1]];
        const yyyymmdd = swapped.join("-");
        const v = tryddmmyyyyToDate(yyyymmdd);
        onChange(v);
      }
    }
  };

  const tryddmmyyyyToDate = (date: string | null): Date | null => {
    if (!date || date === "YYYY-MM-DD") return null;
    return parseISO(date);
  };

  return (
    <div>
      <FormLabel
        required={required}
        error={isInvalid}
        style={{ display: "block" }}>
        {label}
      </FormLabel>

      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <DatePicker
          value={value}
          onChange={onChange}
          slotProps={{
            textField: {
              id,
              fullWidth: true,
              error: isInvalid,
              onBlur: handleTextFieldBlur,
              onKeyDown: (e) => onKeyDown?.(e),
              inputRef: ref,
              size: "small",
              variant: "outlined"
            }
          }}
          views={views}
          openTo={views?.[0]}
          disableFuture={disableFuture}
          disabled={disabled}
          minDate={minDate || undefined}
          maxDate={maxDate || undefined}
          shouldDisableMonth={shouldDisableMonth}
        />
      </LocalizationProvider>
    </div>
  );
};

export default DsmDatePicker;
