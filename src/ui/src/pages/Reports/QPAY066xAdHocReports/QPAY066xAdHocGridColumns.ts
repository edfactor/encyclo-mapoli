import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createPercentageColumn
} from "../../../utils/gridColumnFactory";

export const GetQPAY066xAdHocGridColumns = (): ColDef[] => [
  createBadgeColumn({}),
  createNameColumn({
    field: "fullName"
  }),
  createCurrencyColumn({
    headerName: "Beginning Balance",
    field: "beginningBalance",
    minWidth: 130
  }),
  createCurrencyColumn({
    headerName: "Beneficiary Allocation",
    field: "beneficiaryAllocation"
  }),
  createCurrencyColumn({
    headerName: "Distribution Amount",
    field: "distributions"
  }),
  createCurrencyColumn({
    headerName: "Forfeit",
    field: "forfeitures"
  }),
  createCurrencyColumn({
    headerName: "Ending Balance",
    field: "endingBalance"
  }),
  createCurrencyColumn({
    headerName: "Vesting Balance",
    field: "vestedAmount"
  }),
  createDateColumn({
    headerName: "Term Date",
    field: "terminationDate",
    minWidth: 100
  }),
  createHoursColumn({
    headerName: "YTD Hours",
    field: "profitShareHours",
    minWidth: 90
  }),
  createPercentageColumn({
    headerName: "Vested",
    field: "vestedPercentage"
  }),
  createAgeColumn({}),
  // This is the age at termination
  // FIXME: This logic should be moved to the back end when more work is done on this page
  createAgeColumn({
    headerName: "Age at Term",
    valueGetter: (params) => {
      const dob = params.data?.["dateOfBirth"];
      const termDate = params.data?.["terminationDate"];
      if (!dob || !termDate) return 0;
      const birthDate = new Date(dob);
      const terminationDate = new Date(termDate);
      let age = terminationDate.getFullYear() - birthDate.getFullYear();
      const m = terminationDate.getMonth() - birthDate.getMonth();
      if (m < 0 || (m === 0 && terminationDate.getDate() < birthDate.getDate())) {
        age--;
      }
      return age;
    }
  })
];
