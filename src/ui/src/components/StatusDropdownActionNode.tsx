import { useEffect } from "react";
import StatusDropdown from "./StatusDropdown";
import { useGetNavigationStatusQuery } from "reduxstore/api/NavigationStatusApi";

interface StatusDropdownActionNodeProps {
  initialStatus?: string;
}

const StatusDropdownActionNode: React.FC<StatusDropdownActionNodeProps> = ({ initialStatus }) => {

  const { isSuccess, data}  = useGetNavigationStatusQuery({});
  const handleStatusChange = async (newStatus: string) => {
    console.info("Logging new status: ", newStatus);
  };
  if(isSuccess){
    console.log(data);
  }



  return (
    <div className="flex items-center gap-2 h-10">
      {isSuccess?<StatusDropdown navigationStatusList={data.navigationStatusList} onStatusChange={handleStatusChange} initialStatus={initialStatus} />:<></>}
    </div>
  );
};

export default StatusDropdownActionNode; 