import Typography from "@mui/material/Typography";
import React from "react";

export type ColorKeyOption = {
  key: string;
  color: string;
  border?: string;
};

export type ColorKeyProps = {
  options: ColorKeyOption[];
};

const ColorKey: React.FC<ColorKeyProps> = ({ options }) => {
  const ColorKeyItemStyle: React.CSSProperties = {
    display: "flex",
    padding: "6px 8px",
    alignItems: "center",
    gap: "8px"
  };
  return (
    <div style={{ display: "flex", alignItems: "flex-start", gap: "10px" }}>
      <div style={ColorKeyItemStyle}>
        <Typography
          fontSize={"14px"}
          lineHeight={"24px"}>
          {"KEY"}
        </Typography>
      </div>
      {options.map((option: ColorKeyOption) => {
        return (
          <div style={ColorKeyItemStyle}>
            <div
              style={
                option.border
                  ? {
                      width: "12px",
                      height: "12px",
                      background: option.color,
                      border: `1px solid ${option.border}`
                    }
                  : {
                      width: "12px",
                      height: "12px",
                      background: option.color
                    }
              }></div>
            <Typography
              lineHeight={"24px"}
              fontSize={"14px"}>
              {option.key}
            </Typography>
          </div>
        );
      })}
    </div>
  );
};

export default ColorKey;
