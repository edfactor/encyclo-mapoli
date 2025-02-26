import {useState, useMemo} from "react";
import {useSelector} from "react-redux";
import {RootState} from "reduxstore/store";
import {DSMGrid, ISortParams, Pagination} from "smart-ui-library";
import {ProfitShareUpdateGridColumns} from "./ProfitShareUpdateGridColumns";
import {ProfitShareEditGridColumns} from "./ProfitShareEditGridColumns";

const ProfitShareUpdateGrid = () => {
    const [pageNumber, setPageNumber] = useState(0);
    const [pageSize, setPageSize] = useState(25);
    const [sortParams, setSortParams] = useState<ISortParams>({
        sortBy: "Name",
        isSortDescending: false
    });
    const columnDefs = useMemo(() => ProfitShareUpdateGridColumns(), []);
    const editColumnDefs = useMemo(() => ProfitShareEditGridColumns(), []);
    const {profitSharingUpdate} = useSelector((state: RootState) => state.yearsEnd);

    return (
        <>
            <h1>{profitSharingUpdate?.reportName}</h1>
            {!!profitSharingUpdate && profitSharingUpdate?.reportName == "Profit Sharing Update" && (
                <DSMGrid
                    preferenceKey={"ProfitShareUpdateGrid"}
                    isLoading={profitSharingUpdate?.isLoading}
                    providedOptions={{
                        rowData: 'response' in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
                        columnDefs: columnDefs
                    }}
                />
            )}
            {!!profitSharingUpdate && profitSharingUpdate?.reportName == "Profit Sharing Edit" && (
                <DSMGrid
                    preferenceKey={"ProfitShareEditGrid"}
                    isLoading={profitSharingUpdate?.isLoading}
                    providedOptions={{
                        rowData: 'response' in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
                        columnDefs: editColumnDefs
                    }}
                />
            )}
            {!!profitSharingUpdate && (profitSharingUpdate?.reportName == "Apply" || profitSharingUpdate?.reportName == "Revert") && profitSharingUpdate?.isLoading &&
                (<h2>Loading ...</h2>
                )}
            {!!profitSharingUpdate && (profitSharingUpdate?.reportName == "Apply" || profitSharingUpdate?.reportName == "Revert") && profitSharingUpdate?.isLoading != true &&
                (<div>
                    <br/>       
                    Beneficiaries Effected: { profitSharingUpdate.beneficiariesEffected }<br />
                    Employees Effected: { profitSharingUpdate.employeesEffected }<br />
                    Etvas Effected: { profitSharingUpdate.etvasEffected }<br />
                    </div>
                )}
        </>
    );
};

export default ProfitShareUpdateGrid;
