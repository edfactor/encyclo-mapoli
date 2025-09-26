import { Breadcrumbs, Link, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { Location, useLocation, useNavigate } from "react-router-dom";
import { getReadablePathName } from "../../utils/getReadablePathName";
import { BreadcrumbItem } from "./DSMBreadcrumbItem";

interface DSMDynamicBreadcrumbsProps {
  separator?: string;
  customItems?: BreadcrumbItem[];
}

// Prevents login from showing up as first breadcrumb after Auth Redirect
// Also excludes unauthorized page and dev debug from breadcrumb history
const EXCLUDED_PATHS = ["/login", "/login/callback", "/unauthorized", "/dev-debug"];

const DSMDynamicBreadcrumbs: React.FC<DSMDynamicBreadcrumbsProps> = ({ separator = "/", customItems }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const [navigationHistory, setNavigationHistory] = useState<Location[]>([]);

  useEffect(() => {
    if (EXCLUDED_PATHS.some((path) => location.pathname.startsWith(path))) {
      return;
    }

    setNavigationHistory((prevHistory) => {
      // Remove duplicate paths to prevent A -> B -> C -> A showing A twice
      const filteredHistory = prevHistory.filter((h) => h.pathname !== location.pathname);
      return [...filteredHistory, location];
    });
  }, [location]);

  const handleClick = (path: string, index: number) => {
    if (path !== location.pathname) {
      // Sanitize path to prevent open redirect - only allow internal paths
      const sanitizedPath = path.startsWith("/") && !path.startsWith("//") && !path.includes("://") ? path : "/";

      const newHistory = navigationHistory.slice(0, index + 1);
      setNavigationHistory(newHistory);
      navigate(sanitizedPath);
    }
  };

  const buildBreadcrumbItems = (): BreadcrumbItem[] => {
    if (customItems) return customItems;

    return navigationHistory
      .slice(0, -1)
      .filter((loc) => loc.pathname !== "/" && loc.pathname !== "")
      .slice(-3)
      .map((location) => ({
        label: getReadablePathName(location.pathname),
        path: location.pathname
      }));
  };

  const getCurrentPageLabel = (): string => {
    if (navigationHistory.length === 0) return "";
    const currentLocation = navigationHistory[navigationHistory.length - 1];
    return getReadablePathName(currentLocation.pathname);
  };

  const items = buildBreadcrumbItems();

  const currentPageLabel = getCurrentPageLabel();

  if (navigationHistory.length <= 1) {
    return null;
  }

  return (
    <div style={{ minHeight: "24px" }}>
      <Breadcrumbs
        separator={separator}
        maxItems={3}>
        {items.map((item, index) => (
          <Link
            key={index}
            color="inherit"
            underline="hover"
            onClick={() => handleClick(item.path, index)}
            sx={{ cursor: "pointer" }}>
            {item.label}
          </Link>
        ))}
        {currentPageLabel && (
          <Typography
            color="textPrimary"
            sx={{ fontWeight: "medium" }}>
            {currentPageLabel}
          </Typography>
        )}
      </Breadcrumbs>
    </div>
  );
};

export default DSMDynamicBreadcrumbs;
