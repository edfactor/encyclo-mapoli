/**
 * Recursive Navigation Item Component
 *
 * Renders a navigation item and its children recursively, supporting arbitrary nesting depth.
 * This replaces the hardcoded 3-level structure (MenuLevel → TopPage → SubPages).
 */

import { ExpandLess, ExpandMore } from "@mui/icons-material";
import { Box, Chip, Collapse, List, ListItemButton, ListItemText } from "@mui/material";
import { FC, useCallback, useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { NavigationDto } from "../../reduxstore/types";

// Shared highlight color constant
const HIGHLIGHT_COLOR = "#0258A5";

export interface RecursiveNavItemProps {
  item: NavigationDto;
  level: number;
  maxAutoExpandDepth?: number;
  onNavigate?: (item: NavigationDto) => void;
}

/**
 * Recursively renders a navigation item with collapsible children
 *
 * maxAutoExpandDepth default is 0 (collapsed), meaning groups are collapsed by default
 * They will only expand if:
 * 1. User manually clicks to expand
 * 2. The item or its descendants contain the active route
 */
export const RecursiveNavItem: FC<RecursiveNavItemProps> = ({ item, level, maxAutoExpandDepth = 0, onNavigate }) => {
  const navigate = useNavigate();
  const location = useLocation();

  // Load expanded state from localStorage with fallback
  const getStoredExpanded = useCallback(() => {
    try {
      const stored = localStorage.getItem(`nav-expanded-${item.id}`);
      return stored ? JSON.parse(stored) : false;
    } catch {
      return false;
    }
  }, [item.id]);

  const [expanded, setExpanded] = useState(getStoredExpanded);

  const hasChildren = item.items && item.items.length > 0;
  const isNavigable = item.url && item.url.length > 0;
  const currentPath = location.pathname.replace(/^\/+/, "");
  const itemPath = item.url?.replace(/^\/+/, "");
  const isActive = currentPath === itemPath;

  // Check if this item or any descendant contains the active path
  const containsActivePath = useCallback(
    (navItem: NavigationDto): boolean => {
      const navItemPath = navItem.url?.replace(/^\/+/, "");
      if (currentPath === navItemPath) return true;

      if (navItem.items && navItem.items.length > 0) {
        return navItem.items.some((child) => containsActivePath(child));
      }

      return false;
    },
    [currentPath]
  );

  const hasActiveChild = containsActivePath(item);

  // Auto-expand logic: expand if level is within auto-expand depth OR if this item contains the active path
  // This only runs on mount or when the active path changes, not on every render
  useEffect(() => {
    const storedExpanded = getStoredExpanded();

    // Don't override user's manual collapse/expand choices unless necessary
    if (level <= maxAutoExpandDepth && !storedExpanded) {
      setExpanded(false);
    } else if (hasActiveChild && !isActive && !storedExpanded) {
      // Auto-expand parent items that contain the active child
      setExpanded(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [location.pathname]); // Only re-run when location changes

  // Re-check localStorage when location changes (for page search navigation)
  useEffect(() => {
    const storedExpanded = getStoredExpanded();
    // Only restore expanded state from localStorage, don't force it
    if (storedExpanded) {
      setExpanded(storedExpanded);
    }
  }, [location.pathname, getStoredExpanded]);

  // Persist expanded state
  useEffect(() => {
    try {
      localStorage.setItem(`nav-expanded-${item.id}`, JSON.stringify(expanded));
    } catch (error) {
      console.error("Error saving expanded state:", error);
    }
  }, [expanded, item.id]);

  const handleClick = () => {
    if (item.disabled) return;

    if (hasChildren) {
      // Toggle expansion for items with children
      setExpanded(!expanded);
    }

    if (isNavigable) {
      // Navigate if item has a route
      const absolutePath = item.url.startsWith("/") ? item.url : `/${item.url}`;

      // Store navigation ID for read-only checks and other features
      if (item.id) {
        try {
          localStorage.setItem("navigationId", item.id.toString());
        } catch (error) {
          console.error("Error saving navigation ID:", error);
        }
      }

      navigate(absolutePath);
      onNavigate?.(item);
    }
  };

  // Calculate indentation based on nesting level
  const indentation = 2 + level * 2;

  return (
    <>
      <ListItemButton
        onClick={handleClick}
        disabled={item.disabled}
        sx={{
          pl: indentation,
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          py: 1,
          minHeight: 0,
          backgroundColor: isActive ? `${HIGHLIGHT_COLOR}15` : "transparent",
          borderLeft: isActive ? `4px solid ${HIGHLIGHT_COLOR}` : "4px solid transparent",
          opacity: item.disabled ? 0.5 : 1,
          cursor: item.disabled ? "not-allowed" : "pointer",
          "&:hover": {
            backgroundColor: item.disabled
              ? "transparent"
              : isActive
                ? `${HIGHLIGHT_COLOR}25`
                : (theme) => theme.palette.action.hover
          }
        }}>
        <Box sx={{ display: "flex", flexDirection: "column", flex: 1 }}>
          <ListItemText
            primary={item.title}
            secondary={item.subTitle || undefined}
            primaryTypographyProps={{
              sx: {
                fontSize: "0.875rem",
                fontWeight: isActive ? "bold" : "normal",
                color: isActive ? HIGHLIGHT_COLOR : "inherit"
              }
            }}
            secondaryTypographyProps={{
              sx: {
                fontSize: "0.75rem",
                color: (theme) => theme.palette.text.secondary
              }
            }}
            sx={{
              margin: 0
            }}
          />
        </Box>

        <Box sx={{ display: "flex", alignItems: "right", gap: 1 }}>
          {/* Status chip - only show on leaf nodes (items without children) */}
          {item.statusName && !hasChildren && (
            <Chip
              variant="outlined"
              label={item.statusName}
              className={` ${item.statusName === "In Progress" ? "bg-dsm-action-secondary-hover text-dsm-action" : ""} ${item.statusName === "On Hold" ? "bg-yellow-50 text-yellow-700" : ""} ${item.statusName === "Complete" ? "bg-dsm-action-secondary-hover text-dsm-action" : ""} ${!["In Progress", "On Hold", "Complete"].includes(item.statusName || "") ? "border-dsm-grey-secondary text-dsm-grey-secondary" : ""} font-medium`}
              size="small"
            />
          )}

          {/* Expand/collapse icon for items with children */}
          {hasChildren && (expanded ? <ExpandLess /> : <ExpandMore />)}
        </Box>
      </ListItemButton>

      {/* Recursively render children */}
      {hasChildren && (
        <Collapse
          in={expanded}
          timeout="auto"
          unmountOnExit>
          <List disablePadding>
            {item.items
              .filter((child) => child.isNavigable ?? true)
              .map((childItem) => (
                <RecursiveNavItem
                  key={childItem.id}
                  item={childItem}
                  level={level + 1}
                  maxAutoExpandDepth={maxAutoExpandDepth}
                  onNavigate={onNavigate}
                />
              ))}
          </List>
        </Collapse>
      )}
    </>
  );
};

export default RecursiveNavItem;
