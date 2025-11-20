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

export interface BreakdownByStoreEmployee {
  badgeNumber: number;
  beginningBalance: number;
  beneficiaryAllocation: number;
  certificateSort: number;
  city: string;
  contributions: number;
  dateOfBirth: string;
  departmentId: number;
  distributions: number;
  earnings: number;
  employmentStatusId: string;
  endingBalance: number;
  enrollmentId: number;
  forfeitures: number;
  fullName: string;
  hireDate: string;
  isExecutive: boolean;
  payClassificationId: string; // changed from number to string to match backend refactor
  payClassificationName: string;
  payFrequencyId: number;
  postalCode: string;
  profitShareHours: number;
  ssn: string;
  state: string;
  storeNumber: number;
  street1: string;
  terminationDate: string;
  vestedAmount: number;
  vestedPercentage: number;
}

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
  // This is the person's age calculated from dateOfBirth
  createAgeColumn({
    field: "dateOfBirth"
  }),
  // This is the age at termination
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
