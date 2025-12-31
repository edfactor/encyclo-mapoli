import { QPAY066xAdHocReportPreset } from "@/types";

const reports: QPAY066xAdHocReportPreset[] = [
  {
    id: "QPAY066C",
    name: "QPAY066C",
    description: "Terminated managers and associates for all stores with a balance but not vested",
    params: {
      reportId: 1
    },
    requiresDateRange: true,
    apiEndpoint: "/api/yearend/breakdown-by-store/terminated/withcurrentbalance/notvested"
  },
  {
    id: "QPAY066-INACTIVE",
    name: "QPAY066-Inactive",
    description: "Inactive Employees",
    params: {
      reportId: 2
    },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store/inactive"
  },
  {
    id: "QPAY066-I",
    name: "QPAY066-I",
    description: "Inactive with Vested Balance",
    params: {
      reportId: 3
    },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store/inactive/withvestedbalance"
  },
  {
    id: "QPAY066B",
    name: "QPAY066B",
    description: "Terminated with Beneficiary Allocation",
    params: {
      reportId: 4
    },
    requiresDateRange: true,
    apiEndpoint: "/api/yearend/breakdown-by-store/terminated/withbeneficiaryallocation"
  },
  {
    id: "QPAY066W",
    name: "QPAY066W",
    description: "Retired with Balance Activity",
    params: {
      reportId: 5
    },
    requiresDateRange: true,
    apiEndpoint: "yearend/breakdown-by-store/retired/withbalanceactivity"
  },
  {
    id: "QPAY066TA",
    name: "QPAY066TA",
    description: "Managers and associates for all stores",
    params: {
      reportId: 6
    },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/stores/breakdown"
  },
  {
    id: "QPAY066M",
    name: "QPAY066M",
    description: "Monthly employees with distribution, forfeit, or contribution",
    params: {
      reportId: 7
    },
    requiresDateRange: true,
    apiEndpoint: "/api/yearend/breakdown-by-store/monthly"
  }
];

export default reports;
