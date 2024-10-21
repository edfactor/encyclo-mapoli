import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { useLazyGetMilitaryAndRehireQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { ImpersonationRoles } from "reduxstore/types";

interface MilitaryAndRehireSearch {
  profitYear: number;
}

const MilitaryAndRehireSearchFilter = () => {
  const [isFetching, setIsFetching] = useState(false);

  const [triggerSearch, { isLoading }] = useLazyGetMilitaryAndRehireQuery();

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
              impersonation: ImpersonationRoles.ProfitSharingAdministrator
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
