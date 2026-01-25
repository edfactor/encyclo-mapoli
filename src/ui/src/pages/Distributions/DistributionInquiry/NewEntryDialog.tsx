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
import { useCallback, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "../../../constants";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import {
  ARIA_DESCRIPTIONS,
  formatSSNInput,
  getBadgeOrPSNPlaceholder,
  INPUT_PLACEHOLDERS
} from "../../../utils/inputFormatters";
import { encodePathParameter, isSafePath } from "../../../utils/pathValidation";

interface NewEntryDialogProps {
  open: boolean;
  onClose: () => void;
}

const NewEntryDialog = ({ open, onClose }: NewEntryDialogProps) => {
  const navigate = useNavigate();
  const [badgeNumber, setBadgeNumber] = useState("");
  const [ssn, setSSN] = useState("");
  const [memberType, setMemberType] = useState<number | "">(1);
  const [errors, setErrors] = useState<{ badgeNumber?: string; ssn?: string; memberType?: string }>({});
  const [badgePlaceholder, setBadgePlaceholder] = useState(INPUT_PLACEHOLDERS.BADGE_OR_PSN);

  const handleClose = () => {
    // Reset form
    setBadgeNumber("");
    setSSN("");
    setMemberType("");
    setErrors({});
    setBadgePlaceholder(INPUT_PLACEHOLDERS.BADGE_OR_PSN);
    onClose();
  };

  const handleSSNChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const formatted = formatSSNInput(e.target.value);
      setSSN(formatted.display);
      if (errors.ssn) {
        setErrors({ ...errors, ssn: undefined });
      }
    },
    [errors]
  );

  const handleBadgeChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const value = e.target.value.replace(/\D/g, "").slice(0, 11);
      setBadgeNumber(value);
      setBadgePlaceholder(getBadgeOrPSNPlaceholder(value.length));
      if (errors.badgeNumber) {
        setErrors({ ...errors, badgeNumber: undefined });
      }
    },
    [errors]
  );

  const validateForm = (): boolean => {
    const newErrors: { badgeNumber?: string; ssn?: string; memberType?: string } = {};

    const hasBadgeNumber = badgeNumber.trim() !== "";
    const cleanSSN = ssn.replace(/-/g, "");
    const hasSSN = cleanSSN !== "";

    if (!hasBadgeNumber && !hasSSN) {
      newErrors.badgeNumber = "Badge number or SSN is required";
    } else if (hasBadgeNumber && !/^\d+$/.test(badgeNumber.trim())) {
      newErrors.badgeNumber = "Badge number must be numeric";
    } else if (hasSSN && !/^\d{9}$/.test(cleanSSN)) {
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
            id={generateFieldId("badgeNumber")}
            label="Badge Number or PSN"
            value={badgeNumber}
            placeholder={badgePlaceholder}
            inputMode="numeric"
            onChange={handleBadgeChange}
            onKeyPress={handleKeyPress}
            error={!!errors.badgeNumber}
            aria-invalid={!!errors.badgeNumber}
            aria-describedby={getAriaDescribedBy("badgeNumber", !!errors.badgeNumber, true)}
            helperText={errors.badgeNumber}
          />
          <VisuallyHidden id={generateFieldId("badgeNumber-hint")}>{ARIA_DESCRIPTIONS.BADGE_DYNAMIC}</VisuallyHidden>
          <TextField
            fullWidth
            id={generateFieldId("ssn")}
            label="SSN"
            value={ssn}
            placeholder={INPUT_PLACEHOLDERS.SSN}
            inputMode="numeric"
            onChange={handleSSNChange}
            onKeyPress={handleKeyPress}
            error={!!errors.ssn}
            aria-invalid={!!errors.ssn}
            aria-describedby={getAriaDescribedBy("ssn", !!errors.ssn, true)}
            helperText={errors.ssn}
          />
          <VisuallyHidden id={generateFieldId("ssn-hint")}>{ARIA_DESCRIPTIONS.SSN_FORMAT}</VisuallyHidden>
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
