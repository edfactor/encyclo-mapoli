import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../utils/masterInquiryLink";
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetMilitaryAndRehireForfeituresColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Full Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 60,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Rehired Date",
      field: "reHiredDate",
      colId: "reHiredDate",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Contribution Years",
      field: "companyContributionYears",
      colId: "companyContributionYears",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Hours Current Year",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Forfeiture",
      field: "details.0.forfeiture",
      colId: "details.0.forfeiture",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Remark",
      field: "details.0.remark",
      colId: "details.0.remark",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Profit Year",
      field: "details.0.profitYear",
      colId: "details.0.profitYear",
      minWidth: 60,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};
