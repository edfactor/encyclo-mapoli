import { Breadcrumbs, Link, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Location, useLocation, useNavigate } from "react-router-dom";
import { CAPTIONS, ROUTES } from "../../constants";
import { RootState } from "../../reduxstore/store";
import { getReadablePathName } from "../../utils/getReadablePathName";
import { sanitizePath } from "../../utils/pathValidation";
import { BreadcrumbItem } from "./DSMBreadcrumbItem";

interface NavigationItem {
  title?: string;
  subTitle?: string;
  url?: string;
  items?: NavigationItem[];
  [key: string]: unknown;
}

interface DSMDynamicBreadcrumbsProps {
  separator?: string;
  customItems?: BreadcrumbItem[];
}

const EXCLUDED_PATHS = ["/login", "/login/callback", "/unauthorized", "/dev-debug", "/documentation"];

const DSMDynamicBreadcrumbs: React.FC<DSMDynamicBreadcrumbsProps> = ({ separator = "/", customItems }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const [navigationHistory, setNavigationHistory] = useState<Location[]>([]);
  const navigationState = useSelector((state: RootState) => state.navigation);

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
      // Use centralized path validation utility to prevent open redirect
      const safePath = sanitizePath(path);

      const newHistory = navigationHistory.slice(0, index + 1);
      setNavigationHistory(newHistory);
      navigate(safePath);
    }
  };
  //build breadcrumb items from navigation history
  const buildBreadcrumbItems = (): BreadcrumbItem[] => {
    if (customItems) return customItems;

    return navigationHistory
      .slice(0, -1)
      .filter((loc) => loc.pathname !== "/" && loc.pathname !== "")
      .slice(-3)
      .map((location) => {
        const matched = findNavigationItemByUrl(location.pathname);
        return {
          label: matched
            ? `${matched.title}${matched.subTitle ? ` (${matched.subTitle})` : ""}`
            : getReadablePathName(location.pathname),
          path: location.pathname
        };
      });
  };
  const findNavigationItemByUrl = (path: string): NavigationItem | undefined => {
    const navData = navigationState?.navigationData;
    if (!navData?.navigation) return undefined;

    const cleanPath = path.replace(/^\/+/, "");

    const findByUrl = (items: NavigationItem[]): NavigationItem | undefined => {
      for (const item of items) {
        if (item.url && item.url.replace(/^\/+/, "") === cleanPath) return item;
        if (item.items && item.items.length > 0) {
          const sub = findByUrl(item.items);
          if (sub) return sub;
        }
      }
      return undefined;
    };

    return findByUrl(navData.navigation as unknown as NavigationItem[]);
  };

  const getCurrentPageLabel = (): string => {
    if (navigationHistory.length === 0) return "";
    const currentLocation = navigationHistory[navigationHistory.length - 1];

    const basePath = currentLocation.pathname.replace(/^\/+|\/+$/g, "").split("/")[0];
    if (basePath === ROUTES.MASTER_INQUIRY) {
      return CAPTIONS.MASTER_INQUIRY_SHORT;
    }

    const matched = findNavigationItemByUrl(currentLocation.pathname);
    return matched
      ? `${matched.title}${matched.subTitle ? ` (${matched.subTitle})` : ""}`
      : getReadablePathName(currentLocation.pathname);
  };

  const items = buildBreadcrumbItems();

  const currentPageLabel = getCurrentPageLabel();

  const visibleItems =
    !customItems &&
    currentPageLabel &&
    items.length > 0 &&
    items[items.length - 1].label.trim().toLocaleLowerCase() === currentPageLabel.trim().toLocaleLowerCase()
      ? items.slice(0, -1)
      : items;

  if (navigationHistory.length <= 1) {
    return null;
  }

  return (
    <div style={{ minHeight: "24px" }}>
      <Breadcrumbs
        separator={separator}
        maxItems={3}>
        {visibleItems.map((item, index) => (
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
