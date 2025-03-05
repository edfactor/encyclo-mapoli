import Grid2 from "@mui/material/Unstable_Grid2";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { clearDuplicateSSNsData } from "reduxstore/slices/yearsEndSlice";
import { SearchAndReset } from "smart-ui-library";

interface DuplicateSSNsOnDemographicsSearch {}

interface DuplicateSSNsOnDemographicsSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const DuplicateSSNsOnDemographicsSearchFilter: React.FC<DuplicateSSNsOnDemographicsSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetDuplicateSSNsQuery();
  const dispatch = useDispatch();
  const {
    handleSubmit,
    formState: { isValid },
    reset
  } = useForm<DuplicateSSNsOnDemographicsSearch>({});

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
    setInitialSearchLoaded(false);
    dispatch(clearDuplicateSSNsData());
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
