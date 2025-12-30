import { ColDef, ValueFormatterParams } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn,
  createZipColumn
} from "../../../utils/gridColumnFactory";

export const GetMasterInquiryMemberGridColumns = (navigateFunction: (badgeNumber: string) => void): ColDef[] => {
  const badgeColumn = createBadgeColumn({
    headerName: "Badge/PSN",
    psnSuffix: true,
    navigateFunction: navigateFunction
  });

  return [
    badgeColumn,
    createNameColumn({ field: "fullName" }),
    createSSNColumn({
      maxWidth: 250,
      valueFormatter: (params: ValueFormatterParams) => {
        const value = params.value;
        if (value && params.data.badgesOfDuplicateSsns && params.data.badgesOfDuplicateSsns.length) {
          return value + " (duplicate)";
        }
        return value;
      }
    }),
    { field: "address", headerName: "Street", maxWidth: 400 },
    { field: "addressCity", headerName: "City", maxWidth: 300 },
    { field: "addressState", headerName: "State", maxWidth: 100 },
    createZipColumn({ field: "addressZipCode", headerName: "Zip", maxWidth: 160 }),
    createAgeColumn({}),
    createStatusColumn({
      field: "employmentStatus",
      valueFormatter: (params: ValueFormatterParams) => {
        const value = params.value;
        return value == null || value === undefined || value === "" ? "N/A" : value;
      }
    })
  ];
};
