import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Stack,
    TextField
} from "@mui/material";
import { useState } from "react";
import { CreateBankRequest } from "../../../../types/administration/banks";

interface CreateBankDialogProps {
  open: boolean;
  onClose: () => void;
  onCreate: (request: CreateBankRequest) => Promise<void>;
}

const CreateBankDialog = ({ open, onClose, onCreate }: CreateBankDialogProps) => {
  const [name, setName] = useState("");
  const [officeType, setOfficeType] = useState("");
  const [city, setCity] = useState("");
  const [state, setState] = useState("");
  const [phone, setPhone] = useState("");
  const [status, setStatus] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!name.trim()) {
      return;
    }

    setIsSubmitting(true);
    try {
      await onCreate({
        name: name.trim(),
        officeType: officeType.trim() || null,
        city: city.trim() || null,
        state: state.trim().toUpperCase() || null,
        phone: phone.trim() || null,
        status: status.trim() || null
      });
      
      // Reset form
      setName("");
      setOfficeType("");
      setCity("");
      setState("");
      setPhone("");
      setStatus("");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    // Reset form on close
    setName("");
    setOfficeType("");
    setCity("");
    setState("");
    setPhone("");
    setStatus("");
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Create New Bank</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            required
            label="Bank Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            fullWidth
            inputProps={{ maxLength: 100 }}
          />
          <TextField
            label="Office Type"
            value={officeType}
            onChange={(e) => setOfficeType(e.target.value)}
            fullWidth
            inputProps={{ maxLength: 50 }}
          />
          <TextField
            label="City"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            fullWidth
            inputProps={{ maxLength: 50 }}
          />
          <TextField
            label="State"
            value={state}
            onChange={(e) => setState(e.target.value.toUpperCase())}
            fullWidth
            inputProps={{ maxLength: 2 }}
            helperText="2-letter state code"
          />
          <TextField
            label="Phone"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
            fullWidth
            inputProps={{ maxLength: 20 }}
          />
          <TextField
            label="Status"
            value={status}
            onChange={(e) => setStatus(e.target.value)}
            fullWidth
            inputProps={{ maxLength: 50 }}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          color="primary"
          disabled={!name.trim() || isSubmitting}
        >
          {isSubmitting ? "Creating..." : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default CreateBankDialog;
