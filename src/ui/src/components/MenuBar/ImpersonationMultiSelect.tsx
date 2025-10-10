import { SelectChangeEvent } from "@mui/material";
import MultipleSelectCheckmarks from "./MultiSelect";

export type ImpersonationMultiSelectProps = {
  impersonationRoles?: string[];
  setCurrentRoles: (value: string[]) => void;
  currentRoles: string[];
};
export const ImpersonationMultiSelect: React.FC<ImpersonationMultiSelectProps> = ({
  impersonationRoles = [],
  setCurrentRoles,
  currentRoles
}) => {
  const handleImpersonationChange = (event: SelectChangeEvent<string[]>) => {
    const {
      target: { value }
    } = event;

    const currentRoles = typeof value === "string" ? value.split(",") : value;
    setCurrentRoles(currentRoles);
  };

  const handleClose = () => {};

  return (
    <div className="flex items-center justify-center">
      <div className="pr-2 text-[16px] uppercase text-white">impersonate</div>
      <div className="mb-1 mt-1">
        <MultipleSelectCheckmarks
          options={impersonationRoles}
          handleChange={handleImpersonationChange}
          handleClose={handleClose}
          value={currentRoles}
          ariaLabel="roles"
          dataTestId="impersonate-input"
        />
      </div>
    </div>
  );
};

export default ImpersonationMultiSelect;
