import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField
} from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../../../constants";
import { encodePathParameter, isSafePath } from "../../../utils/pathValidation";

interface NewEntryDialogProps {
  open: boolean;
  onClose: () => void;
}

const NewEntryDialog = ({ open, onClose }: NewEntryDialogProps) => {
  const navigate = useNavigate();
  const [badgeNumber, setBadgeNumber] = useState("");
  const [ssn, setSSN] = useState("");
  const [memberType, setMemberType] = useState<number | "">("");
  const [errors, setErrors] = useState<{ badgeNumber?: string; ssn?: string; memberType?: string }>({});

  const handleClose = () => {
    // Reset form
    setBadgeNumber("");
    setSSN("");
    setMemberType("");
    setErrors({});
    onClose();
  };

  const validateForm = (): boolean => {
    const newErrors: { badgeNumber?: string; ssn?: string; memberType?: string } = {};

    const hasBadgeNumber = badgeNumber.trim() !== "";
    const hasSSN = ssn.trim() !== "";

    if (!hasBadgeNumber && !hasSSN) {
      newErrors.badgeNumber = "Badge number or SSN is required";
    } else if (hasBadgeNumber && !/^\d+$/.test(badgeNumber.trim())) {
      newErrors.badgeNumber = "Badge number must be numeric";
    } else if (hasSSN && !/^\d{9}$/.test(ssn.replace(/-/g, ""))) {
      newErrors.ssn = "SSN must be 9 digits";
    }

    if (memberType === "") {
      newErrors.memberType = "Member type is required";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = () => {
    if (validateForm()) {
      const badge = badgeNumber.trim();
      const cleanSSN = ssn.replace(/-/g, "");
      // Navigate to add distribution page with badge number (or SSN if no badge), and member type
      // Encode parameters to prevent injection attacks
      const identifier = encodePathParameter(badge || cleanSSN);
      const encodedMemberType = encodePathParameter(memberType.toString());
      const path = `/${ROUTES.ADD_DISTRIBUTION}/${identifier}/${encodedMemberType}`;

      // Validate constructed path before navigation
      if (isSafePath(path)) {
        navigate(path);
        handleClose();
      }
    }
  };

  const handleKeyPress = (event: React.KeyboardEvent) => {
    if (event.key === "Enter") {
      event.preventDefault();
      handleSubmit();
    }
  };

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      fullWidth>
      <DialogTitle>New Distribution Entry</DialogTitle>
      <DialogContent>
        <Stack
          spacing={2}
          sx={{ mt: 0 }}>
          <div style={{ fontSize: "0.875rem", color: "#666", marginBottom: "8px" }}>
            Please fill out either Badge Number/PSN or SSN, then Member Type to add a distribution
          </div>
          <TextField
            autoFocus
            fullWidth
            label="Badge Number or PSN"
            value={badgeNumber}
            onChange={(e) => {
              const value = e.target.value.replace(/\D/g, "").slice(0, 11);
              setBadgeNumber(value);
              if (errors.badgeNumber) {
                setErrors({ ...errors, badgeNumber: undefined });
              }
            }}
            onKeyPress={handleKeyPress}
            error={!!errors.badgeNumber}
            helperText={errors.badgeNumber}
            placeholder="Enter employee badge number or PSN"
          />
          <TextField
            fullWidth
            label="SSN"
            value={ssn}
            onChange={(e) => {
              const value = e.target.value.replace(/\D/g, "").slice(0, 9);
              setSSN(value);
              if (errors.ssn) {
                setErrors({ ...errors, ssn: undefined });
              }
            }}
            onKeyPress={handleKeyPress}
            error={!!errors.ssn}
            helperText={errors.ssn}
            placeholder="Enter 9-digit SSN"
          />
          <FormControl
            fullWidth
            error={!!errors.memberType}
            size="small">
            <InputLabel id="member-type-label">Member Type</InputLabel>
            <Select
              labelId="member-type-label"
              value={memberType}
              label="Member Type"
              size="medium"
              onChange={(e) => {
                setMemberType(e.target.value as number);
                if (errors.memberType) {
                  setErrors({ ...errors, memberType: undefined });
                }
              }}
              MenuProps={{
                PaperProps: {
                  sx: {
                    minWidth: 250
                  }
                }
              }}>
              <MenuItem value={1}>Employee</MenuItem>
              <MenuItem value={2}>Beneficiary</MenuItem>
            </Select>
            {errors.memberType && <FormHelperText>{errors.memberType}</FormHelperText>}
          </FormControl>
        </Stack>
      </DialogContent>
      <DialogActions sx={{ padding: "16px 24px" }}>
        <Button
          onClick={handleSubmit}
          variant="contained"
          color="primary"
          disabled={(badgeNumber.trim() === "" && ssn.trim() === "") || memberType === ""}>
          Continue
        </Button>
        <Button
          onClick={handleClose}
          variant="outlined">
          Cancel
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default NewEntryDialog;
