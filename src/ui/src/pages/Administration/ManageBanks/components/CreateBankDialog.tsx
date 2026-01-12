import { yupResolver } from "@hookform/resolvers/yup";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField
} from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import * as yup from "yup";
import { CreateBankRequest } from "../../../../types/administration/banks";

interface CreateBankDialogProps {
  open: boolean;
  onClose: () => void;
  onCreate: (request: CreateBankRequest) => Promise<void>;
}

interface CreateBankFormData {
  name: string;
  officeType?: string | null;
  city?: string | null;
  state?: string | null;
  phone?: string | null;
  status?: string | null;
}

const validationSchema = yup.object().shape({
  name: yup
    .string()
    .required("Bank name is required")
    .trim()
    .max(100, "Bank name must not exceed 100 characters"),
  officeType: yup
    .string()
    .trim()
    .max(50, "Office type must not exceed 50 characters")
    .nullable(),
  city: yup
    .string()
    .trim()
    .max(50, "City must not exceed 50 characters")
    .nullable(),
  state: yup
    .string()
    .trim()
    .uppercase()
    .matches(/^[A-Z]{0,2}$/, "State must be a 2-letter code")
    .max(2, "State must be a 2-letter code")
    .nullable(),
  phone: yup
    .string()
    .trim()
    .max(20, "Phone must not exceed 20 characters")
    .nullable(),
  status: yup
    .string()
    .trim()
    .max(50, "Status must not exceed 50 characters")
    .nullable()
});

const CreateBankDialog = ({ open, onClose, onCreate }: CreateBankDialogProps) => {
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting }
  } = useForm<CreateBankFormData>({
    resolver: yupResolver(validationSchema),
    defaultValues: {
      name: "",
      officeType: "",
      city: "",
      state: "",
      phone: "",
      status: ""
    }
  });

  const onSubmit = async (data: CreateBankFormData) => {
    await onCreate({
      name: data.name.trim(),
      officeType: data.officeType?.trim() || null,
      city: data.city?.trim() || null,
      state: data.state?.trim().toUpperCase() || null,
      phone: data.phone?.trim() || null,
      status: data.status?.trim() || null
    });
    
    reset();
  };

  const handleClose = () => {
    reset();
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Create New Bank</DialogTitle>
      <DialogContent>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Controller
              name="name"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  required
                  label="Bank Name"
                  fullWidth
                  error={!!errors.name}
                  helperText={errors.name?.message}
                  inputProps={{ maxLength: 100 }}
                />
              )}
            />
            <Controller
              name="officeType"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Office Type"
                  fullWidth
                  error={!!errors.officeType}
                  helperText={errors.officeType?.message}
                  inputProps={{ maxLength: 50 }}
                />
              )}
            />
            <Controller
              name="city"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="City"
                  fullWidth
                  error={!!errors.city}
                  helperText={errors.city?.message}
                  inputProps={{ maxLength: 50 }}
                />
              )}
            />
            <Controller
              name="state"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="State"
                  fullWidth
                  error={!!errors.state}
                  helperText={errors.state?.message || "2-letter state code"}
                  inputProps={{ maxLength: 2 }}
                  onChange={(e) => field.onChange(e.target.value.toUpperCase())}
                />
              )}
            />
            <Controller
              name="phone"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Phone"
                  fullWidth
                  error={!!errors.phone}
                  helperText={errors.phone?.message}
                  inputProps={{ maxLength: 20 }}
                />
              )}
            />
            <Controller
              name="status"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Status"
                  fullWidth
                  error={!!errors.status}
                  helperText={errors.status?.message}
                  inputProps={{ maxLength: 50 }}
                />
              )}
            />
          </Stack>
        </form>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={isSubmitting}>
          Cancel
        </Button>
        <Button
          onClick={handleSubmit(onSubmit)}
          variant="contained"
          color="primary"
          disabled={isSubmitting}
        >
          {isSubmitting ? "Creating..." : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default CreateBankDialog;
