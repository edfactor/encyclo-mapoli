import { ColDef } from "ag-grid-community";
import { yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { viewBadgeRenderer } from "../../utils/masterInquiryLink";

export const GetDuplicateSSNsOnDemographicsColumns = (): ColDef[] => {
    return [
      {
        headerName: "Badge",
        field: "badgeNumber",
        colId: "badgeNumber",
        minWidth: 80,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true,
        cellRenderer: (params) => viewBadgeRenderer({ value: params.data.badgeNumber }),
      },
      {
        headerName: "SSN",
        field: "ssn",
        colId: "ssn",
        minWidth: 100,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true
      },
      {
        headerName: "Name",
        field: "name",
        colId: "name",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true
      },
      {
        headerName: "Address",
        field: "address",
        colId: "address",
        minWidth: 200,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueGetter: (params) => {
          const addr = params.data.address;
          return addr ? `${addr.street}${addr.street2 ? ', ' + addr.street2 : ''}` : "";
        }
      },
      {
        headerName: "City",
        field: "city",
        colId: "city",
        minWidth: 120,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueGetter: (params) => params.data.address?.city || ""
      },
      {
        headerName: "State",
        field: "state",
        colId: "state",
        minWidth: 60,
        headerClass: "center-align",
        cellClass: "center-align",
        resizable: true,
        valueGetter: (params) => params.data.address?.state || ""
      },
      {
        headerName: "Hire",
        field: "hireDate",
        colId: "hireDate",
        minWidth: 100,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueFormatter: (params) => params.value ? yyyyMMDDToMMDDYYYY(params.value) : ""
      },
      {
        headerName: "Rehire",
        field: "rehireDate",
        colId: "rehireDate",
        minWidth: 100,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueFormatter: (params) => params.value ? yyyyMMDDToMMDDYYYY(params.value) : ""
      },
      {
        headerName: "St",
        field: "status",
        colId: "status",
        minWidth: 60,
        headerClass: "center-align",
        cellClass: "center-align",
        resizable: true
      },
      {
        headerName: "Str",
        field: "storeNumber",
        colId: "storeNumber",
        minWidth: 60,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true
      },
    ];
  };