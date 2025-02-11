import { MenuItem, Select, SelectChangeEvent } from "@mui/material";
import { useState } from "react";

export type ProcessStatus = "Not Yet Started" | "In Progress" | "Complete";

interface StatusDropdownProps {
    initialStatus?: ProcessStatus;
    onStatusChange: (newStatus: ProcessStatus) => void;
}

const StatusDropdown = ({ initialStatus = "Not Yet Started", onStatusChange }: StatusDropdownProps) => {
    const [status, setStatus] = useState<ProcessStatus>(initialStatus);

    const handleChange = (event: SelectChangeEvent<ProcessStatus>) => {
        const newStatus = event.target.value as ProcessStatus;
        setStatus(newStatus);
        onStatusChange(newStatus);
    };

    return (
        <Select
            size="small"
            value={status}
            onChange={handleChange}
            fullWidth
        >
            <MenuItem value="Not Yet Started">Not Yet Started</MenuItem>
            <MenuItem value="In Progress">In Progress</MenuItem>
            <MenuItem value="Complete">Complete</MenuItem>
        </Select>
    );
};

export default StatusDropdown;