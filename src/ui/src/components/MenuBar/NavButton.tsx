import { Button } from "@mui/material";
import { FC } from "react";

type MyProps = {
  isUnderlined: boolean;
  label: string;
  onClick: () => void;
  disabled?: boolean;
};
const NavButton: FC<MyProps> = ({ isUnderlined, label, onClick, disabled }) => {
  return (
    <span
      className={`${
        isUnderlined
          ? "border-0 border-b-2 border-solid border-white"
          : "border-0 border-b-2 border-solid border-transparent"
      }`}>
      <Button
        className="h-full"
        aria-haspopup="true"
        onClick={disabled ? undefined : onClick}
        disableRipple={disabled}
        sx={{ color: "inherit", cursor: disabled ? "default" : "pointer", opacity: disabled ? 0.6 : 1 }}>
        {label}
      </Button>
    </span>
  );
};
export default NavButton;
