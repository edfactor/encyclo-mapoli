import {useState, useMemo} from "react";
import {useSelector} from "react-redux";
import {RootState} from "reduxstore/store";
import {DSMGrid, ISortParams, Pagination} from "smart-ui-library";
import {ProfitShareUpdateGridColumns} from "./ProfitShareUpdateGridColumns";

const ProfitShareUpdateGrid = () => {
    const [pageNumber, setPageNumber] = useState(0);
    const [pageSize, setPageSize] = useState(25);
    const [sortParams, setSortParams] = useState<ISortParams>({
        sortBy: "Name",
        isSortDescending: false
    });
    const columnDefs = useMemo(() => ProfitShareUpdateGridColumns(), []);
    const {profitSharingUpdate} = useSelector((state: RootState) => state.yearsEnd);

    return (
        <> {!!profitSharingUpdate && (
            <DSMGrid
                preferenceKey={"ProfitShareUpdateGrid"}
                isLoading={profitSharingUpdate?.isLoading}
                providedOptions={{
                    rowData: profitSharingUpdate?.response?.results,
                    columnDefs: columnDefs
                }}
            />
        )}
        </>
    );
};

export default ProfitShareUpdateGrid;
