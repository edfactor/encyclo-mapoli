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

import { useCreateCommentTypeMutation } from "../../../reduxstore/api/administrationApi";
import { CreateCommentTypeRequest } from "../../../types";

interface AddCommentTypeDialogProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const AddCommentTypeDialog = ({ open, onClose, onSuccess }: AddCommentTypeDialogProps) => {
  const [createCommentType, { isLoading }] = useCreateCommentTypeMutation();
  const [name, setName] = useState("");
  const [isProtected, setIsProtected] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSave = async () => {
    const trimmedName = name.trim();

    if (!trimmedName) {
      setError("Name is required");
      return;
    }

    try {
      const request: CreateCommentTypeRequest = {
        name: trimmedName,
        isProtected
      };
      await createCommentType(request).unwrap();
      onSuccess();
      handleClose();
    } catch (e: unknown) {
      // Handle validation errors from backend
      const error = e as { data?: { errors?: { Name?: string[] }; detail?: string } };
      if (error?.data?.errors?.Name) {
        setError(error.data.errors.Name[0]);
      } else if (error?.data?.detail) {
        setError(error.data.detail);
      } else {
        setError("Failed to create comment type");
      }
    }
  };

  const handleClose = () => {
    setName("");
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
      <DialogTitle>Add New Comment Type</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
          margin="dense"
          label="Name"
          fullWidth
          value={name}
          onChange={(e) => {
            setName(e.target.value);
            if (error) setError(null); // Clear error on typing
          }}
          onKeyDown={handleKeyDown}
          error={!!error}
          helperText={error}
          required
          inputProps={{ maxLength: 255 }}
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={isProtected}
              onChange={(e) => setIsProtected(e.target.checked)}
            />
          }
          label="Protected (used in business logic - cannot be renamed after creation)"
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
