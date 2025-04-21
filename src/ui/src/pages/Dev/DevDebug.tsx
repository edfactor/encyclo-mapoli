import { DSMAccordion, Page } from "smart-ui-library";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { SecurityState } from "reduxstore/slices/securitySlice";
import { useEffect } from "react";
import { useLazyGetCurrentUserQuery, useLazyGetMetadataQuery } from "reduxstore/api/ItOperations";

const DevDebug = () => {
  const securityState = useSelector<RootState, SecurityState>((state) => state.security);
  const hasToken: boolean = !!useSelector((state: RootState) => securityState.token);
  
  // Initialize the lazy queries
  const [getCurrentUser, { data: currentUserData, isLoading: currentUserLoading }] = useLazyGetCurrentUserQuery();
  const [getMetadata, { data: metadataData, isLoading: metadataLoading }] = useLazyGetMetadataQuery();

  // Trigger the API calls when the component mounts
  useEffect(() => {
    if ( hasToken) {
      getCurrentUser();
      getMetadata();
    }
  }, [getCurrentUser, getMetadata, hasToken]);

  return (
    <Page label="Dev Debug">
      <div style={{ padding: "24px" }}>
        <DSMAccordion title="Access Token">
          <pre
            style={{
              maxWidth: "50%",
              overflowWrap: "break-word",
              wordBreak: "break-all",
              whiteSpace: "pre-wrap"
            }}>
            {securityState.token || "No token available"}
          </pre>
        </DSMAccordion>

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

        {/* Metadata in an accordion */}
        <div style={{ marginTop: "24px" }}>
          <DSMAccordion title="Database Metadata">
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
                    {metadataData.map((item, index) => (
                      <tr key={index}>
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
          </DSMAccordion>
        </div>
      </div>
    </Page>
  );
};

export default DevDebug;