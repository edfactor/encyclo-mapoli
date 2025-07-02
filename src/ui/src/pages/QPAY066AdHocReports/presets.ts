import { ReportPreset } from "reduxstore/types";

const presets: ReportPreset[] = [
  {
    id: 'QPAY066A',
    name: 'QPAY066A',
    description: 'Less than 20% Vested',
    params: {
      reportId: 1,
      isYearEnd: true,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066A-1',
    name: 'QPAY066A-1',
    description: 'Less than 20% Vested (Variation 1)',
    params: {
      reportId: 2,
      isYearEnd: true,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066AF',
    name: 'QPAY066AF',
    description: 'QPAY066AF Name',
    params: {
      reportId: 3,
      isYearEnd: true,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066-AGE70',
    name: 'QPAY066-AGE70',
    description: 'Aged 70 and Over',
    params: {
      reportId: 4,
      isYearEnd: true,
      minimumAgeInclusive: 70,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066I',
    name: 'QPAY066I',
    description: 'Inactive Employees',
    params: {
      reportId: 5,
      isYearEnd: true,
      includeActiveEmployees: false,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066B',
    name: 'QPAY066B',
    description: 'Beneficiaries Report',
    params: {
      reportId: 6,
      isYearEnd: true,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: true,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066D',
    name: 'QPAY066D',
    description: 'Disabled Employees',
    params: {
      reportId: 7,
      isYearEnd: true,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066M',
    name: 'QPAY066M',
    description: 'Military Leave Employees',
    params: {
      reportId: 8,
      isYearEnd: true,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'QPAY066W',
    name: 'QPAY066W',
    description: 'Employees with Wages',
    params: {
      reportId: 9,
      isYearEnd: true,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  }
];

export default presets; 