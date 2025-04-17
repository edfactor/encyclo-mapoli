import { LoginCallback } from "@okta/okta-react";

const OktaLoginCallback = () => {
  const params = new URLSearchParams(document.location.search);
  const errorCode = params.get("error");
  const errorDescription = params.get("error_description");

  if (errorCode === "access_denied") {
    return (
      <div style={{ padding: "20px", textAlign: "center" }}>
        <h2>Access Denied</h2>
        <p>You are not assigned to this application in Okta.</p>
        <p>Please contact your administrator to request access to this environment.</p>
      </div>
    );
  }

  if (errorCode) {
    return (
      <div style={{ padding: "20px", textAlign: "center" }}>
        <h2>Authentication Error</h2>
        <p>{errorDescription || errorCode}</p>
      </div>
    );
  }

  return <LoginCallback />;
};

export default OktaLoginCallback;
