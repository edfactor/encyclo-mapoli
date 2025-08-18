import { Button, Divider, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Page } from "smart-ui-library";

const Unauthorized = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [requiredRoles, setRequiredRoles] = useState<string>("");
  const [attemptedPage, setAttemptedPage] = useState<string>("");

  useEffect(() => {
    const roles = searchParams.get("requiredRoles");
    const page = searchParams.get("page");

    setRequiredRoles(roles || "specific permissions");
    setAttemptedPage(page || "this page");
  }, [searchParams]);

  const handleGoHome = () => {
    navigate("/", { replace: true });
  };

  const handleGoBack = () => {
    navigate(-1);
  };

  return (
    <Page label="Access Denied">
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <div className="mx-auto max-w-2xl p-6 text-center">
            <h2 className="mb-4 text-2xl font-bold text-gray-800">Access Denied</h2>
            <p className="mb-6 text-base leading-relaxed text-gray-600">
              You do not have permission to access {attemptedPage}.
              {requiredRoles && ` This page requires the following role(s): ${requiredRoles}`}
            </p>

            <div className="flex justify-center gap-3">
              <Button
                variant="contained"
                onClick={handleGoHome}
                color="primary"
                className="px-6 py-2">
                Go to Home
              </Button>
              <Button
                variant="outlined"
                onClick={handleGoBack}
                color="secondary"
                className="px-6 py-2">
                Go Back
              </Button>
            </div>
          </div>
        </Grid>
      </Grid>
    </Page>
  );
};

export default Unauthorized;
