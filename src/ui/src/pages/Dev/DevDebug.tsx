import { Page } from "smart-ui-library";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { SecurityState } from "reduxstore/slices/securitySlice";
import React, { useEffect } from "react";
import { useLazyGetCurrentUserQuery, useLazyGetMetadataQuery } from "reduxstore/api/ItOperationsApi";
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
          <div style={{ marginTop: "24px" }}>
            <DSMCollapsedAccordion title="API Request History">
              <div>
                <h4>Recent API Requests (Last 50 Calls)</h4>
                <p>This section shows the last 50 API calls made during the current session, even if made on other pages.</p>

                {(() => {
                  // Define a React state to store the history
                  const [apiHistory, setApiHistory] = React.useState(() => {
                    // Try to get history from session storage using the utility function
                    return JSON.parse(sessionStorage.getItem('api_request_history') || '[]');
                  });

                  // Check for updates in session storage
                  React.useEffect(() => {
                    // Function to handle storage changes
                    const handleStorageChange = () => {
                      const storedHistory = sessionStorage.getItem('api_request_history');
                      if (storedHistory) {
                        setApiHistory(JSON.parse(storedHistory));
                      }
                    };

                    // Set up an interval to check session storage
                    const interval = setInterval(handleStorageChange, 2000);

                    // Initial load
                    handleStorageChange();

                    // Clean up
                    return () => clearInterval(interval);
                  }, []);

                  // If no history, show message
                  if (!apiHistory || apiHistory.length === 0) {
                    return (
                      <div style={{ padding: "16px", backgroundColor: "#f5f5f5", borderRadius: "4px" }}>
                        <p>No API request history found. Make some API calls and return to this page.</p>
                        <p>Note: API calls are limited to the last 50 requests for debugging purposes.</p>
                      </div>
                    );
                  }

                  // Display the history
                  return (
                    <table style={{ width: "100%", borderCollapse: "collapse" }}>
                      <thead>
                      <tr>
                        <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Time</th>
                        <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Method</th>
                        <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>URL</th>
                        <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Status</th>
                        <th style={{ border: "1px solid #ddd", padding: "8px", textAlign: "left" }}>Duration</th>
                      </tr>
                      </thead>
                      <tbody>
                      {apiHistory.map((request, index) => (
                        <tr key={index}>
                          <td style={{ border: "1px solid #ddd", padding: "8px" }}>
                            {typeof request.time === 'string'
                              ? new Date(request.time).toLocaleTimeString()
                              : new Date(request.time).toLocaleTimeString()}
                          </td>
                          <td style={{ border: "1px solid #ddd", padding: "8px" }}>{request.method}</td>
                          <td style={{ border: "1px solid #ddd", padding: "8px" }}>{request.url}</td>
                          <td style={{
                            border: "1px solid #ddd",
                            padding: "8px",
                            color: request.status >= 400
                              ? "red"
                              : request.status >= 200 && request.status < 300
                                ? "green"
                                : "orange"
                          }}>
                            {request.status}
                          </td>
                          <td style={{ border: "1px solid #ddd", padding: "8px" }}>{request.duration}ms</td>
                        </tr>
                      ))}
                      </tbody>
                    </table>
                  );
                })()}

                {/* Add controls to manage history */}
                <div style={{ marginTop: "16px", display: "flex", gap: "8px" }}>
                  <button
                    onClick={() => {
                      sessionStorage.removeItem('api_request_history');
                      // Force a re-render
                      window.location.reload();
                    }}
                    style={{
                      padding: "8px 16px",
                      backgroundColor: "#dc3545",
                      color: "white",
                      border: "none",
                      borderRadius: "4px"
                    }}
                  >
                    Clear History
                  </button>

                  <button
                    onClick={() => {
                      // Force a refresh of the component
                      window.location.reload();
                    }}
                    style={{
                      padding: "8px 16px",
                      backgroundColor: "#28a745",
                      color: "white",
                      border: "none",
                      borderRadius: "4px"
                    }}
                  >
                    Refresh History
                  </button>
                </div>
              </div>
            </DSMCollapsedAccordion>
          </div>
        </div>        
      </div>
    </Page>
  );
};

export default DevDebug;
