import Grid2 from "@mui/material/Unstable_Grid2";
import { isValid } from "date-fns";
import { useDispatch } from "react-redux";

import { useLazyGetNamesMissingCommasQuery } from "reduxstore/api/YearsEndApi";
import { clearMissingCommaInPYName } from "reduxstore/slices/yearsEndSlice";
import { SearchAndReset } from "smart-ui-library";
interface MissingCommaInPyNameSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const MissingCommaInPyNameSearchFilter: React.FC<MissingCommaInPyNameSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetNamesMissingCommasQuery();

  const search = () => {
    triggerSearch(
      {
        pagination: { skip: 0, take: 100 }
      },
      false
    );
  };

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearMissingCommaInPYName());
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
