import { MenuItem, Select, SelectChangeEvent, Tooltip } from "@mui/material";
import { NavigationStatusDto } from "../types/navigation/navigation";

interface StatusDropdownProps {
  initialStatus?: string;
  navigationStatusList?: NavigationStatusDto[];
  onStatusChange: (newStatus: string) => void;
  disabled?: boolean;
  tooltip?: string;
}

const StatusDropdown = ({
  initialStatus = "1",
  navigationStatusList,
  onStatusChange,
  disabled = false,
  tooltip
}: StatusDropdownProps) => {
  const handleChange = (event: SelectChangeEvent<string>) => {
    const newStatus = event.target.value;
    onStatusChange(newStatus);
  };

  const selectComponent = (
    <Select
      size="small"
      value={initialStatus}
      onChange={handleChange}
      disabled={disabled}
      fullWidth>
      {navigationStatusList?.map((value) => (
        <MenuItem
          key={value.id}
          value={String(value.id)}>
          {value.name}
        </MenuItem>
      ))}
    </Select>
  );

  // Wrap in tooltip if disabled and tooltip is provided
  if (disabled && tooltip) {
    return (
      <Tooltip
        title={tooltip}
        arrow>
        <span>{selectComponent}</span>
      </Tooltip>
    );
  }

  return selectComponent;
};

export default StatusDropdown;
