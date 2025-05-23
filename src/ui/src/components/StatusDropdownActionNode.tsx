import { useEffect, useState } from "react";
import StatusDropdown from "./StatusDropdown";
import { useGetNavigationStatusQuery, useLazyUpdateNavigationStatusQuery, useUpdateNavigationStatusQuery } from "reduxstore/api/NavigationStatusApi";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { NavigationDto } from "reduxstore/types";
import { useNavigate, useLocation } from "react-router-dom";
import { useLazyGetNavigationQuery } from "reduxstore/api/NavigationApi";

interface StatusDropdownActionNodeProps {
  initialStatus?: string;
  navigationId?: number;
}

const StatusDropdownActionNode: React.FC<StatusDropdownActionNodeProps> = ({ initialStatus, navigationId }) => {
  const [currentStatus, setCurrentStatus] = useState("1");
  const [navigationObj, setNavigationObj] = useState<NavigationDto | null>(null);
  const { isSuccess, data } = useGetNavigationStatusQuery({});
  const location = useLocation();
  const navigationList = useSelector(
    (state: RootState) => state.navigation.navigationData
  );
  const [triggerUpdate] = useLazyUpdateNavigationStatusQuery();
  const [triggerGetNavigation] = useLazyGetNavigationQuery();
  const handleStatusChange = async (newStatus: string) => {
    const result = await triggerUpdate({ navigationId: navigationObj?.id, statusId: parseInt(newStatus) })
    if (result.data?.isSuccessful) {
      setCurrentStatus(newStatus);
      triggerGetNavigation({ navigationId: undefined });
    }
  };

  const getNavigationObjectBasedOnUrl = (navigationArray?: NavigationDto[], url?: string): NavigationDto | undefined => {
    if (navigationArray) {
      for (const item of navigationArray) {
        if (item.url == url) {
          return item;
        }
        if (item.items && item.items.length > 0) {
          const found = getNavigationObjectBasedOnUrl(item.items, url);
          if (found) {
            return found;
          }
        }
      }
    }
    return undefined;
  }

  useEffect(() => {
    const obj = getNavigationObjectBasedOnUrl(navigationList?.navigation, location.pathname.replace("/", ""));
    if (obj)
      setNavigationObj(obj);
    setCurrentStatus(obj?.statusId + "");
  }, [data, navigationList])

  return (
    <div className="flex items-center gap-2 h-10">
      {isSuccess ? <StatusDropdown navigationStatusList={data.navigationStatusList} onStatusChange={handleStatusChange} initialStatus={currentStatus} /> : <></>}
    </div>
  );
};

export default StatusDropdownActionNode; 