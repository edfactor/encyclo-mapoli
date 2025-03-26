/**
 * Profit Sharing Plumbing Prototype
 * ===============================
 * 
 * This prototype demonstrates a structured approach to managing the annual profit sharing
 * process through a series of "flows" and "jobs". While currently using localStorage for
 * state management, this architecture is designed to transition to an API-based solution.
 * 
 * Current Implementation:
 * ----------------------
 * - Uses localStorage to simulate persistent state
 * - Hardcoded flow and job definitions
 * - Static mock data for reports
 * - Basic completion tracking and history
 * 
 * Path to Production:
 * ------------------
 * 1. Replace localStorage with API calls
 *    - Fetch process state on app load
 *    - Update state through API endpoints
 *    - Implement explicit sign-off functionality for job completion (access-based)
 * 
 * 2. Hook up frontend components in sequence
 *    - Start with Clean Up Reports flow
 *    - Implement each report/job one at a time
 *    - Maintain process state management
 *    - Add validation for job completion requirements
 */

interface JobCompletion {
  completedBy: string; // Employee ID / Badge Number
  completedAt: Date;
}

interface JobDefinition {
  id: string;
  name: string;
  description: string;
  type: "JOB" | "REPORT";
  isRerunnable: boolean;
  prerequisites: string[]; // Array of job IDs that must be completed first
  order: number; // Order within the flow
}

interface JobState {
  jobId: string;
  order: number;
  status: "NOT_STARTED" | "IN_PROGRESS" | "COMPLETED";
  validationStatus?: "PENDING" | "VALID" | "INVALID";
  latestRun?: JobCompletion;
  data?: any; // The actual job/report data
}

interface FlowState {
  id: string;
  name: string;
  description: string;
  order: number;
  status: "NOT_STARTED" | "IN_PROGRESS" | "COMPLETED";
  jobs: {
    [jobId: string]: JobState;
  };
}

interface ProcessState {
  year: number;
  flows: {
    [flowId: string]: FlowState;
  };
}

const exampleState: ProcessState = {
  year: 2024,
  flows: {
    "december-cleanup": {
      id: "december-cleanup",
      name: "December Cleanup",
      description: "End of year cleanup processes and validations",
      order: 1,
      status: "IN_PROGRESS",
      jobs: {
        "negative-etva-check": {
          jobId: "negative-etva-check",
          order: 1,
          status: "COMPLETED",
          validationStatus: "VALID",
          latestRun: {
            completedBy: "user123",
            completedAt: new Date("2024-12-18T10:00:00")
          },
          data: {
            // The actual report data
          }
        },
        "duplicate-ssn-check": {
          jobId: "duplicate-ssn-check",
          order: 2,
          status: "NOT_STARTED",
          validationStatus: "PENDING"
        }
      }
    }
  }
};
