import { ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import { ValidationIcon } from "../../../components/ValidationIcon";

interface ForfeituresCellRendererProps extends ICellRendererParams {
  onValidationClick?: (fieldName: string) => void;
}

/**
 * Custom cell renderer for the Forfeitures column that displays:
 * - A validation icon (if validation data is available)
 * - The formatted forfeiture amount
 */
export const ForfeituresCellRenderer: React.FC<ForfeituresCellRendererProps> = (params) => {
  const { data, value, onValidationClick } = params;

  const handleClick = onValidationClick ? () => onValidationClick("ForfeitureTotal") : undefined;

  return (
    <div className="flex items-center gap-1">
      <ValidationIcon
        validationGroup={data?.validation || null}
        fieldName="ForfeitureTotal"
        onClick={handleClick}
      />
      <span>{numberToCurrency(+value)}</span>
    </div>
  );
};
