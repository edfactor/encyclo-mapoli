import Grid2 from "@mui/material/Unstable_Grid2";
import { useDispatch } from "react-redux";
import { useLazyGetEmployeesOnMilitaryLeaveQuery } from "reduxstore/api/YearsEndApi";
import { clearEmployeesOnMilitaryLeaveDetails } from "reduxstore/slices/yearsEndSlice";
import { SearchAndReset } from "smart-ui-library";

interface EmployeesOnMilitaryLeaveSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const EmployeesOnMilitaryLeaveSearchFilter: React.FC<EmployeesOnMilitaryLeaveSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetEmployeesOnMilitaryLeaveQuery();

  const dispatch = useDispatch();

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
    dispatch(clearEmployeesOnMilitaryLeaveDetails());
    // Leaving this stub here in case we do want this page to have search filters. If we don't, this entire file and
    // its reference in the MissingCommaInPyName page component.
  };

  return (
    <form onSubmit={search}>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={search}
          isFetching={isFetching}
        />
      </Grid2>
    </form>
  );
};

export default EmployeesOnMilitaryLeaveSearchFilter;
