import { Button, CircularProgress, FormLabel, MenuItem, Select } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";
import { setEmployeeWagesForYearQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";

interface YTDWagesSearch {
  profitYear: number;
}

interface YTDWagesSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const YTDWagesSearchFilter: React.FC<YTDWagesSearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const [triggerSearch, { isFetching }] = useLazyGetEmployeeWagesForYearQuery();
  const { employeeWagesForYearQueryParams, employeeWagesForYear } = useSelector((state: RootState) => state.yearsEnd);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();

  const { handleSubmit, setValue } = useForm<YTDWagesSearch>({
    defaultValues: {
      profitYear: fiscalCloseProfitYear || employeeWagesForYearQueryParams?.profitYear || undefined
    }
  });

  const doSearch = handleSubmit((data) => {
    // Our one-select 'form' cannot be in an invalid state, so we can safely trigger the search
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear,
        pagination: { skip: 0, take: 25 },
        acceptHeader: "application/json"
      },
      false
    ).unwrap();
    dispatch(setEmployeeWagesForYearQueryParams(fiscalCloseProfitYear));
  });

  if (fiscalCloseProfitYear && !employeeWagesForYear) {
    setInitialSearchLoaded(true);
  }

  return (
    <form onSubmit={doSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Profit Year</FormLabel>
          <Select
            size="small"
            value={fiscalCloseProfitYear}
            disabled={true}
            fullWidth>
            <MenuItem value={fiscalCloseProfitYear}>{fiscalCloseProfitYear}</MenuItem>
          </Select>
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <div className="search-buttons flex mt-5 justify-start">
          <Button
            variant="contained"
            disabled={false || isFetching}
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
      </Grid2>
    </form>
  );
};

export default YTDWagesSearchFilter;
