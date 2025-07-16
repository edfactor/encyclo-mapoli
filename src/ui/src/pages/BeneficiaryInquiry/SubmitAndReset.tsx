import Button from "@mui/material/Button";
import CircularProgress from "@mui/material/CircularProgress";
import Grid from "@mui/material/Grid2";
import React, { FC } from "react";
import "./SubmitAndReset.css";

export interface ISubmitAndResetProps {
  disabled?: boolean;
  handleSearch?: React.MouseEventHandler<HTMLButtonElement>;
  handleReset: React.MouseEventHandler<HTMLButtonElement>;
  isFetching: boolean;
}

export const SubmitAndReset: FC<ISubmitAndResetProps> = ({
  disabled = false,
  handleSearch,
  handleReset,
  isFetching,
  ...props
}) => {
  return (
    <Grid
      size={12}
      {...props}>
      <div
        className="search-buttons"
        style={{
          display: "flex",
          justifyContent: "left",
          marginTop: "16px"
        }}>
        <Button
          variant="contained"
          disabled={disabled || isFetching}
          data-testid="searchButton"
          type="submit"
          onClick={handleSearch}
          sx={{
            "&.Mui-disabled": {
              background: "#eaeaea",
              color: "#c0c0c0"
            }
          }}>
          {isFetching ? (
            //Prevent loading spinner from shrinking button
            <div className="spinner">
              <CircularProgress
                color="inherit"
                size="20px"
              />
            </div>
          ) : (
            "Submit"
          )}
        </Button>
        <Button
          className="reset"
          type="reset"
          variant="contained"
          data-testid="resetButton"
          onClick={handleReset}>
          Reset
        </Button>
      </div>
    </Grid>
  );
};

export default SubmitAndReset;
