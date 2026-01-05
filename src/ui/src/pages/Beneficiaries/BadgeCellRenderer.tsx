import { ICellRendererParams } from "ag-grid-community";
import { Button } from "@mui/material";
import { BeneficiaryDto } from "../../types";

interface BadgeCellRendererProps extends ICellRendererParams {
  data: BeneficiaryDto;
  onBadgeClick?: (beneficiary: BeneficiaryDto) => void;
}

export const BadgeCellRenderer = (props: BadgeCellRendererProps) => {
  const { data, onBadgeClick } = props;
  const badgeNumber = data?.badgeNumber;
  const psnSuffix = data?.psnSuffix;

  if (!badgeNumber) return "";

  const displayValue = psnSuffix ? `${badgeNumber}${psnSuffix}` : badgeNumber;

  if (onBadgeClick) {
    return (
      <Button
        variant="text"
        onClick={() => onBadgeClick(data)}
        sx={{
          textTransform: "none",
          padding: 0,
          minWidth: "auto",
          textDecoration: "underline",
          "&:hover": {
            textDecoration: "underline"
          }
        }}>
        {displayValue}
      </Button>
    );
  }

  return <span>{displayValue}</span>;
};
