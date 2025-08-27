import { ReportPreset } from "reduxstore/types";
import { CAPTIONS } from "../../../constants";

const presets: ReportPreset[] = [
  {
    id: "1",
    name: "PAY426-1",
    description: CAPTIONS.PAY426_ACTIVE_18_20,
    params: {
      reportId: 1
    }
  },
  {
    id: "2",
    name: "PAY426-2",
    description: CAPTIONS.PAY426_ACTIVE_21_PLUS,
    params: {
      reportId: 2
    }
  },
  {
    id: "3",
    name: "PAY426-3",
    description: CAPTIONS.PAY426_ACTIVE_UNDER_18,
    params: {
      reportId: 3
    }
  },
  {
    id: "4",
    name: "PAY426-4",
    description: CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING,
    params: {
      reportId: 4
    }
  },
  {
    id: "5",
    name: "PAY426-5",
    description: CAPTIONS.PAY426_ACTIVE_NO_PRIOR,
    params: {
      reportId: 5
    }
  },
  {
    id: "6",
    name: "PAY426-6",
    description: CAPTIONS.PAY426_TERMINATED_1000_PLUS,
    params: {
      reportId: 6
    }
  },
  {
    id: "7",
    name: "PAY426-7",
    description: CAPTIONS.PAY426_TERMINATED_NO_PRIOR,
    params: {
      reportId: 7
    }
  },
  {
    id: "8",
    name: "PAY426-8",
    description: CAPTIONS.PAY426_TERMINATED_PRIOR,
    params: {
      reportId: 8
    }
  },
  {
    id: "9",
    name: "PAY426-9",
    description: CAPTIONS.PAY426_SUMMARY,
    params: {
      reportId: 9
    }
  },
  {
    id: "10",
    name: "PAY426-10",
    description: CAPTIONS.PAY426_NON_EMPLOYEE,
    params: {
      reportId: 10
    }
  }
];

export default presets;
