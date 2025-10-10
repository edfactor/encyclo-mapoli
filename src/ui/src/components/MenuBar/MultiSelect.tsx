import { FormControl } from "@mui/material";
import Checkbox from "@mui/material/Checkbox";
import ListItemText from "@mui/material/ListItemText";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import { FC } from "react";

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

type MultiSelectCheckmarksProps = {
  options: string[];
  handleChange: Function;
  handleClose: () => void;
  value: string[];
  dataTestId?: string;
  ariaLabel?: string;
};

export const MultipleSelectCheckmarks: FC<MultiSelectCheckmarksProps> = ({
  options,
  handleChange,
  handleClose,
  value,
  dataTestId,
  ariaLabel
}) => {
  return (
    <div>
      <FormControl
        sx={{ m: 1, width: 220 }}
        size="small">
        <Select
          sx={{ background: "white", width: 220 }}
          multiple
          value={value}
          onChange={(e) => handleChange(e)}
          onClose={handleClose}
          renderValue={(selected: string[]) => selected.join(", ")}
          MenuProps={MenuProps}
          data-testid={dataTestId}
          inputProps={{ "aria-label": ariaLabel }}>
          {options.map((option: string, index: number) => (
            <MenuItem
              key={index}
              value={option || ""}>
              <Checkbox
                checked={value.indexOf(option) > -1}
                sx={{ width: "70px" }}
              />
              <ListItemText primary={option} />
            </MenuItem>
          ))}
        </Select>
      </FormControl>
    </div>
  );
};

export default MultipleSelectCheckmarks;
