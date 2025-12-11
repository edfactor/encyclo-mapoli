import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetNavigationQuery } from "../reduxstore/api/NavigationApi";
import {
  useLazyGetNavigationStatusQuery,
  useLazyUpdateNavigationStatusQuery
} from "../reduxstore/api/NavigationStatusApi";
import { RootState } from "../reduxstore/store";
import { NavigationDto } from "../types/navigation/navigation";
import StatusDropdown from "./StatusDropdown";

interface StatusDropdownActionNodeProps {
  initialStatus?: string;
  navigationId?: number;
  onStatusChange?: (newStatus: string, statusName?: string) => void;
  onSearchClicked?: () => void; // Callback when search is clicked - will auto-change from "Not Started" to "In Progress"
}

const StatusDropdownActionNode: React.FC<StatusDropdownActionNodeProps> = ({ onStatusChange, onSearchClicked }) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [currentStatus, setCurrentStatus] = useState("1");
  const [navigationObj, setNavigationObj] = useState<NavigationDto | null>(null);
  //const { isSuccess, data } = useGetNavigationStatusQuery({});
  const [triggerGetNavigationStatus, { data, isSuccess }] = useLazyGetNavigationStatusQuery({});

  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  const [triggerUpdate] = useLazyUpdateNavigationStatusQuery();
  const [triggerGetNavigation] = useLazyGetNavigationQuery();

  const handleStatusChange = async (newStatus: string) => {
    // Call the parent callback BEFORE the API call so we can detect the transition immediately
    if (onStatusChange) {
      const statusName = data?.navigationStatusList?.find((status) => status.id === parseInt(newStatus))?.name;
      onStatusChange(newStatus, statusName);
    }

    const result = await triggerUpdate({ navigationId: navigationObj?.id, statusId: parseInt(newStatus) });
    if (result.data?.isSuccessful) {
      setCurrentStatus(newStatus);
      if (hasToken) {
        triggerGetNavigation({ navigationId: undefined });
      }
    }
  };

  // Handle auto-change from "Not Started" to "In Progress" when search is clicked
  useEffect(() => {
    if (onSearchClicked && currentStatus === "1" && navigationObj) {
      // Auto-transition to "In Progress" (status 2)
      const autoChangeStatus = async () => {
        await handleStatusChange("2");
      };
      autoChangeStatus();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [onSearchClicked]); // Only runs when onSearchClicked changes (i.e., when search button is clicked)

  const getNavigationObjectBasedOnId = (navigationArray?: NavigationDto[], id?: number): NavigationDto | undefined => {
    if (navigationArray) {
      for (const item of navigationArray) {
        if (item.id == id) {
          return item;
        }
        if (item.items && item.items.length > 0) {
          const found = getNavigationObjectBasedOnId(item.items, id);
          if (found) {
            return found;
          }
        }
      }
    }
    return undefined;
  };

  useEffect(() => {
    if (hasToken) triggerGetNavigationStatus({});
    const obj = getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId ?? undefined);
    if (obj) {
      setNavigationObj(obj);
      setCurrentStatus(String(obj.statusId));
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [data, navigationList, currentNavigationId, hasToken, triggerGetNavigationStatus]);

  const isReadOnly = navigationObj?.isReadOnly ?? false;
  const readOnlyTooltip = "You are in read-only mode and cannot change the status of this page.";

  return (
    <div className="flex h-10 items-center gap-2">
      {isSuccess ? (
        <StatusDropdown
          navigationStatusList={data.navigationStatusList}
          onStatusChange={handleStatusChange}
          initialStatus={currentStatus}
          disabled={isReadOnly}
          tooltip={isReadOnly ? readOnlyTooltip : undefined}
        />
      ) : (
        <></>
      )}
    </div>
  );
};

export default StatusDropdownActionNode;
