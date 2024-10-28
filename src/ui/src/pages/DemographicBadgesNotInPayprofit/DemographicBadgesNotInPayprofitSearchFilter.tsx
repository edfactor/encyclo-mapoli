import { FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { isValid } from "date-fns";
import { useState } from "react";
import {
  useLazyGetDemographicBadgesNotInPayprofitQuery,
} from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { ImpersonationRoles } from "reduxstore/types";

const DemographicBadgesNotInPayprofitSearchFilter = () => {
  const [isFetching, setIsFetching] = useState(false);

  const [triggerSearch, { isLoading }] = useLazyGetDemographicBadgesNotInPayprofitQuery();

  const validateAndSearch = (event: any) => {
    event.preventDefault();
    triggerSearch(
      { pagination: { skip: 0, take: 25 }, impersonation: ImpersonationRoles.ProfitSharingAdministrator },
      false
    );
  };

  const handleReset = () => {
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
