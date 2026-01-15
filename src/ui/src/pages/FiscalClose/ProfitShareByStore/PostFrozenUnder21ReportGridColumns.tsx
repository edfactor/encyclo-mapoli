import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createSSNColumn,
  createStoreColumn,
  createYesOrNoColumn
} from "../../../utils/gridColumnFactory";

export const GetPostFrozenUnder21ReportColumnDefs = (): ColDef[] => {
  return [
    createStoreColumn({
      field: "storeNumber",
      headerName: "Store"
    }),
    createBadgeColumn({
      field: "badgeNumber"
    }),
    {
      headerName: "First Name",
      field: "firstName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Last Name",
      field: "lastName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    createSSNColumn({
      field: "ssn"
    }),
    createCountColumn({
      headerName: "PS Years",
      field: "profitSharingYears",
      minWidth: 90
    }),
    createYesOrNoColumn({
      headerName: "New",
      field: "isNew"
    }),
    createHoursColumn({
      headerName: "This Year Hours",
      field: "thisYearHours"
    }),
    createHoursColumn({
      headerName: "Last Year Hours",
      field: "lastYearHours"
    }),
    createDateColumn({
      headerName: "Hire Date",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Full Time Date",
      field: "fullTimeDate"
    }),
    createDateColumn({
      headerName: "Termination Date",
      field: "terminationDate"
    }),
    createDateColumn({
      headerName: "Birth Date",
      field: "dateOfBirth"
    }),
    createAgeColumn({
      field: "age"
    }),
    {
      headerName: "Status",
      field: "employmentStatusId",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true
    },
    createCurrencyColumn({
      headerName: "Current Balance",
      field: "currentBalance"
    }),
    createCountColumn({
      headerName: "Enrollment ID",
      field: "enrollmentId"
    })
  ];
};
