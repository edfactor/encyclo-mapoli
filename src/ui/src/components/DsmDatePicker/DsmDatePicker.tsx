import { FormHelperText, FormLabel, TextField } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFnsV3";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { FC, KeyboardEvent, useState } from "react";
import { parseISO } from "date-fns";

type MyProps = {
  id: string;
  onChange: (value: Date | null) => void;
  value: Date | null;
  disableFuture?: boolean;
  error?: string;
  setError?: Function;
  required: boolean;
  label: string;
  onKeyDown?: Function;
  ref?: React.ForwardedRef<unknown>;
  views?: Array<"year" | "month" | "day">;
};

const DsmDatePicker: FC<MyProps> = ({
  error,
  views,
  onChange,
  value,
  disableFuture,
  setError,
  required,
  label,
  onKeyDown,
  ref,
  id
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
      const swapped = [parts[2], parts[0], parts[1]];
      const yyyymmdd = swapped.join("-");
      const v = tryddmmyyyyToDate(yyyymmdd);
      onChange(v);
    }
  };

  const tryddmmyyyyToDate = (date: string | null): Date | null => {
    if (!date || date === "YYYY-MM-DD") return null;
    return parseISO(date);
  };

  const CustomTextField = (params: any) => (
    <TextField
      {...params}
      inputRef={ref}
      id={id}
      data-your-attrib={id}
      fullWidth
      error={isInvalid}
      onError={(err) => {}}
      onKeyDown={(e: KeyboardEvent<HTMLDivElement>) => {
        if (onKeyDown) {
          onKeyDown(e);
        }
      }}
      onBlur={handleTextFieldBlur}
    />
  );

  return (
    <div className="dsm-date-picker">
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <div className="date-picker-label-wrapper">
          <FormLabel
            className={isInvalid ? "" : "date-picker-label"}
            required={required ?? false}
            error={isInvalid}>
            {label}
          </FormLabel>
        </div>
        <DatePicker
          sx={{ width: "100%" }}
          slots={{
            textField: CustomTextField
          }}
          onAccept={(e) => {
            onChange(e);
          }}
          views={views}
          openTo={views?.[0]}
          disableFuture={disableFuture}
          value={value}
        />
      </LocalizationProvider>
      {!!error && <FormHelperText error>{error}</FormHelperText>}
    </div>
  );
};

export default DsmDatePicker;
