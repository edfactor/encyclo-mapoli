import { ReportPreset } from "reduxstore/types";
import { CAPTIONS } from "../../../constants";

const presets: ReportPreset[] = [
  {
    id: '1',
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
    id: '2',
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
    id: '3',
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
    id: '4',
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
    id: '5',
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
    id: '6',
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
    id: '7',
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
    id: '8',
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
    id: '9',
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
  }
];

export default presets; 