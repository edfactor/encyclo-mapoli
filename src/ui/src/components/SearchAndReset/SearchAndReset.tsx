import Button from "@mui/material/Button";
import CircularProgress from "@mui/material/CircularProgress";
import Grid from "@mui/material/Grid";
import React, { FC } from "react";
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
      <div className="mt-4 flex justify-start [&>button]:mr-5">
        <Button
          variant="contained"
          disabled={disabled || isFetching}
          data-testid="searchButton"
          type="submit"
          onClick={handleSearch}
          sx={{
            borderRadius: "2px",
            background: "#2e7d32",
            border: "none",
            display: "flex",
            padding: "8px 22px",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            color: "white",
            fontFamily: "Lato",
            fontSize: "15px",
            fontWeight: 400,
            lineHeight: "26px",
            letterSpacing: "0.46px",
            textTransform: "uppercase",
            "&.Mui-disabled": {
              background: "#eaeaea",
              color: "#c0c0c0"
            }
          }}>
          {isFetching ? (
            //Prevent loading spinner from shrinking button
            <div className="min-w-[61px]">
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
          type="reset"
          variant="contained"
          data-testid="resetButton"
          onClick={handleReset}
          sx={{
            color: "#2e7d32",
            background: "none",
            border: "none",
            borderRadius: "2px",
            display: "flex",
            padding: "8px 22px",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            fontFamily: "Lato",
            fontSize: "15px",
            fontWeight: 400,
            lineHeight: "26px",
            letterSpacing: "0.46px",
            textTransform: "uppercase",
            width: "69px",
            "&:hover": {
              backgroundColor: "#d3d3d3"
            }
          }}>
          Reset
        </Button>
      </div>
    </Grid>
  );
};

export default SearchAndReset;
