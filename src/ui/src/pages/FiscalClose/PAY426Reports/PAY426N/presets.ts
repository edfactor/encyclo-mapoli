import { ReportPreset } from "reduxstore/types";
import { CAPTIONS } from "../../../../constants";

const presets: ReportPreset[] = [
  {
    id: "1",
    name: "PAY426-1",
    description: CAPTIONS.PAY426_ACTIVE_18_20,
    params: {
      reportId: 1
    },
    displayCriteria: {
      ageRange: "18-20",
      hoursRange: "1000+",
      employeeStatus: "Active/Inactive",
      priorProfitShare: "N/A"
    }
  },
  {
    id: "2",
    name: "PAY426-2",
    description: CAPTIONS.PAY426_ACTIVE_21_PLUS,
    params: {
      reportId: 2
    },
    displayCriteria: {
      ageRange: "21+",
      hoursRange: "1000+",
      employeeStatus: "Active/Inactive",
      priorProfitShare: "N/A"
    }
  },
  {
    id: "3",
    name: "PAY426-3",
    description: CAPTIONS.PAY426_ACTIVE_UNDER_18,
    params: {
      reportId: 3
    },
    displayCriteria: {
      ageRange: "Under 18",
      hoursRange: "N/A",
      employeeStatus: "Active/Inactive",
      priorProfitShare: "N/A"
    }
  },
  {
    id: "4",
    name: "PAY426-4",
    description: CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING,
    params: {
      reportId: 4
    },
    displayCriteria: {
      ageRange: "18+",
      hoursRange: "<1000",
      employeeStatus: "Active/Inactive",
      priorProfitShare: "With Prior Amounts"
    }
  },
  {
    id: "5",
    name: "PAY426-5",
    description: CAPTIONS.PAY426_ACTIVE_NO_PRIOR,
    params: {
      reportId: 5
    },
    displayCriteria: {
      ageRange: "18+",
      hoursRange: "<1000",
      employeeStatus: "Active/Inactive",
      priorProfitShare: "No Prior Amounts"
    }
  },
  {
    id: "6",
    name: "PAY426-6",
    description: CAPTIONS.PAY426_TERMINATED_1000_PLUS,
    params: {
      reportId: 6
    },
    displayCriteria: {
      ageRange: "18+",
      hoursRange: "1000+",
      employeeStatus: "Terminated",
      priorProfitShare: "N/A"
    }
  },
  {
    id: "7",
    name: "PAY426-7",
    description: CAPTIONS.PAY426_TERMINATED_NO_PRIOR,
    params: {
      reportId: 7
    },
    displayCriteria: {
      ageRange: "18+",
      hoursRange: "<1000",
      employeeStatus: "Terminated",
      priorProfitShare: "No Prior Amounts"
    }
  },
  {
    id: "8",
    name: "PAY426-8",
    description: CAPTIONS.PAY426_TERMINATED_PRIOR,
    params: {
      reportId: 8
    },
    displayCriteria: {
      ageRange: "18+",
      hoursRange: "<1000",
      employeeStatus: "Terminated",
      priorProfitShare: "With Prior Amounts"
    }
  },
  {
    id: "9",
    name: "PAY426-9",
    description: CAPTIONS.PAY426_SUMMARY,
    params: {
      reportId: 9
    },
    displayCriteria: {
      ageRange: "All",
      hoursRange: "All",
      employeeStatus: "All",
      priorProfitShare: "All"
    }
  },
  {
    id: "10",
    name: "PAY426-10",
    description: CAPTIONS.PAY426_NON_EMPLOYEE,
    params: {
      reportId: 10
    },
    displayCriteria: {
      ageRange: "All",
      hoursRange: "N/A",
      employeeStatus: "Non-Employee",
      priorProfitShare: "N/A"
    }
  }
];

export default presets;
