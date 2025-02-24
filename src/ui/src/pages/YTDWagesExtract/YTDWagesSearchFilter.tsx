import { Button, CircularProgress, MenuItem, Select, SelectChangeEvent } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useLazyGetEmployeeWagesForYearQuery } from "reduxstore/api/YearsEndApi";

interface YTDWagesSearch {
  profitYear: number;
}

const YTDWagesSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetEmployeeWagesForYearQuery();

  const thisYear = new Date().getFullYear();
  const lastYear = thisYear - 1;

  const [chosenYear, setChosenYear] = useState<number>(lastYear);

  const {
    control,
    handleSubmit,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    formState: { errors, isValid },
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    reset,
    setValue
  } = useForm<YTDWagesSearch>({
    defaultValues: {
      profitYear: chosenYear
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    console.log("Validating and submitting for profit year: " + data.profitYear);

    // Our form cannot be in an invalid state, so we can safely trigger the search
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
    { value: lastYear, label: `${lastYear}` },
    { value: thisYear, label: `${thisYear}` }
  ];

  return (
    <form onSubmit={validateAndSearch}>
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
              console.log("Changing year selection!");
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
            onClick={validateAndSearch}
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
