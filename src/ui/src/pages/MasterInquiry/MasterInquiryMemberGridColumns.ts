import { ColDef, ICellRendererParams, ValueFormatterParams } from "ag-grid-community";

export const GetMasterInquiryMemberGridColumns = (): ColDef[] => {
    return [
    {
        field: "badgeNumber",
        headerName: "Badge",
        maxWidth: 120,
        cellRenderer: (params: ICellRendererParams) => {
        const { badgeNumber, psnSuffix } = params.data;
        return psnSuffix > 0 ? `${badgeNumber}-${psnSuffix}` : badgeNumber;
        }
    },
    { field: "fullName", headerName: "Name", maxWidth: 500 },
    { field: "ssn", headerName: "SSN", maxWidth: 250 },
    { field: "address", headerName: "Street", maxWidth: 400 },
    { field: "addressCity", headerName: "City", maxWidth: 300 },
    { field: "addressState", headerName: "State", maxWidth: 100 },
    { field: "addressZipCode", headerName: "Zip", maxWidth: 160 },
    { field: "age", headerName: "Age", maxWidth: 120, },
    { field: "employmentStatus", headerName: "Status", maxWidth: 120,
        valueFormatter: (params: ValueFormatterParams) => {
            const value = params.value;
            return value == null || value === undefined || value === "" ? "N/A" : value;
        }
    }
    ]
};