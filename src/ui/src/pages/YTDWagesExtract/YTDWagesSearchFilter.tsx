import { Button, CircularProgress, FormLabel, Grid, MenuItem, Select } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useCallback } from "react";
import { useForm } from "react-hook-form";
import { YTDWagesSearchParams } from "./hooks/useYTDWages";

interface YTDWagesSearchFilterProps {
  onSearch: (params: YTDWagesSearchParams) => void;
  isSearching?: boolean;
}

const YTDWagesSearchFilter: React.FC<YTDWagesSearchFilterProps> = ({ onSearch, isSearching = false }) => {
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const { handleSubmit } = useForm({
    defaultValues: {
      profitYear: fiscalCloseProfitYear
    }
  });

  const doSearch = useCallback(
    handleSubmit(() => {
      if (fiscalCloseProfitYear) {
        onSearch({ profitYear: fiscalCloseProfitYear });
      }
    }),
    [handleSubmit, fiscalCloseProfitYear, onSearch]
  );

  return (
    <form onSubmit={doSearch}>
      <Grid
        container
        paddingX="24px"
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Profit Year</FormLabel>
          <Select
            size="small"
            value={fiscalCloseProfitYear || ""}
            disabled={true}
            fullWidth>
            <MenuItem value={fiscalCloseProfitYear || ""}>{fiscalCloseProfitYear}</MenuItem>
          </Select>
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <div className="search-buttons mt-5 flex justify-start">
          <Button
            variant="contained"
            disabled={isSearching || !fiscalCloseProfitYear}
            data-testid="searchButton"
            type="submit"
            onClick={doSearch}
            sx={{
              "&.Mui-disabled": {
                background: "#eaeaea",
                color: "#c0c0c0"
              }
            }}>
            {isSearching ? (
              <div className="spinner">
                <CircularProgress
                  color="inherit"
                  size="20px"
                />
              </div>
            ) : (
              "Search"
            )}
          </Button>
        </div>
      </Grid>
    </form>
  );
};

export default YTDWagesSearchFilter;
