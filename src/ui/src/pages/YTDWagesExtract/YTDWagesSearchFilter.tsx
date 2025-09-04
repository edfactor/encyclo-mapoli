import { Button, CircularProgress, FormLabel, Grid, MenuItem, Select } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";
import { setEmployeeWagesForYearQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SortedPaginationRequestDto } from "../../reduxstore/types";

interface YTDWagesSearch {
  profitYear: number;
  pagination: SortedPaginationRequestDto;
}

interface YTDWagesSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  setPageReset: (reset: boolean) => void;
}

const YTDWagesSearchFilter: React.FC<YTDWagesSearchFilterProps> = ({ setInitialSearchLoaded, setPageReset }) => {
  const [triggerSearch, { isFetching }] = useLazyGetEmployeeWagesForYearQuery();
  const { employeeWagesForYearQueryParams, employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();

  const { handleSubmit } = useForm<YTDWagesSearch>({
    defaultValues: {
      profitYear: fiscalCloseProfitYear || employeeWagesForYearQueryParams?.profitYear || undefined,
      pagination: { skip: 0, take: 25, sortBy: "storeNumber", isSortDescending: false }
    }
  });

  const doSearch = handleSubmit((_data) => {
    // Our one-select 'form' cannot be in an invalid state, so we can safely trigger the search
    setPageReset(true);
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear,
        pagination: { skip: 0, take: 25, sortBy: "storeNumber", isSortDescending: false },
        acceptHeader: "application/json"
      },
      false
    ).unwrap();
    dispatch(setEmployeeWagesForYearQueryParams(fiscalCloseProfitYear));
  });

  useEffect(() => {
    if (fiscalCloseProfitYear && !employeeWagesForYear) {
      setInitialSearchLoaded(true);
    }
  }, [fiscalCloseProfitYear, employeeWagesForYear, setInitialSearchLoaded]);

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
            value={fiscalCloseProfitYear}
            disabled={true}
            fullWidth>
            <MenuItem value={fiscalCloseProfitYear}>{fiscalCloseProfitYear}</MenuItem>
          </Select>
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <div className="search-buttons mt-5 flex justify-start">
          <Button
            variant="contained"
            disabled={isFetching}
            data-testid="searchButton"
            type="submit"
            onClick={doSearch}
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
              "Search"
            )}
          </Button>
        </div>
      </Grid>
    </form>
  );
};

export default YTDWagesSearchFilter;
