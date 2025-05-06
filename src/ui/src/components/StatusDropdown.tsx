import { MenuItem, Select, SelectChangeEvent } from "@mui/material";
import { useState } from "react";
import { NavigationStatusDto } from "reduxstore/types";


interface StatusDropdownProps {
    initialStatus?: string;
    navigationStatusList?: NavigationStatusDto[];
    onStatusChange: (newStatus: string) => void;
}

const StatusDropdown = ({ initialStatus = "1",navigationStatusList, onStatusChange }: StatusDropdownProps) => {
    const [status, setStatus] = useState(initialStatus);

    const handleChange = (event: SelectChangeEvent<string>) => {
        const newStatus = event.target.value;
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
            {navigationStatusList?.map((value, index)=><MenuItem value={value.id}>{value.name}</MenuItem>)}
        </Select>
    );
};

export default StatusDropdown;