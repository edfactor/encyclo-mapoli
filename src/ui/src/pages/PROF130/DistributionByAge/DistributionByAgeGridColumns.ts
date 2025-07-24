import { agGridNumberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { ColDef, ColGroupDef } from "ag-grid-community";

export const GetDistributionsByAgeColumns = (reportType: FrozenReportsByAgeRequestType): (ColDef | ColGroupDef)[] => {
  const columns: (ColDef | ColGroupDef)[] = [
    {
      headerName: "A",
      groupId: "blag",
      columnGroupShow: "closed",
      headerValueGetter: (params) => {
        console.log("Header Value Getter for Group", params);
                // params.columnGroup will give you access to the column group object
                return 'Custom Group Name: ' + (params.columnGroup?.getGroupId() ?? 'No Group');
            },
      children: [
        {
          headerName: "Age",
          field: "age",
          colId: "age",
          minWidth: 80,
          type: "rightAligned",
          resizable: true,
          sort: "asc",
          cellDataType: "text"
          
        },
        {
          headerName: "EMPS",
          field: "regularEmployeeCount",
          colId: "regularEmployeeCount",
          minWidth: 100,
          type: "rightAligned",
          resizable: true
        },
        /*
        {
          headerName: "Amount",
          field: "regularAmount",
          colId: "regularAmount",
          minWidth: 150,
          type: "rightAligned",
          resizable: true,
          //columnGroupShow: "open",
          //valueFormatter: agGridNumberToCurrency
        }
          */
      ]
    }
  ];
  return columns;
};
