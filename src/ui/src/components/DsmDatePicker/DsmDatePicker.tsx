import { FormHelperText, FormLabel, TextField } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFnsV3";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { FC, KeyboardEvent } from "react";
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
};

const DsmDatePicker: FC<MyProps> = (props) => {
  const isInvalid = props.error ? props.error?.length > 0 : false;

  const handleTextFieldBlur = (e: React.FocusEvent<HTMLInputElement>) => {
    const ddmmyyyyString = e.target.value;
    const parts = ddmmyyyyString.split("/");
    const swapped = [parts[2], parts[0], parts[1]];
    const yyyymmdd = swapped.join("-");
    const v = tryddmmyyyyToDate(yyyymmdd);
    props.onChange(v);
  };

  const tryddmmyyyyToDate = (date: string | null): Date | null => {
    if (!date || date === "YYYY-MM-DD") {
      return null;
    }
    const result = parseISO(date);
    return result;
  };

  const CustomTextField = (inputProps: object) => (
    <TextField
      {...inputProps}
      inputRef={props.ref}
      id={props.id}
      data-your-attrib={props.id}
      fullWidth
      error={isInvalid}
      onError={(err) => {}}
      onKeyDown={(e: KeyboardEvent<HTMLDivElement>) => {
        if (props.onKeyDown) {
          props.onKeyDown(e);
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
            required={props.required ?? false}
            error={isInvalid}>
            {props.label}
          </FormLabel>
        </div>
        <DatePicker
          sx={{ width: "100%" }}
          slots={{
            textField: CustomTextField
          }}
          onAccept={(e) => {
            props.onChange(e);
          }}
          disableFuture={props.disableFuture}
          value={props.value}
        />
      </LocalizationProvider>
      {!!props.error && <FormHelperText error>{props.error}</FormHelperText>}
    </div>
  );
};

export default DsmDatePicker;
