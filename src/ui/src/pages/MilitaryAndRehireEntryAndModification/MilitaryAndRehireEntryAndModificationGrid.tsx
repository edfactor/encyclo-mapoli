import { Typography } from "@mui/material";
import { useState, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetNegativeEVTASSNQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import MilitaryAndRehireEntryAndModificationEmployeeDetails from "./MilitaryAndRehireEntryAndModificationEmployeeDetails";

const MilitaryAndRehireEntryAndModificationGrid = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const dispatch = useDispatch();
  const { militaryAndRehireEntryAndModification } = useSelector((state: RootState) => state.yearsEnd);
  const [_, { isLoading }] = useLazyGetNegativeEVTASSNQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  return (
    <>
      {!!militaryAndRehireEntryAndModification && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              "Military and Rehire - ENTRY AND MOD"
            </Typography>
          </div>
          <MilitaryAndRehireEntryAndModificationEmployeeDetails details={militaryAndRehireEntryAndModification} />
        </>
      )}
    </>
  );
};

export default MilitaryAndRehireEntryAndModificationGrid;
