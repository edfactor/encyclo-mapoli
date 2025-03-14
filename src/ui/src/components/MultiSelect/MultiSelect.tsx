import Checkbox from "@mui/material/Checkbox";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import ListItemText from "@mui/material/ListItemText";
import MenuItem from "@mui/material/MenuItem";
import OutlinedInput from "@mui/material/OutlinedInput";
import Select, { SelectChangeEvent } from "@mui/material/Select";
import { FC, SyntheticEvent } from "react";

const ITEM_HEIGHT = 48;
const ITEM_PADDING_TOP = 8;
const MenuProps = {
  PaperProps: {
    style: {
      maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
      width: 250
    }
  }
};

type myProps = {
  label: string;
  options: string[];
  handleChange: (event: SelectChangeEvent<string[]>) => void;
  handleClose: (event: SyntheticEvent) => void;
  value: string[];
};

export const MultipleSelectCheckmarks: FC<myProps> = ({ label, options, handleChange, handleClose, value }) => {
  return (
    <div>
      <FormControl
        sx={{ m: 1, width: 220, margin: "0px", maxHeight: "35px" }}
        size="small">
        <InputLabel
          size="small"
          id="demo-multiple-checkbox-label">{`${label}`}</InputLabel>
        <Select
          sx={{ maxHeight: "35px", color: "white" }}
          labelId="demo-multiple-checkbox-label"
          id="demo-multiple-checkbox"
          multiple
          value={value}
          onChange={(e) => handleChange(e)}
          onClose={(e) => handleClose(e)}
          input={<OutlinedInput label={`${label}`} />}
          renderValue={(selected: string[]) => selected.join(", ")}
          MenuProps={MenuProps}>
          {options.map((option) => (
            <MenuItem
              key={option || ""}
              value={option || ""}>
              <Checkbox checked={value.indexOf(option) > -1} />
              <ListItemText primary={option} />
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </div>
  );
};

export default MultipleSelectCheckmarks;
