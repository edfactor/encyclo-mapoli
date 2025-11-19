import { QPAY066xAdHocReportPreset } from "@/types";

const reports: QPAY066xAdHocReportPreset[] = [
  {
    id: "QPAY066A",
    name: "QPAY066A",
    description: "Less than 20% Vested",
    params: {
      reportId: 1
    },
    requiresDateRange: true
  },
  {
    id: "QPAY066A-1",
    name: "QPAY066A-1",
    description: "Less than 20% Vested (Variation 1)",
    params: {
      reportId: 2
    },
    requiresDateRange: true
  },
  {
    id: "QPAY066AF",
    name: "QPAY066AF",
    description: "QPAY066AF Name",
    params: {
      reportId: 3
    },
    requiresDateRange: false
  },
  {
    id: "QPAY066-AGE70",
    name: "QPAY066-AGE70",
    description: "Aged 70 and Over",
    params: {
      reportId: 4
    },
    requiresDateRange: false
  },
  {
    // ok
    id: "QPAY066I",
    name: "QPAY066I",
    description: "Inactive Employees",
    params: {
      reportId: 5
    },
    requiresDateRange: false
  },
  {
    id: "QPAY066B",
    name: "QPAY066B",
    description: "Beneficiaries Report",
    params: {
      reportId: 6
    },
    requiresDateRange: false
  },
  {
    id: "QPAY066D",
    name: "QPAY066D",
    description: "Disabled Employees",
    params: {
      reportId: 7
    },
    requiresDateRange: false
  },
  {
    id: "QPAY066M",
    name: "QPAY066M",
    description: "QPAY066M",
    params: {
      reportId: 8
    },
    requiresDateRange: false
  },
  {
    id: "QPAY066W",
    name: "QPAY066W",
    description: "Employees with Wages",
    params: {
      reportId: 9
    },
    requiresDateRange: false
  }
];

export default reports;
