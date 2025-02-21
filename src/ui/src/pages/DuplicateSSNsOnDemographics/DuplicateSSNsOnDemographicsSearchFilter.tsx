import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useForm } from "react-hook-form";
import { useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";

interface DuplicateSSNsOnDemographicsSearch {}

const DuplicateSSNsOnDemographicsSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetDuplicateSSNsQuery();

  const {
    handleSubmit,
    formState: { isValid },
    reset    
  } = useForm<DuplicateSSNsOnDemographicsSearch>({  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          pagination: { skip: 0, take: 25 }
        },
        false
      );
    }
  });

  const handleReset = () => {
    reset();
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={false}
        />
      </Grid2>
    </form>
  );
};

export default DuplicateSSNsOnDemographicsSearchFilter;
