import StatusDropdown, { ProcessStatus } from "./StatusDropdown";

interface StatusDropdownActionNodeProps {
  initialStatus?: ProcessStatus;
}

const StatusDropdownActionNode: React.FC<StatusDropdownActionNodeProps> = ({ initialStatus }) => {
  const handleStatusChange = async (newStatus: ProcessStatus) => {
    console.info("Logging new status: ", newStatus);
  };

  return (
    <div className="flex items-center gap-2 h-10">
      <StatusDropdown onStatusChange={handleStatusChange} initialStatus={initialStatus} />
    </div>
  );
};

export default StatusDropdownActionNode; 