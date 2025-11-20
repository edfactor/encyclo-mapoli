import { QPAY066xAdHocReportPreset } from "@/types";

const reports: QPAY066xAdHocReportPreset[] = [
  {
    id: "QPAY066C",
    name: "QPAY066C",
    description:
      "QPAY066C: Breakdown terminated managers and associates for all stores who have a balance but not vested",
    params: {
      reportId: 1
    },
    requiresDateRange: true,
    apiEndpoint: "/api/yearend/breakdown-by-store/terminated/withcurrentbalance/notvested"
  },
  {
    id: "QPAY066-Inactive",
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
    description: "QPAY066-I: Inactive with Vested Balance",
    params: {
      reportId: 3
    },
    requiresDateRange: true,
    apiEndpoint: "/api/yearend/breakdown-by-store/inactive/withvestedbalance"
  },
  {
    id: "QPAY066B",
    name: "QPAY066B",
    description: "QPAY066B: Terminated with Beneficiary Allocation",
    params: {
      reportId: 4
    },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store/terminated/withbeneficiaryallocation"
  },
  {
    id: "QPAY066W",
    name: "QPAY066W",
    description: "QPAY066W: Retired with Balance Activity",
    params: {
      reportId: 5
    },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store/retired/withbalanceactivity"
  },
  {
    id: "QPAY066TA",
    name: "QPAY066TA",
    description: "QPAY066TA: Breakdown managers and associates for all stores",
    params: {
      reportId: 6
    },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store"
  }
  /*

  These reports have not yet been implemented on the backend.

  {
    id: "QPAY066D",
    name: "QPAY066D",
    description: "QPAY066D - Terminated with Balance Activity",
    params: {
      reportId: 4
    },
    requiresDateRange: true,
    apiEndpoint: "/api/yearend/breakdown-by-store/terminated/withbalanceactivity"
  },
  
  
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
    id: "QPAY066M",
    name: "QPAY066M",
    description: "QPAY066M: Managers",
    params: {
      reportId: 8
    },
    requiresDateRange: false
  },
  */
];

export default reports;
