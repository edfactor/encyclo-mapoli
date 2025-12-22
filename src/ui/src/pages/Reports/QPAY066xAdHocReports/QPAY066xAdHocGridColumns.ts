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
  createBadgeColumn({
    maxWidth: 110
  }),
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
    field: "forfeitures",
    maxWidth: 120
  }),
  createCurrencyColumn({
    headerName: "Ending Balance",
    field: "endingBalance",
    maxWidth: 150
  }),
  createCurrencyColumn({
    headerName: "Vested Balance",
    field: "vestedAmount",
    maxWidth: 150
  }),
  createHoursColumn({
    headerName: "YTD Hours",
    field: "profitShareHours",
    maxWidth: 125
  })
];

export const GetQPAY066xDistributionAmountColumn = (field?: string): ColDef => {
  return createCurrencyColumn({
    headerName: "Distribution Amount",
    field: field || "distributions",
    maxWidth: 140
  });
};

export const GetQPAY066xBeneficiaryAllocationColumn = (field?: string): ColDef => {
  return createCurrencyColumn({
    headerName: "Beneficiary Allocation",
    field: field || "beneficiaryAllocation",
    maxWidth: 160
  });
};

export const GetQPAY066xTerminationDateColumn = (field?: string): ColDef => {
  return createDateColumn({
    headerName: "Termination Date",
    field: field || "terminationDate",
    maxWidth: 160
  });
};

export const GetQPAY066xVestedPercentageColumn = (field?: string): ColDef => {
  return createPercentageColumn({
    headerName: "Vested PCT",
    field: field || "vestedPercent",
    maxWidth: 120
  });
};

export const GetQPAY066xAgeColumn = (): ColDef => {
  return createAgeColumn({
    maxWidth: 100
  });
};

export const GetQPAY066xAgeAtTerminationColumn = (): ColDef => {
  return createAgeColumn({
    headerName: "Age at Term",
    field: "ageAtTermination",
    maxWidth: 150
  });
};

export const GetQPAY066xEnrollmentCodeColumn = (field?: string): ColDef => {
  return createCountColumn({
    headerName: "EC",
    field: field || "enrollmentId",
    maxWidth: 60
  });
};

export const GetQPAY066xPSYearsColumn = (field?: string): ColDef => {
  return createCountColumn({
    headerName: "PS Years",
    field: field || "profitShareYears"
  });
};

export const GetQPAY066xTerminationCodeColumn = (field?: string): ColDef => {
  return createStatusColumn({
    headerName: "Termination Code",
    field: field || "terminationCode"
  });
};

export const GetQPAY066xInactiveDateColumn = (field?: string): ColDef => {
  return createDateColumn({
    headerName: "Inactive Date",
    field: field || "inactiveDate"
  });
};
