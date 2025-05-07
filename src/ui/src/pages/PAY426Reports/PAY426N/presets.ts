import { ReportPreset } from "reduxstore/types";

const presets: ReportPreset[] = [
  {
    id: 'PAY426-1',
    name: 'PAY426-1',
    description: 'All Active and Inactive Employees >= Age 20 with >= 1000 PS Hours',
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumAgeInclusive: 20,
      minimumHoursInclusive: 1000,
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
    id: 'PAY426-2',
    name: 'PAY426-2',
    description: 'All Active and Inactive Employees >= Age 21 with >= 1000 PS Hours',
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 21,
      minimumHoursInclusive: 1000,
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
    id: 'PAY426-3',
    name: 'PAY426-3',
    description: 'All Active and Inactive Employees < Age 18',
    params: {
      isYearEnd: true,
      maximumAgeInclusive: 17,
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
    id: 'PAY426-4',
    name: 'PAY426-4',
    description: 'All Active and Inactive Employees >= Age 18 with < 1000 PS Hours and Prior PS Amount',
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 999,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: false,
    }
  },
  {
    id: 'PAY426-5',
    name: 'PAY426-5',
    description: 'All Active and Inactive Employees >= Age 18 with < 1000 PS Hours and No Prior PS Amount',
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 999,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: false,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'PAY426-6',
    name: 'PAY426-6',
    description: 'All Terminated Employees >= Age 18 with >= 1000 PS Hours',
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      minimumHoursInclusive: 1000,
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
    id: 'PAY426-7',
    name: 'PAY426-7',
    description: 'All Terminated Employees >= Age 18 with < 1000 PS Hours and No Prior PS Amount',
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 999,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: false,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'PAY426-8',
    name: 'PAY426-8',
    description: 'All Terminated Employees >= Age 18 with < 1000 PS Hours and Prior PS Amount',
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 999,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: false,
    }
  },
  {
    id: 'PAY426-9',
    name: 'PAY426-9',
    description: 'Year End Profit Sharing Summary Report',
    params: {
      isYearEnd: true,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: true,
      includeTerminatedEmployees: true,
      includeBeneficiaries: true,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
    }
  },
  {
    id: 'PAY426-10',
    name: 'PAY426-10',
    description: 'All Non-Employee Beneficiaries',
    params: {
      isYearEnd: true,
      includeActiveEmployees: false,
      includeInactiveEmployees: false,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: false,
      includeBeneficiaries: true,
      includeEmployeesWithPriorProfitSharingAmounts: false,
      includeEmployeesWithNoPriorProfitSharingAmounts: false,
    }
  }
];

export default presets; 