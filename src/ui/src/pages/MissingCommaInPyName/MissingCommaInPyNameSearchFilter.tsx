import Grid2 from "@mui/material/Unstable_Grid2";
import { isValid } from "date-fns";
import { useState } from "react";
import { useLazyGetNamesMissingCommasQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";

const MissingCommaInPyNameSearchFilter = () => {
  const [isFetching, setIsFetching] = useState(false);

  const [triggerSearch, { isLoading }] = useLazyGetNamesMissingCommasQuery();

  const search = () => {
    setIsFetching(true);
    triggerSearch(
      {
        pagination: { skip: 0, take: 25 }
      },
      false
    );
    setIsFetching(false);
  };

  const handleReset = () => {
    // Leaving this stub here in case we do want this page to have search filters. If we don't, this entire file and
    // its reference in the MissingCommaInPyName page component.
  };

  return (
    <Grid2
      width="100%"
      paddingX="24px">
      <SearchAndReset
        handleReset={handleReset}
        handleSearch={search}
        isFetching={isFetching}
        disabled={!isValid}
      />
    </Grid2>
  );
};

export default MissingCommaInPyNameSearchFilter;
