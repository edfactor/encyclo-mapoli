import { Page } from "smart-ui-library";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { SecurityState } from "reduxstore/slices/securitySlice";
import { useEffect } from "react";
import { useLazyGetCurrentUserQuery, useLazyGetMetadataQuery } from "reduxstore/api/ItOperations";
import DSMCollapsedAccordion from "../../components/DSMCollapsedAccordion";

const DevDebug = () => {
  const securityState = useSelector<RootState, SecurityState>((state) => state.security);
  const hasToken: boolean = !!useSelector((state: RootState) => securityState.token);

  // Initialize the lazy queries
  const [getCurrentUser, { data: currentUserData, isLoading: currentUserLoading }] = useLazyGetCurrentUserQuery();
  const [getMetadata, { data: metadataData, isLoading: metadataLoading }] = useLazyGetMetadataQuery();

  // Trigger the API calls when the component mounts
  useEffect(() => {
    if (hasToken) {
      getCurrentUser();
      getMetadata();
    }
  }, [getCurrentUser, getMetadata, hasToken]);

  return (
    <Page label="Dev Debug">
      <div style={{ padding: "24px" }}>
        <div style={{ display: "flex", marginTop: "24px", gap: "24px" }}>
          {/* Security State on the left */}
          <div style={{ flex: 1 }}>
            <h3>Security State (JWT)</h3>
            <pre style={{ maxHeight: "500px", overflow: "auto" }}>
              {JSON.stringify(
                {
                  username: securityState.username,
                  impersonating: securityState.impersonating,
                  appUser: securityState.appUser,
                  userRoles: securityState.userRoles,
                  userGroups: securityState.userGroups,
                  userPermissions: securityState.userPermissions
                },
                null,
                2
              )}
            </pre>
          </div>

          {/* Current User Data on the right */}
          <div style={{ flex: 1 }}>
            <h3>Current API User Data</h3>
            {currentUserLoading ? (
              <p>Loading current user data...</p>
            ) : (
              <pre style={{ maxHeight: "500px", overflow: "auto" }}>
                {currentUserData ? JSON.stringify(currentUserData, null, 2) : "No current user data available"}
              </pre>
            )}
          </div>
        </div>

        <DSMCollapsedAccordion title="Access Token">
          <pre
            style={{
              maxWidth: "50%",
              overflowWrap: "break-word",
              wordBreak: "break-all",
              whiteSpace: "pre-wrap"
            }}>
            {securityState.token || "No token available"}
          </pre>
        </DSMCollapsedAccordion>

        {/* Database Metadata in an accordion */}
        <div style={{ marginTop: "24px" }}>
          <DSMCollapsedAccordion title="Database Metadata">
            {metadataLoading ? (
              <p>Loading metadata...</p>
            ) : (
              <div>
                <h4>Table Row Counts</h4>
                {metadataData && metadataData.length > 0 ? (
                  <table style={{ width: "100%", borderCollapse: "collapse" }}>
                    <thead>
                      <tr>
                        <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Table Name</th>
                        <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Row Count</th>
                      </tr>
                    </thead>
                    <tbody>
                      {metadataData.map((item) => (
                        <tr key={item.tableName}>
                          <td style={{ border: "1px solid #ddd", padding: "8px" }}>{item.tableName}</td>
                          <td style={{ border: "1px solid #ddd", padding: "8px" }}>{item.rowCount}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                ) : (
                  <p>No metadata available</p>
                )}
              </div>
            )}
          </DSMCollapsedAccordion>
        </div>
        <div style={{ marginTop: "24px" }}>
          <DSMCollapsedAccordion title="Environment Variables">
            <pre style={{ maxHeight: "500px", overflow: "auto" }}>
              {JSON.stringify(
                {
                  NODE_ENV: process.env.NODE_ENV,
                  API_URL: process.env.VITE_API_URL,
                  VERSION: process.env.VITE_APP_VERSION,
                  BUILD_TIME: process.env.VITE_BUILD_TIME
                },
                null,
                2
              )}
            </pre>
          </DSMCollapsedAccordion>
        </div>
        <div style={{ marginTop: "24px" }}>
          <DSMCollapsedAccordion title="Redux Store State">
            <pre style={{ maxHeight: "500px", overflow: "auto" }}>
              {JSON.stringify(
                useSelector((state: RootState) => state),
                null,
                2
              )}
            </pre>
          </DSMCollapsedAccordion>
        </div>
        <div style={{ marginTop: "24px" }}>
          <DSMCollapsedAccordion title="Browser Information">
            <table style={{ width: "100%", borderCollapse: "collapse" }}>
              <tbody>
                <tr>
                  <td style={{ border: "1px solid #ddd", padding: "8px", fontWeight: "bold" }}>User Agent</td>
                  <td style={{ border: "1px solid #ddd", padding: "8px" }}>{navigator.userAgent}</td>
                </tr>
                <tr>
                  <td style={{ border: "1px solid #ddd", padding: "8px", fontWeight: "bold" }}>Screen Resolution</td>
                  <td
                    style={{
                      border: "1px solid #ddd",
                      padding: "8px"
                    }}>{`${window.screen.width}x${window.screen.height}`}</td>
                </tr>
                <tr>
                  <td style={{ border: "1px solid #ddd", padding: "8px", fontWeight: "bold" }}>Language</td>
                  <td style={{ border: "1px solid #ddd", padding: "8px" }}>{navigator.language}</td>
                </tr>
                <tr>
                  <td style={{ border: "1px solid #ddd", padding: "8px", fontWeight: "bold" }}>Timezone</td>
                  <td
                    style={{
                      border: "1px solid #ddd",
                      padding: "8px"
                    }}>
                    {Intl.DateTimeFormat().resolvedOptions().timeZone}
                  </td>
                </tr>
              </tbody>
            </table>
          </DSMCollapsedAccordion>
        </div>

        <div style={{ marginTop: "24px" }}>
          <DSMCollapsedAccordion title="Recent API Requests">
            <div>
              <h4>API Endpoints Status</h4>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr>
                    <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>API</th>
                    <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Endpoint</th>
                    <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Status</th>
                    <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Last Called</th>
                    <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Response Time (ms)</th>
                  </tr>
                </thead>
                <tbody>
                  {(() => {
                    const state = useSelector((state: RootState) => state);
                    const rows = [];

                    // List of potential API reducer paths to check
                    const apiPaths = ["militaryApi", "api", "ItOperationsApi", "InquiryApi", "YearsEndApi"];

                    // Loop through each API path
                    for (const apiPath of apiPaths) {
                      if (!state[apiPath]) continue;

                      const apiState = state[apiPath];
                      const queries = apiState.queries || {};

                      // Process each query in this API
                      Object.entries(queries).forEach(([queryKey, queryData]: [string, any], index) => {
                        // Extract endpoint name from the query key
                        const endpointName = queryKey.split("(")[0];

                        // Get request status
                        let status = "unknown";
                        if (queryData.status === "fulfilled") status = "success";
                        if (queryData.status === "rejected") status = "error";
                        if (queryData.status === "pending") status = "loading";

                        // Format timestamp
                        const timestamp = queryData.startedTimeStamp
                          ? new Date(queryData.startedTimeStamp).toLocaleString()
                          : "N/A";

                        // Calculate response time if available
                        const responseTime =
                          queryData.fulfilledTimeStamp && queryData.startedTimeStamp
                            ? queryData.fulfilledTimeStamp - queryData.startedTimeStamp
                            : "N/A";

                        rows.push(
                          <tr key={`${apiPath}-${index}`}>
                            <td style={{ border: "1px solid #ddd", padding: "8px" }}>{apiPath}</td>
                            <td style={{ border: "1px solid #ddd", padding: "8px" }}>{endpointName}</td>
                            <td
                              style={{
                                border: "1px solid #ddd",
                                padding: "8px",
                                color:
                                  status === "error"
                                    ? "red"
                                    : status === "success"
                                      ? "green"
                                      : status === "loading"
                                        ? "orange"
                                        : "inherit"
                              }}>
                              {status}
                            </td>
                            <td style={{ border: "1px solid #ddd", padding: "8px" }}>{timestamp}</td>
                            <td style={{ border: "1px solid #ddd", padding: "8px" }}>{responseTime}</td>
                          </tr>
                        );
                      });
                    }

                    // If no rows, display a message
                    if (rows.length === 0) {
                      return (
                        <tr>
                          <td
                            colSpan={5}
                            style={{ border: "1px solid #ddd", padding: "8px", textAlign: "center" }}>
                            No API requests found in Redux store. Try making an API call first.
                          </td>
                        </tr>
                      );
                    }

                    return rows;
                  })()}
                </tbody>
              </table>
            </div>
          </DSMCollapsedAccordion>
        </div>

        <div style={{ marginTop: "24px" }}>
          <DSMCollapsedAccordion title="RTK Query Debug">
            <div>
              <h4>Available API Reducer Paths</h4>
              <pre style={{ maxHeight: "200px", overflow: "auto" }}>
                {JSON.stringify(
                  Object.keys(useSelector((state: RootState) => state)).filter(
                    (key) => key.includes("Api") || key === "api"
                  ),
                  null,
                  2
                )}
              </pre>

              <h4>Sample Query State</h4>
              <pre style={{ maxHeight: "300px", overflow: "auto" }}>
                {(() => {
                  const state = useSelector((state: RootState) => state);
                  // Find the first API reducer path
                  const apiPath = Object.keys(state).find((key) => key.includes("Api") || key === "api");
                  if (!apiPath || !state[apiPath]) return "No API state found";

                  // Get a sample query if available
                  const queries = state[apiPath].queries || {};
                  const sampleKey = Object.keys(queries)[0];
                  if (!sampleKey) return "No queries found in " + apiPath;

                  return JSON.stringify(
                    {
                      reducerPath: apiPath,
                      sampleQuery: sampleKey,
                      queryData: queries[sampleKey]
                    },
                    null,
                    2
                  );
                })()}
              </pre>
            </div>
          </DSMCollapsedAccordion>
        </div>
      </div>
    </Page>
  );
};

export default DevDebug;
