import { useEffect, useState } from "react";
import StatusDropdown from "./StatusDropdown";
import { useGetNavigationStatusQuery, useLazyUpdateNavigationStatusQuery, useUpdateNavigationStatusQuery } from "reduxstore/api/NavigationStatusApi";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { NavigationDto } from "reduxstore/types";
import { useLazyGetNavigationQuery } from "reduxstore/api/NavigationApi";

interface StatusDropdownActionNodeProps {
  initialStatus?: string;
  navigationId?: number;
}

const StatusDropdownActionNode: React.FC<StatusDropdownActionNodeProps> = ({ initialStatus, navigationId }) => {
  const [currentStatus, setCurrentStatus] = useState("1");
  const [navigationObj, setNavigationObj] = useState<NavigationDto | null>(null);
  const { isSuccess, data } = useGetNavigationStatusQuery({});
  const navigationList = useSelector(
    (state: RootState) => state.navigation.navigationData
  );
  const currentNavigationId = useSelector(
    (state: RootState) => state.navigation.currentNavigationId
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
  }

  useEffect(() => {
    const obj = getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId??undefined);
    if (obj)
      setNavigationObj(obj);
    setCurrentStatus(obj?.statusId + "");
  }, [data, navigationList,currentNavigationId])

  return (
    <div className="flex items-center gap-2 h-10">
      {isSuccess ? <StatusDropdown navigationStatusList={data.navigationStatusList} onStatusChange={handleStatusChange} initialStatus={currentStatus} /> : <></>}
    </div>
  );
};

export default StatusDropdownActionNode; 