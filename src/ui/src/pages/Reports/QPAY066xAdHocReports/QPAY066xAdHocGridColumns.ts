import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createPercentageColumn,
  createStatusColumn
} from "../../../utils/gridColumnFactory";


export const GetQPAY066xAdHocCommonGridColumns = (): ColDef[] => [
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
    headerName: "Forfeit",
    field: "forfeitures"
  }),
  createCurrencyColumn({
    headerName: "Ending Balance",
    field: "endingBalance"
  }),
  createCurrencyColumn({
    headerName: "Vested Balance",
    field: "vestedAmount"
  }),
  createHoursColumn({
    headerName: "YTD Hours",
    field: "profitShareHours",
    minWidth: 90
  })
];

export const GetQPAY066xDistributionAmountColumn = (field: string): ColDef => {
  return createCurrencyColumn({
    headerName: "Distribution Amount",
    field: field || "distributions"
  });
}

export const GetQPAY066xBeneficiaryAllocationColumn = (field: string): ColDef => {
  return createCurrencyColumn({
    headerName: "Beneficiary Allocation",
    field: field || "beneficiaryAllocation"
  });
}

export const GetQPAY066xTerminationDateColumn = (field: string): ColDef => {
  return createDateColumn({
    headerName: "Termination Date",
    field: field || "terminationDate"
  });
}

export const GetQPAY066xVestedBalanceColumn = (field: string): ColDef => {
  return createPercentageColumn({
    headerName: "Vested Balance",
    field: field || "vestedAmount"
  });
}

export const GetQPAY066xAgeColumn = (): ColDef => {
  return createAgeColumn({});
}

// FIXME: This calculation should be done on the back end
export const GetQPAY066xAgeAtTerminationColumn = (): ColDef => {
  return createAgeColumn({
    headerName: "Age at Termination",
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
  });
}

export const GetQPAY066xEnrollmentCodeColumn = (field: string): ColDef => {
  return createCountColumn({
    headerName: "EC",
    field: field || "enrollmentCode"
  });
}

export const GetQPAY066xPSYearsColumn = (field: string): ColDef => {
  return createCountColumn({
    headerName: "PS Years",
    field: field || "profitShareYears"
  });
}

export const GetQPAY066xTerminationCodeColumn = (field: string): ColDef => {
  return createStatusColumn({
    headerName: "Termination Code",
    field: field || "terminationCode"
  });
}

export const GetQPAY066xInactiveDateColumn = (field: string): ColDef => {
  return createDateColumn({
    headerName: "Inactive Date",
    field: field || "inactiveDate"
  });
}


