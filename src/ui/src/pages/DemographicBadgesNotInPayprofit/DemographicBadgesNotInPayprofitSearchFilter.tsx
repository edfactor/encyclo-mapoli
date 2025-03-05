import { FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { isValid } from "date-fns";
import { useDispatch } from "react-redux";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "reduxstore/api/YearsEndApi";
import { clearDemographicBadgesNotInPayprofitData } from "reduxstore/slices/yearsEndSlice";
import { SearchAndReset } from "smart-ui-library";

interface DemographicBadgesNotInPayprofitSearchFilterProps {
  setProfitYear: (year: number) => void;
  setInitialSearchLoaded: (include: boolean) => void;
}

const DemographicBadgesNotInPayprofitSearchFilter: React.FC<DemographicBadgesNotInPayprofitSearchFilterProps> = ({
  setProfitYear,
  setInitialSearchLoaded
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetDemographicBadgesNotInPayprofitQuery();
  const dispatch = useDispatch();

  const validateAndSearch = (event: any) => {
    event.preventDefault();
    triggerSearch({ pagination: { skip: 0, take: 25 } }, false);
  };

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearDemographicBadgesNotInPayprofitData());
    // TODO - handle reset
  };

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
          <FormLabel>Year</FormLabel>

          <TextField
            fullWidth
            variant="outlined"
            onChange={(e) => {
              setProfitYear(Number(e.target.value));
            }}
            inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
          />
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default DemographicBadgesNotInPayprofitSearchFilter;
