/**
 * Response DTO for fake time status endpoint.
 */
export interface FakeTimeStatusResponse {
  /**
   * Whether fake time is currently active.
   */
  isActive: boolean;

  /**
   * Whether fake time is allowed in the current environment.
   * False in Production environments.
   */
  isAllowed: boolean;

  /**
   * Whether runtime switching between real and fake time is supported.
   * When true, fake time can be enabled/disabled via the API without restarting the application.
   */
  isRuntimeSwitchingEnabled: boolean;

  /**
   * The current fake date/time when active (ISO 8601 format).
   * Null when fake time is not active.
   */
  currentFakeDateTime?: string | null;

  /**
   * The configured fixed date/time (ISO 8601 format).
   */
  configuredDateTime?: string | null;

  /**
   * The configured time zone identifier.
   */
  timeZone?: string | null;

  /**
   * Whether time advances automatically from the configured starting point.
   */
  advanceTime: boolean;

  /**
   * The current environment name (Development, QA, UAT, Production).
   */
  environment: string;

  /**
   * The system's real current date/time for reference (ISO 8601 format).
   */
  realDateTime: string;

  /**
   * An informational message about the fake time status.
   */
  message?: string | null;
}

/**
 * Response DTO for user-specific fake time status.
 * This is per-user, independent of the global server time.
 */
export interface UserFakeTimeStatusResponse {
  /**
   * Whether the current user has fake time active.
   */
  isActive: boolean;

  /**
   * Whether per-user fake time is allowed in this environment.
   * Always false in Production.
   */
  isAllowed: boolean;

  /**
   * The current fake date/time for this user, if active.
   */
  currentFakeDateTime?: string | null;

  /**
   * The configured fixed date/time string for this user.
   */
  configuredDateTime?: string | null;

  /**
   * The timezone being used for this user's fake time.
   */
  timeZone?: string | null;

  /**
   * Whether time advances from the configured start for this user.
   */
  advanceTime: boolean;

  /**
   * The current real system date/time for reference.
   */
  realDateTime: string;

  /**
   * The user ID this setting applies to.
   */
  userId?: string | null;

  /**
   * Human-readable message about the current state.
   */
  message?: string | null;
}

/**
 * Response containing a list of all users with fake time settings.
 */
export interface AllUserFakeTimeResponse {
  /**
   * Total number of users with fake time settings.
   */
  totalUsers: number;

  /**
   * List of all user fake time settings.
   */
  users: UserFakeTimeStatusResponse[];
}

/**
 * Request DTO for validating fake time configuration.
 */
export interface SetFakeTimeRequest {
  /**
   * Whether fake time should be enabled.
   */
  enabled: boolean;

  /**
   * The fixed date/time to use (ISO 8601 format without timezone).
   * Example: "2025-12-15T10:00:00"
   */
  fixedDateTime?: string | null;

  /**
   * The time zone identifier. Defaults to "Eastern Standard Time" if not specified.
   */
  timeZone?: string | null;

  /**
   * Whether time should advance from the configured starting point.
   * If false, time remains frozen at the exact configured moment.
   */
  advanceTime: boolean;
}
