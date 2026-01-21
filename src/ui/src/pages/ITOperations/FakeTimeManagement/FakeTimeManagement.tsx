import AccessTimeIcon from "@mui/icons-material/AccessTime";
import RefreshIcon from "@mui/icons-material/Refresh";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import {
  Alert,
  AlertTitle,
  Box,
  Button,
  Card,
  CardContent,
  Checkbox,
  CircularProgress,
  Divider,
  FormControlLabel,
  Grid,
  TextField,
  Typography
} from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { Page } from "smart-ui-library";
import { PageErrorBoundary } from "../../../components/PageErrorBoundary";
import { CAPTIONS } from "../../../constants";
import {
  useGetFakeTimeStatusQuery,
  useSetFakeTimeMutation,
  useValidateFakeTimeMutation
} from "../../../reduxstore/api/ItOperationsApi";
import { FakeTimeStatusResponse, SetFakeTimeRequest } from "../../../reduxstore/types";

interface FakeTimeFormValues {
  enabled: boolean;
  fixedDateTime: string;
  timeZone: string;
  advanceTime: boolean;
}

const defaultTimeZone = "Eastern Standard Time";

const FakeTimeManagement = () => {
  const { data: status, isLoading, isFetching, refetch, error } = useGetFakeTimeStatusQuery();
  const [setFakeTime, { isLoading: isApplying }] = useSetFakeTimeMutation();
  const [validateFakeTime, { isLoading: isValidating }] = useValidateFakeTimeMutation();
  const [validationResult, setValidationResult] = useState<FakeTimeStatusResponse | null>(null);
  const [validationError, setValidationError] = useState<string | null>(null);
  const [applyResult, setApplyResult] = useState<FakeTimeStatusResponse | null>(null);
  const [applyError, setApplyError] = useState<string | null>(null);

  const {
    control,
    handleSubmit,
    reset,
    watch,
    formState: { isDirty }
  } = useForm<FakeTimeFormValues>({
    defaultValues: {
      enabled: false,
      fixedDateTime: "",
      timeZone: defaultTimeZone,
      advanceTime: false
    }
  });

  const enabled = watch("enabled");

  // Reset form values when status loads
  useEffect(() => {
    if (status) {
      reset({
        enabled: status.isActive,
        fixedDateTime: status.configuredDateTime ?? "",
        timeZone: status.timeZone ?? defaultTimeZone,
        advanceTime: status.advanceTime
      });
    }
  }, [status, reset]);

  const onValidate = async (formData: FakeTimeFormValues) => {
    setValidationError(null);
    setValidationResult(null);
    setApplyError(null);
    setApplyResult(null);

    const request: SetFakeTimeRequest = {
      enabled: formData.enabled,
      fixedDateTime: formData.enabled ? formData.fixedDateTime : null,
      timeZone: formData.timeZone || defaultTimeZone,
      advanceTime: formData.advanceTime
    };

    try {
      const result = await validateFakeTime(request).unwrap();
      setValidationResult(result);
    } catch (err) {
      setValidationError(err instanceof Error ? err.message : "Validation failed");
    }
  };

  const onApply = async (formData: FakeTimeFormValues) => {
    setValidationError(null);
    setValidationResult(null);
    setApplyError(null);
    setApplyResult(null);

    const request: SetFakeTimeRequest = {
      enabled: formData.enabled,
      fixedDateTime: formData.enabled ? formData.fixedDateTime : null,
      timeZone: formData.timeZone || defaultTimeZone,
      advanceTime: formData.advanceTime
    };

    try {
      const result = await setFakeTime(request).unwrap();
      setApplyResult(result);
      // Refetch status to update the display
      refetch();
    } catch (err) {
      setApplyError(err instanceof Error ? err.message : "Failed to apply fake time settings");
    }
  };

  const handleRefresh = () => {
    setValidationResult(null);
    setValidationError(null);
    setApplyResult(null);
    setApplyError(null);
    refetch();
  };

  const formatDateTime = (isoString: string | null | undefined): string => {
    if (!isoString) return "N/A";
    try {
      const date = new Date(isoString);
      return date.toLocaleString("en-US", {
        weekday: "short",
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        timeZoneName: "short"
      });
    } catch {
      return isoString;
    }
  };

  if (error) {
    return (
      <PageErrorBoundary pageName="Fake Time Management">
        <Page label={CAPTIONS.FAKE_TIME_MANAGEMENT}>
          <Alert severity="error">
            <AlertTitle>Error</AlertTitle>
            Failed to load fake time status. Please try again.
          </Alert>
        </Page>
      </PageErrorBoundary>
    );
  }

  return (
    <PageErrorBoundary pageName="Fake Time Management">
      <Page label={CAPTIONS.FAKE_TIME_MANAGEMENT}>
        <Grid
          container
          spacing={3}>
          <Grid size={{ xs: 12 }}>
            <Divider />
          </Grid>

          {/* Current Status Card */}
          <Grid size={{ xs: 12, md: 6 }}>
            <Card>
              <CardContent>
                <Box
                  display="flex"
                  alignItems="center"
                  justifyContent="space-between"
                  mb={2}>
                  <Typography
                    variant="h6"
                    component="h2">
                    <AccessTimeIcon sx={{ mr: 1, verticalAlign: "middle" }} />
                    Current Time Status
                  </Typography>
                  <Button
                    variant="outlined"
                    size="small"
                    startIcon={<RefreshIcon />}
                    onClick={handleRefresh}
                    disabled={isFetching}>
                    Refresh
                  </Button>
                </Box>

                {isLoading || isFetching ? (
                  <Box
                    display="flex"
                    justifyContent="center"
                    p={3}>
                    <CircularProgress />
                  </Box>
                ) : status ? (
                  <Box>
                    <Alert
                      severity={status.isActive ? "warning" : "info"}
                      sx={{ mb: 2 }}
                      icon={status.isActive ? <WarningAmberIcon /> : <AccessTimeIcon />}>
                      <AlertTitle>{status.isActive ? "Fake Time Active" : "Real Time Active"}</AlertTitle>
                      {status.message}
                    </Alert>

                    <Grid
                      container
                      spacing={2}>
                      <Grid size={{ xs: 6 }}>
                        <Typography
                          variant="body2"
                          color="textSecondary">
                          Environment
                        </Typography>
                        <Typography variant="body1">{status.environment}</Typography>
                      </Grid>
                      <Grid size={{ xs: 6 }}>
                        <Typography
                          variant="body2"
                          color="textSecondary">
                          Fake Time Allowed
                        </Typography>
                        <Typography variant="body1">{status.isAllowed ? "Yes" : "No"}</Typography>
                      </Grid>
                      <Grid size={{ xs: 6 }}>
                        <Typography
                          variant="body2"
                          color="textSecondary">
                          Runtime Switching
                        </Typography>
                        <Typography variant="body1">
                          {status.isRuntimeSwitchingEnabled ? "Enabled" : "Disabled (restart required)"}
                        </Typography>
                      </Grid>
                      <Grid size={{ xs: 12 }}>
                        <Typography
                          variant="body2"
                          color="textSecondary">
                          Real System Time
                        </Typography>
                        <Typography
                          variant="body1"
                          fontFamily="monospace">
                          {formatDateTime(status.realDateTime)}
                        </Typography>
                      </Grid>
                      {status.isActive && status.currentFakeDateTime && (
                        <Grid size={{ xs: 12 }}>
                          <Typography
                            variant="body2"
                            color="textSecondary">
                            Current Fake Time
                          </Typography>
                          <Typography
                            variant="body1"
                            fontFamily="monospace"
                            color="warning.main">
                            {formatDateTime(status.currentFakeDateTime)}
                          </Typography>
                        </Grid>
                      )}
                      {status.configuredDateTime && (
                        <>
                          <Grid size={{ xs: 6 }}>
                            <Typography
                              variant="body2"
                              color="textSecondary">
                              Configured Date/Time
                            </Typography>
                            <Typography
                              variant="body1"
                              fontFamily="monospace">
                              {status.configuredDateTime}
                            </Typography>
                          </Grid>
                          <Grid size={{ xs: 6 }}>
                            <Typography
                              variant="body2"
                              color="textSecondary">
                              Time Zone
                            </Typography>
                            <Typography variant="body1">{status.timeZone}</Typography>
                          </Grid>
                          <Grid size={{ xs: 6 }}>
                            <Typography
                              variant="body2"
                              color="textSecondary">
                              Advance Time
                            </Typography>
                            <Typography variant="body1">{status.advanceTime ? "Yes" : "No (Frozen)"}</Typography>
                          </Grid>
                        </>
                      )}
                    </Grid>
                  </Box>
                ) : (
                  <Typography>No status available</Typography>
                )}
              </CardContent>
            </Card>
          </Grid>

          {/* Configuration Validator Card */}
          <Grid size={{ xs: 12, md: 6 }}>
            <Card>
              <CardContent>
                <Typography
                  variant="h6"
                  component="h2"
                  mb={2}>
                  Configuration Validator
                </Typography>

                {!status?.isAllowed ? (
                  <Alert severity="error">
                    <AlertTitle>Fake Time Not Allowed</AlertTitle>
                    Fake time is disabled in {status?.environment ?? "this environment"} for security reasons.
                  </Alert>
                ) : (
                  <form onSubmit={handleSubmit(onValidate)}>
                    <Grid
                      container
                      spacing={2}>
                      <Grid size={{ xs: 12 }}>
                        <Controller
                          name="enabled"
                          control={control}
                          render={({ field }) => (
                            <FormControlLabel
                              control={
                                <Checkbox
                                  {...field}
                                  checked={field.value}
                                />
                              }
                              label="Enable Fake Time"
                            />
                          )}
                        />
                      </Grid>

                      <Grid size={{ xs: 12 }}>
                        <Controller
                          name="fixedDateTime"
                          control={control}
                          render={({ field }) => (
                            <TextField
                              {...field}
                              fullWidth
                              label="Fixed Date/Time (ISO 8601)"
                              placeholder="2025-12-15T10:00:00"
                              helperText="Example: 2025-12-15T10:00:00"
                              disabled={!enabled}
                              size="small"
                            />
                          )}
                        />
                      </Grid>

                      <Grid size={{ xs: 12 }}>
                        <Controller
                          name="timeZone"
                          control={control}
                          render={({ field }) => (
                            <TextField
                              {...field}
                              fullWidth
                              label="Time Zone"
                              disabled={!enabled}
                              size="small"
                            />
                          )}
                        />
                      </Grid>

                      <Grid size={{ xs: 12 }}>
                        <Controller
                          name="advanceTime"
                          control={control}
                          render={({ field }) => (
                            <FormControlLabel
                              control={
                                <Checkbox
                                  {...field}
                                  checked={field.value}
                                  disabled={!enabled}
                                />
                              }
                              label="Advance Time (uncheck to freeze at exact moment)"
                            />
                          )}
                        />
                      </Grid>

                      <Grid size={{ xs: 12 }}>
                        <Button
                          type="submit"
                          variant="contained"
                          disabled={isValidating || !isDirty}
                          startIcon={isValidating ? <CircularProgress size={16} /> : undefined}>
                          Validate Configuration
                        </Button>
                        {status?.isRuntimeSwitchingEnabled && (
                          <Button
                            variant="contained"
                            color="warning"
                            sx={{ ml: 2 }}
                            disabled={isApplying || !isDirty}
                            onClick={handleSubmit(onApply)}
                            startIcon={isApplying ? <CircularProgress size={16} /> : undefined}>
                            Apply Now
                          </Button>
                        )}
                      </Grid>
                    </Grid>
                  </form>
                )}

                {validationError && (
                  <Alert
                    severity="error"
                    sx={{ mt: 2 }}>
                    {validationError}
                  </Alert>
                )}

                {applyError && (
                  <Alert
                    severity="error"
                    sx={{ mt: 2 }}>
                    {applyError}
                  </Alert>
                )}

                {applyResult && (
                  <Alert
                    severity={applyResult.isActive ? "warning" : "success"}
                    sx={{ mt: 2 }}>
                    <AlertTitle>{applyResult.isActive ? "Fake Time Activated" : "Fake Time Deactivated"}</AlertTitle>
                    {applyResult.message}
                  </Alert>
                )}

                {validationResult && (
                  <Alert
                    severity="success"
                    sx={{ mt: 2 }}>
                    <AlertTitle>Configuration Valid</AlertTitle>
                    {validationResult.message}
                    <Box mt={1}>
                      <Typography variant="body2">
                        To apply this configuration, update <code>appsettings.json</code> and restart the application:
                      </Typography>
                      <Box
                        component="pre"
                        sx={{
                          mt: 1,
                          p: 1,
                          bgcolor: "grey.100",
                          borderRadius: 1,
                          overflow: "auto",
                          fontSize: "0.75rem"
                        }}>
                        {JSON.stringify(
                          {
                            FakeTime: {
                              Enabled: validationResult.isActive || watch("enabled"),
                              FixedDateTime: watch("fixedDateTime") || null,
                              TimeZone: watch("timeZone") || defaultTimeZone,
                              AdvanceTime: watch("advanceTime")
                            }
                          },
                          null,
                          2
                        )}
                      </Box>
                    </Box>
                  </Alert>
                )}
              </CardContent>
            </Card>
          </Grid>

          {/* Information Card */}
          <Grid size={{ xs: 12 }}>
            <Card>
              <CardContent>
                <Typography
                  variant="h6"
                  component="h2"
                  mb={2}>
                  About Fake Time
                </Typography>
                <Typography
                  variant="body2"
                  paragraph>
                  The Fake Time feature allows testing time-sensitive operations (like year-end processing) by
                  simulating a specific date and time. This feature is{" "}
                  <strong>only available in non-production environments</strong>.
                </Typography>

                {status?.isRuntimeSwitchingEnabled ? (
                  <>
                    <Alert
                      severity="info"
                      sx={{ mb: 2 }}>
                      <AlertTitle>Runtime Switching Enabled</AlertTitle>
                      You can enable or disable fake time instantly using the <strong>Apply Now</strong> button. No
                      application restart is required.
                    </Alert>
                    <Typography
                      variant="body2"
                      paragraph>
                      <strong>To change fake time settings:</strong>
                    </Typography>
                    <ol>
                      <li>
                        <Typography variant="body2">Configure the desired date/time and options above</Typography>
                      </li>
                      <li>
                        <Typography variant="body2">
                          Click <strong>Validate Configuration</strong> to verify your settings
                        </Typography>
                      </li>
                      <li>
                        <Typography variant="body2">
                          Click <strong>Apply Now</strong> to activate the changes immediately
                        </Typography>
                      </li>
                    </ol>
                  </>
                ) : (
                  <>
                    <Typography
                      variant="body2"
                      paragraph>
                      <strong>Note:</strong> Runtime switching is not enabled. To change the fake time settings:
                    </Typography>
                    <ol>
                      <li>
                        <Typography variant="body2">
                          Edit the <code>FakeTime</code> section in <code>appsettings.json</code> or
                          <code>
                            appsettings.{"{"}Environment{"}"}.json
                          </code>
                        </Typography>
                      </li>
                      <li>
                        <Typography variant="body2">Restart the application</Typography>
                      </li>
                    </ol>
                  </>
                )}

                <Typography
                  variant="body2"
                  paragraph>
                  <strong>Configuration options:</strong>
                </Typography>
                <ul>
                  <li>
                    <Typography variant="body2">
                      <strong>Enabled:</strong> Set to <code>true</code> to activate fake time
                    </Typography>
                  </li>
                  <li>
                    <Typography variant="body2">
                      <strong>FixedDateTime:</strong> The simulated date/time (e.g., "2025-12-15T10:00:00")
                    </Typography>
                  </li>
                  <li>
                    <Typography variant="body2">
                      <strong>TimeZone:</strong> Time zone identifier (defaults to "Eastern Standard Time")
                    </Typography>
                  </li>
                  <li>
                    <Typography variant="body2">
                      <strong>AdvanceTime:</strong> If true, time advances from the starting point; if false, time stays
                      frozen
                    </Typography>
                  </li>
                </ul>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default FakeTimeManagement;
