import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../../constants";

export const GetMilitaryAndRehireColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Name",
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
      minWidth: GRID_COLUMN_WIDTHS.SSN,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};
