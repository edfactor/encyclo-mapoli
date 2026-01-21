import {
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  TextField
} from "@mui/material";
import { useState } from "react";
import { useCreateTaxCodeMutation } from "../../../reduxstore/api/administrationApi";
import { CreateTaxCodeRequest } from "../../../types";

interface AddTaxCodeDialogProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const AddTaxCodeDialog = ({ open, onClose, onSuccess }: AddTaxCodeDialogProps) => {
  const [createTaxCode, { isLoading }] = useCreateTaxCodeMutation();
  const [id, setId] = useState("");
  const [name, setName] = useState("");
  const [isAvailableForDistribution, setIsAvailableForDistribution] = useState(true);
  const [isAvailableForForfeiture, setIsAvailableForForfeiture] = useState(true);
  const [isProtected, setIsProtected] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSave = async () => {
    const trimmedId = id.trim().toUpperCase();
    const trimmedName = name.trim();

    if (!trimmedId) {
      setError("Id is required.");
      return;
    }

    if (trimmedId.length !== 1 || !/^[A-Z0-9]$/.test(trimmedId)) {
      setError("Id must be a single letter or number.");
      return;
    }

    if (!trimmedName) {
      setError("Name is required.");
      return;
    }

    if (trimmedName.length > 128) {
      setError("Name must be 128 characters or less.");
      return;
    }

    try {
      const request: CreateTaxCodeRequest = {
        id: trimmedId,
        name: trimmedName,
        isAvailableForDistribution,
        isAvailableForForfeiture,
        isProtected
      };

      await createTaxCode(request).unwrap();
      onSuccess();
      handleClose();
    } catch (e: unknown) {
      const apiError = e as { data?: { errors?: { Id?: string[]; Name?: string[] }; detail?: string } };
      if (apiError?.data?.errors?.Id?.length) {
        setError(apiError.data.errors.Id[0]);
      } else if (apiError?.data?.errors?.Name?.length) {
        setError(apiError.data.errors.Name[0]);
      } else if (apiError?.data?.detail) {
        setError(apiError.data.detail);
      } else {
        setError("Failed to create tax code");
      }
    }
  };

  const handleClose = () => {
    setId("");
    setName("");
    setIsAvailableForDistribution(true);
    setIsAvailableForForfeiture(true);
    setIsProtected(false);
    setError(null);
    onClose();
  };

  const handleKeyDown = (event: React.KeyboardEvent) => {
    if (event.key === "Enter" && !isLoading) {
      handleSave();
    }
  };

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      fullWidth>
      <DialogTitle>Add New Tax Code</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
          margin="dense"
          label="Id"
          fullWidth
          value={id}
          onChange={(e) => {
            setId(e.target.value.toUpperCase());
            if (error) setError(null);
          }}
          onKeyDown={handleKeyDown}
          error={!!error}
          helperText={error}
          required
          inputProps={{ maxLength: 1 }}
        />
        <TextField
          margin="dense"
          label="Name"
          fullWidth
          value={name}
          onChange={(e) => {
            setName(e.target.value);
            if (error) setError(null);
          }}
          onKeyDown={handleKeyDown}
          error={!!error}
          helperText={error}
          required
          inputProps={{ maxLength: 128 }}
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={isAvailableForDistribution}
              onChange={(e) => setIsAvailableForDistribution(e.target.checked)}
            />
          }
          label="Available for Distribution"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={isAvailableForForfeiture}
              onChange={(e) => setIsAvailableForForfeiture(e.target.checked)}
            />
          }
          label="Available for Forfeiture"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={isProtected}
              onChange={(e) => setIsProtected(e.target.checked)}
            />
          }
          label="Protected (used in business logic - cannot be edited after creation)"
        />
      </DialogContent>
      <DialogActions>
        <Button
          onClick={handleClose}
          disabled={isLoading}>
          Cancel
        </Button>
        <Button
          onClick={handleSave}
          variant="contained"
          disabled={isLoading}>
          {isLoading ? "Saving..." : "Save"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
