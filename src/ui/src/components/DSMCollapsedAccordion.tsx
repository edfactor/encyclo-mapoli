import React, { useState } from "react";
import { Accordion, AccordionSummary, AccordionDetails, Typography, Box, Chip } from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";

interface DSMCollapsedAccordionProps {
  title: string;
  children: React.ReactNode;
  actionButtonText?: string;
  onActionClick?: () => void;
  isCollapsedOnRender?: boolean;
  className?: string;
  status?: {
    label: string;
    color?: "default" | "primary" | "secondary" | "error" | "info" | "success" | "warning";
  };
  disabled?: boolean;
  expandable?: boolean;
}

const DSMCollapsedAccordion: React.FC<DSMCollapsedAccordionProps> = ({
  title,
  children,
  actionButtonText,
  onActionClick,
  isCollapsedOnRender = true,
  className,
  status,
  disabled = false,
  expandable = true
}) => {
  const [expanded, setExpanded] = useState(!isCollapsedOnRender);

  const handleChange = (_event: React.SyntheticEvent, isExpanded: boolean) => {
    if (expandable) {
      setExpanded(isExpanded);
    }
  };

  const handleActionClick = (event: React.MouseEvent) => {
    event.stopPropagation();
    onActionClick?.();
  };

  return (
    <Accordion
      expanded={expandable ? expanded : false}
      onChange={handleChange}
      className={className}
      disabled={disabled}
      sx={{
        boxShadow: "none",
        "&:before": {
          display: "none"
        },
        margin: "0 !important",
        "& + &": {
          marginTop: "1px"
        }
      }}>
      <AccordionSummary
        expandIcon={expandable ? <ExpandMoreIcon /> : null}
        sx={{
          borderBottom: "1px solid",
          borderColor: "divider",
          height: "72px",
          minHeight: "72px !important",
          flexDirection: "row-reverse",
          "&.Mui-expanded": {
            minHeight: "72px !important",
            height: "72px"
          },
          "& .MuiAccordionSummary-expandIconWrapper": {
            marginRight: "16px",
            marginLeft: "0"
          },
          "& .MuiAccordionSummary-content": {
            margin: "0",
            marginLeft: "16px",
            "&.Mui-expanded": {
              margin: "0"
            }
          }
        }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            width: "100%",
            pr: 2
          }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 2,
              flexWrap: "nowrap"
            }}>
            {actionButtonText && (
              <Box
                sx={{
                  width: "120px", // Set a fixed width box container for the button
                  flexShrink: 0, // Prevent shrinking
                  display: "flex",
                  justifyContent: "flex-start" // Align button to left within its container
                }}>
                <Box
                  onClick={handleActionClick}
                  sx={{
                    textTransform: "uppercase",
                    fontWeight: "medium",
                    alignSelf: "center",
                    whiteSpace: "nowrap", // Prevent text from wrapping
                    flexShrink: 0,
                    cursor: disabled ? "not-allowed" : "pointer",
                    color: disabled ? "rgba(0, 0, 0, 0.26)" : "#0258A5",
                    padding: "6px 14px", // Increased padding for more space
                    fontSize: "0.90rem", // Slightly smaller font size
                    border: "1px solid",
                    borderColor: disabled ? "rgba(0, 0, 0, 0.26)" : "#0258A5",
                    borderRadius: "0px", // Square corners
                    display: "inline-block",
                    textAlign: "center",
                    pointerEvents: disabled ? "none" : "auto"
                  }}>
                  {actionButtonText}
                </Box>
              </Box>
            )}
            <Typography
              sx={{
                color: "#0258A5",
                fontSize: "1.25rem",
                fontWeight: 500,
                alignSelf: "center"
              }}
              variant="h2">
              {title}
            </Typography>
            {status && (
              <Chip
                variant="outlined"
                label={status.label}
                color={status.color}
                size="small"
                sx={{
                  alignSelf: "center",
                  flexShrink: 0
                }}
              />
            )}
          </Box>
        </Box>
      </AccordionSummary>
      <AccordionDetails
        sx={{
          backgroundColor: "white",
          padding: "0"
        }}>
        {children}
      </AccordionDetails>
    </Accordion>
  );
};

export default DSMCollapsedAccordion;
