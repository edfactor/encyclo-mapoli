import { ReactNode } from "react";

/**
 * Accessibility Helper Components - WCAG 2.2 AA compliant React components
 *
 * Provides reusable React components for implementing accessible forms,
 * error announcements, loading states, and screen reader support.
 *
 * For utility functions, see accessibilityUtils.ts
 */

/**
 * VisuallyHidden - Component for screen-reader-only content
 * Implements visually-hidden/sr-only pattern for WCAG compliance
 *
 * @example
 * <VisuallyHidden>Loading search results...</VisuallyHidden>
 */
export const VisuallyHidden: React.FC<{ children: ReactNode; id?: string }> = ({ children, id }) => {
  return (
    <span
      id={id}
      style={{
        position: "absolute",
        width: "1px",
        height: "1px",
        padding: "0",
        margin: "-1px",
        overflow: "hidden",
        clip: "rect(0, 0, 0, 0)",
        whiteSpace: "nowrap",
        border: "0"
      }}>
      {children}
    </span>
  );
};

/**
 * LoadingAnnouncement - Accessible loading state component
 * Announces loading state to screen readers using aria-live
 *
 * @param isLoading - Whether content is loading
 * @param loadingMessage - Message to announce when loading (default: "Loading data...")
 * @param loadedMessage - Message to announce when loaded (default: "Data loaded")
 * @example
 * <LoadingAnnouncement isLoading={isFetching} />
 */
export const LoadingAnnouncement: React.FC<{
  isLoading: boolean;
  loadingMessage?: string;
  loadedMessage?: string;
}> = ({ isLoading, loadingMessage = "Loading data...", loadedMessage = "Data loaded" }) => {
  return (
    <div
      aria-live="polite"
      aria-atomic="true"
      role="status">
      <VisuallyHidden>{isLoading ? loadingMessage : loadedMessage}</VisuallyHidden>
    </div>
  );
};

/**
 * ErrorAnnouncement - Accessible error message component
 * Announces validation errors to screen readers
 *
 * @param hasError - Whether error exists
 * @param errorMessage - Error message to display
 * @param fieldName - Name of field (for ID generation)
 * @param isInline - Whether error is inline validation (polite) or form submission (assertive)
 * @example
 * <ErrorAnnouncement
 *   hasError={!!errors.ssn}
 *   errorMessage={errors.ssn?.message}
 *   fieldName="ssn"
 *   isInline={true}
 * />
 */
export const ErrorAnnouncement: React.FC<{
  hasError: boolean;
  errorMessage?: string;
  fieldName: string;
  isInline?: boolean;
}> = ({ hasError, errorMessage, fieldName, isInline = true }) => {
  if (!hasError || !errorMessage) return null;

  return (
    <div
      id={`${fieldName}-error`}
      role={isInline ? undefined : "alert"}
      aria-live={isInline ? "polite" : "assertive"}
      aria-atomic="true"
      style={{ color: "#d32f2f", fontSize: "0.75rem", marginTop: "4px" }}>
      {errorMessage}
    </div>
  );
};

/**
 * Skip link component for keyboard navigation
 * Allows users to skip repetitive navigation
 *
 * @param targetId - ID of target element to skip to
 * @param label - Link label text
 * @example
 * <SkipLink targetId="main-content" label="Skip to main content" />
 */
export const SkipLink: React.FC<{ targetId: string; label: string }> = ({ targetId, label }) => {
  return (
    <a
      href={`#${targetId}`}
      style={{
        position: "absolute",
        left: "-9999px",
        zIndex: 999,
        padding: "1em",
        backgroundColor: "#0258a5",
        color: "white",
        textDecoration: "none",
        outline: "none"
      }}
      onFocus={(e) => {
        e.currentTarget.style.left = "0";
      }}
      onBlur={(e) => {
        e.currentTarget.style.left = "-9999px";
      }}>
      {label}
    </a>
  );
};
