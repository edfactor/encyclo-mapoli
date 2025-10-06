import { Info } from "@mui/icons-material";
import { useSelector } from "react-redux";
import { RootState } from "../../reduxstore/store";

interface StatusReadOnlyInfoProps {
  /**
   * Optional custom message to display. If not provided, a default message will be shown.
   */
  message?: string;
}

/**
 * Component that displays an informational banner explaining why the page is read-only
 * due to page status not being "In Progress". Instructs users to change status to enable modifications.
 */
const StatusReadOnlyInfo = ({ message }: StatusReadOnlyInfoProps) => {
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");

  // Helper function to find current navigation item
  const getCurrentNavigation = () => {
    const findNavigation = (items: any[]): any => {
      for (const item of items) {
        if (item.id === currentNavigationId) return item;
        if (item.items?.length > 0) {
          const found = findNavigation(item.items);
          if (found) return found;
        }
      }
      return null;
    };
    return navigationList?.navigation ? findNavigation(navigationList.navigation) : null;
  };

  const currentNavigation = getCurrentNavigation();
  const currentStatusName = currentNavigation?.statusName || "Not Started";

  const defaultMessage = `This page is currently in "${currentStatusName}" status. To add or modify data, please change the page status to "In Progress" using the status dropdown in the upper right corner.`;

  return (
    <div className="missive-alert missive-info status-readonly-info">
      <div className="status-readonly-info-content">
        <Info className="status-readonly-info-icon" />
        <div>
          <strong>Page Locked for Modifications</strong>
          <p>{message || defaultMessage}</p>
        </div>
      </div>
    </div>
  );
};

export default StatusReadOnlyInfo;
