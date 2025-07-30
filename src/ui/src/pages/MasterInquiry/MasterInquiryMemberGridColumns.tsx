import { ColDef, ValueFormatterParams } from "ag-grid-community";
import { createSSNColumn } from "../../utils/gridColumnFactory";

export const GetMasterInquiryMemberGridColumns = (): ColDef[] => {
  return [
    {
      field: "badgeNumber",
      headerName: "Badge",
      maxWidth: 120,
      cellRenderer: (params: any) => {
        const { badgeNumber, psnSuffix, isEmployee, id } = params.data;
        return (
          <a
            href="#"
            className="badge-link"
            onClick={(e) => {
              e.preventDefault();
              if (params.context && params.context.onBadgeClick) {
                params.context.onBadgeClick({ memberType: isEmployee ? 1 : 2, id, badgeNumber, psnSuffix });
              }
            }}>
            {psnSuffix > 0 ? `${badgeNumber}-${psnSuffix}` : badgeNumber}
          </a>
        );
      }
    },
    { field: "fullName", headerName: "Name", maxWidth: 500 },
    createSSNColumn({ maxWidth: 250 }),
    { field: "address", headerName: "Street", maxWidth: 400 },
    { field: "addressCity", headerName: "City", maxWidth: 300 },
    { field: "addressState", headerName: "State", maxWidth: 100 },
    { field: "addressZipCode", headerName: "Zip", maxWidth: 160 },
    { field: "age", headerName: "Age", maxWidth: 120 },
    {
      field: "employmentStatus",
      headerName: "Status",
      maxWidth: 120,
      valueFormatter: (params: ValueFormatterParams) => {
        const value = params.value;
        return value == null || value === undefined || value === "" ? "N/A" : value;
      }
    }
  ];
};
