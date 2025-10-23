import { Middleware } from "@reduxjs/toolkit";
import { url as baseUrl } from "../reduxstore/api/api";

// For storing in-flight request information
type RequestTiming = {
  startTime: number;
  url: string | "pending";
  method: string;
  duration?: number;
  status?: number;
  completed?: boolean;
};
const requestTimings = new Map<string, RequestTiming>();

// Intercept actual fetch requests to capture timing and URLs
const originalFetch = window.fetch.bind(window);
window.fetch = function captureUrlFetch(input: RequestInfo | URL, init?: RequestInit) {
  const startTime = performance.now();
  const requestUrl = typeof input === "string" || input instanceof URL ? String(input) : (input as Request).url;

  // Save the URL for later correlation with RTK Query actions
  const requestData: RequestTiming = {
    url: requestUrl,
    method: init?.method || (input instanceof Request ? input.method : "GET"),
    startTime,
    duration: 0
  };

  // Actual fetch call
  return originalFetch
    .apply(this, [input as RequestInfo | URL, init as RequestInit | undefined])
    .then((response) => {
      const endTime = performance.now();
      requestData.duration = Math.round(endTime - startTime);
      requestData.status = response.status;

      // Check if this URL corresponds to a cached request ID
      requestTimings.forEach((data, requestId) => {
        if (data.url === "pending" && !data.completed) {
          // Update the request with actual data
          requestTimings.set(requestId, {
            ...data,
            url: requestUrl,
            method: requestData.method,
            duration: requestData.duration,
            status: requestData.status,
            completed: true
          });

          // Update the session storage
          updateSessionStorage(requestId, {
            url: requestUrl,
            method: requestData.method,
            status: requestData.status ?? 0,
            duration: requestData.duration ?? 0
          });

          // Only update one request to avoid duplicates
        }
      });

      return response;
    })
    .catch((error) => {
      const endTime = performance.now();
      requestData.duration = Math.round(endTime - startTime);
      requestData.status = error.status || 500;

      // Check if this URL corresponds to a cached request ID
      requestTimings.forEach((data, requestId) => {
        if (data.url === "pending" && !data.completed) {
          // Update the request with actual data
          requestTimings.set(requestId, {
            ...data,
            url: requestUrl,
            method: requestData.method,
            duration: requestData.duration,
            status: requestData.status,
            completed: true
          });

          // Update the session storage
          updateSessionStorage(requestId, {
            url: requestUrl,
            method: requestData.method,
            status: requestData.status ?? 0,
            duration: requestData.duration ?? 0
          });

          // Only update one request to avoid duplicates
        }
      });

      throw error;
    });
};

// Helper function to update session storage with actual request data
interface RequestHistoryEntry {
  requestId: string;
  time: string;
  method: string;
  url: string;
  status: string | number;
  duration: number;
}

function updateSessionStorage(
  requestId: string,
  data: { url: string; method: string; status: number; duration: number }
) {
  const history = JSON.parse(sessionStorage.getItem("api_request_history") || "[]") as RequestHistoryEntry[];
  const updatedHistory = history.map((entry) => {
    if (entry.requestId === requestId) {
      return {
        ...entry,
        url: data.url,
        method: data.method,
        status: data.status,
        duration: data.duration
      };
    }
    return entry;
  });

  // Limit to 50 entries instead of no limit
  sessionStorage.setItem("api_request_history", JSON.stringify(updatedHistory.slice(0, 50)));
}

interface RtkQueryAction {
  type: string;
  meta?: {
    requestId?: string;
  };
  error?: {
    status?: number;
  };
}

/**
 * This middleware logs RTK Query API requests to session storage
 * by tracking pending/fulfilled action pairs and correlating them with fetch requests
 */
export const apiLoggerMiddleware: Middleware = () => (next) => (action: unknown) => {
  const typedAction = action as RtkQueryAction;
  // Handle RTK Query actions
  if (typedAction.type && typeof typedAction.type === "string") {
    // First check if it's an RTK Query action by looking for the pattern api/endpoint/status
    const parts = typedAction.type.split("/");

    // Must have exactly 3 parts and end with a standard RTK Query status
    if (parts.length === 3 && ["pending", "fulfilled", "rejected"].includes(parts[2])) {
      const apiName = parts[0].replace(/Api$/, "");
      const endpointName = parts[1];
      const status = parts[2];

      // Only handle pending actions (start of request)
      if (status === "pending" && typedAction.meta?.requestId) {
        const requestId = typedAction.meta.requestId;

        // Store timing information for this request
        requestTimings.set(requestId, {
          startTime: performance.now(),
          url: "pending",
          method: "GET",
          completed: false
        });

        // Initial entry for the request
        const requestInfo: RequestHistoryEntry = {
          time: new Date().toISOString(), // Changed from Date.now() for better readability
          method: "GET", // Will be updated when fetch completes
          url: `${baseUrl}/api/pending`, // Placeholder until fetch completes
          status: "pending",
          duration: 0,
          requestId
        };

        // Add to history
        const history = JSON.parse(sessionStorage.getItem("api_request_history") || "[]") as RequestHistoryEntry[];
        history.unshift(requestInfo);
        // Limit to exactly 50 entries
        sessionStorage.setItem("api_request_history", JSON.stringify(history.slice(0, 50)));
      }

      // Handle completion actions (end of request)
      else if ((status === "fulfilled" || status === "rejected") && typedAction.meta?.requestId) {
        const requestId = typedAction.meta.requestId;
        const requestData = requestTimings.get(requestId);

        if (requestData && !requestData.completed) {
          // If the fetch hasn't updated with actual data yet, use the completion time
          const endTime = performance.now();
          const duration = Math.round(endTime - requestData.startTime);
          const responseStatus = status === "fulfilled" ? 200 : typedAction.error?.status || 500;

          // Update the request timing record
          requestTimings.set(requestId, {
            ...requestData,
            duration,
            status: responseStatus,
            completed: true
          });

          // Update the session storage history
          updateSessionStorage(requestId, {
            url:
              requestData.url !== "pending"
                ? requestData.url
                : `${baseUrl}/api/${apiName.toLowerCase()}/${endpointName.toLowerCase()}`,
            method: requestData.method,
            status: responseStatus,
            duration
          });
        }

        // Clean up
        setTimeout(() => {
          requestTimings.delete(requestId);
        }, 1000);
      }
    }
  }

  // Let the action proceed
  return next(action);
};

// Helper function to get the API call history
export function getApiCallHistory() {
  return JSON.parse(sessionStorage.getItem("api_request_history") || "[]");
}
