import { ReportPreset } from "reduxstore/types";
import { CAPTIONS } from "../../../constants";

const presets: ReportPreset[] = [
  {
    id: 'PAY426-1',
    name: 'PAY426-1',
    description: CAPTIONS.PAY426_ACTIVE_18_20,
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
    description: CAPTIONS.PAY426_ACTIVE_21_PLUS,
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
    description: CAPTIONS.PAY426_ACTIVE_UNDER_18,
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
    description: CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING,
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 1000,
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
    description: CAPTIONS.PAY426_ACTIVE_NO_PRIOR,
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 1000,
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
    description: CAPTIONS.PAY426_TERMINATED_1000_PLUS,
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
    description: CAPTIONS.PAY426_TERMINATED_NO_PRIOR,
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 1000,
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
    description: CAPTIONS.PAY426_TERMINATED_PRIOR,
    params: {
      isYearEnd: true,
      minimumAgeInclusive: 18,
      maximumHoursInclusive: 1000,
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
    description: CAPTIONS.PAY426_SUMMARY,
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
    description: CAPTIONS.PAY426_NON_EMPLOYEE,
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