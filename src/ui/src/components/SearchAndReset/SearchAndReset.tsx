import Button from "@mui/material/Button";
import CircularProgress from "@mui/material/CircularProgress";
import Grid from "@mui/material/Grid";
import React, { FC } from "react";
import "./SearchButtons.css";
import { ICommon } from "../ICommon";

export interface ISearchAndResetProps extends ICommon {
  disabled?: boolean;
  handleSearch?: React.MouseEventHandler<HTMLButtonElement>;
  handleReset: React.MouseEventHandler<HTMLButtonElement>;
  isFetching: boolean;
  searchButtonText?: string;
}

export const SearchAndReset: FC<ISearchAndResetProps> = ({
  disabled = false,
  handleSearch,
  handleReset,
  isFetching,
  searchButtonText,
  ...props
}) => {
  return (
    <Grid
      xs={12}
      md={12}
      lg={12}
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
            searchButtonText || "Search"
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

export default SearchAndReset;
