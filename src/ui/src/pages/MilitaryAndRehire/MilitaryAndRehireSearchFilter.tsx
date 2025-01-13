import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useLazyGetMilitaryAndRehireQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";

const MilitaryAndRehireSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetMilitaryAndRehireQuery();

  return (
    <Grid2
      width="100%"
      paddingX="24px">
      <SearchAndReset
        handleReset={() => {}}
        handleSearch={() => {
          triggerSearch(
            {
              pagination: { skip: 0, take: 25 },
              
            },
            false
          );
        }}
        isFetching={isFetching}
      />
    </Grid2>
  );
};

export default MilitaryAndRehireSearchFilter;
