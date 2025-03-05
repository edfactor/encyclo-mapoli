import { Button, CircularProgress, MenuItem, Select, SelectChangeEvent } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useForm } from "react-hook-form";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";

interface YTDWagesSearch {
  profitYear: number;
}

interface YTDWagesSearchFilterProps {
  originalYear: number;
  setChosenYear: (year: number) => void;
}

const YTDWagesSearchFilter: React.FC<YTDWagesSearchFilterProps> = ({ originalYear, setChosenYear }) => {
  const [triggerSearch, { isFetching }] = useLazyGetEmployeeWagesForYearQuery();

  const { handleSubmit, setValue } = useForm<YTDWagesSearch>({
    defaultValues: {
      profitYear: originalYear
    }
  });

  const doSearch = handleSubmit((data) => {
    // Our one-select 'form' cannot be in an invalid state, so we can safely trigger the search
    triggerSearch(
      {
        profitYear: data.profitYear,
        pagination: { skip: 0, take: 25 },
        acceptHeader: "application/json"
      },
      false
    );
  });

  const options = [
    { value: originalYear, label: `${originalYear}` },
    { value: originalYear + 1, label: `${originalYear + 1}` }
  ];

  return (
    <form onSubmit={doSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2
          xs={12}
          sm={6}
          md={3}>
          <Select
            size="small"
            defaultValue={options[0].value}
            onChange={(e: SelectChangeEvent<number>) => {
              e.preventDefault();
              setChosenYear(Number(e.target.value));
              setValue("profitYear", Number(e.target.value));
            }}
            fullWidth>
            {options.map((option) => (
              <MenuItem
                key={option.value}
                value={option.value}>
                {option.label}
              </MenuItem>
            ))}
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
