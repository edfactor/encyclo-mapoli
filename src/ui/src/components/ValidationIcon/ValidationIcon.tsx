import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { CrossReferenceValidationGroup } from "../../types/validation/cross-reference-validation";

/**
 * Props for ValidationIcon component
 */
interface ValidationIconProps {
  /** The validation group containing field validations (or null if not available) */
  validationGroup: CrossReferenceValidationGroup | null;

  /** The field name to check for in the validations array */
  fieldName: string;

  /** Optional CSS class name for additional styling */
  className?: string;

  /** Optional click handler for the icon */
  onClick?: () => void;
}

/**
 * Displays an info icon if the provided validation group contains a validation
 * entry matching the specified field name. Renders nothing if no match is found
 * or if the validation group is null.
 *
 * The icon color indicates validation status:
 * - Green: Field validation passed (isValid: true)
 * - Orange: Field validation failed (isValid: false)
 *
 * @example
 * // With a validation group containing the field
 * <ValidationIcon
 *   validationGroup={distributionsValidationGroup}
 *   fieldName="DistributionTotals"
 *   onClick={() => handleValidationClick("DistributionTotals")}
 * />
 *
 * // With null validation group (renders nothing)
 * <ValidationIcon
 *   validationGroup={null}
 *   fieldName="DistributionTotals"
 * />
 */
export const ValidationIcon: React.FC<ValidationIconProps> = ({
  validationGroup,
  fieldName,
  className,
  onClick
}) => {
  // Return nothing if no validation group provided
  if (!validationGroup) {
    return null;
  }

  // Find the validation entry matching the field name
  const validation = validationGroup.validations.find((v) => v.fieldName === fieldName);

  // Return nothing if no matching validation found
  if (!validation) {
    return null;
  }

  // Determine icon color based on validation status
  const iconColorClass = validation.isValid ? "text-green-500" : "text-orange-500";

  return (
    <div
      className={`inline-block ${onClick ? "cursor-pointer" : ""} ${className ?? ""}`}
      onClick={onClick}
      role={onClick ? "button" : undefined}
      tabIndex={onClick ? 0 : undefined}
      onKeyDown={
        onClick
          ? (e) => {
              if (e.key === "Enter" || e.key === " ") {
                e.preventDefault();
                onClick();
              }
            }
          : undefined
      }
      aria-label={`Validation status for ${fieldName}: ${validation.isValid ? "valid" : "invalid"}`}>
      <InfoOutlinedIcon
        className={iconColorClass}
        fontSize="small"
      />
    </div>
  );
};
