import { Page } from "smart-ui-library";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { SecurityState } from "reduxstore/slices/securitySlice";

const DevDebug = () => {
    const securityState = useSelector<RootState, SecurityState>((state) => state.security);

    return (
        <Page label="Dev Debug">
            <div style={{ padding: '24px'}}>
                <h3>Access Token</h3>
                <pre style={{ 
                    maxWidth: '50%', 
                    overflowWrap: 'break-word', 
                    wordBreak: 'break-all', 
                    whiteSpace: 'pre-wrap'
                }}>
                    {securityState.token || "No token available"}
                </pre>
                <br />
                <br />
                <h3>User Info</h3>
                <pre>{JSON.stringify({
                    username: securityState.username,
                    impersonating: securityState.impersonating,
                    appUser: securityState.appUser,
                    userRoles: securityState.userRoles,
                    userGroups: securityState.userGroups,
                    userPermissions: securityState.userPermissions
                }, null, 2)}</pre>
            </div>
        </Page>
    );
};

export default DevDebug;