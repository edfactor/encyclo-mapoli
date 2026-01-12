import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField } from "@mui/material";
import { useState } from "react";
import { CreateBankAccountRequest } from "../../../../types/administration/banks";

interface CreateBankAccountDialogProps {
    open: boolean;
    onClose: () => void;
    onCreate: (request: CreateBankAccountRequest) => Promise<void>;
}

const CreateBankAccountDialog = ({ open, onClose, onCreate }: CreateBankAccountDialogProps) => {
    const [routingNumber, setRoutingNumber] = useState("");
    const [accountNumber, setAccountNumber] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async () => {
        setIsSubmitting(true);
        try {
            await onCreate({
                routingNumber,
                accountNumber
            });
            handleClose();
        } catch (error) {
            console.error("Error creating bank account:", error);
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleClose = () => {
        setRoutingNumber("");
        setAccountNumber("");
        setIsSubmitting(false);
        onClose();
    };

    const isValid = routingNumber.trim() !== "" && accountNumber.trim() !== "";

    return (
        <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
            <DialogTitle>Add Bank Account</DialogTitle>
            <DialogContent>
                <Stack spacing={2} sx={{ mt: 1 }}>
                    <TextField
                        label="Routing Number"
                        value={routingNumber}
                        onChange={(e) => setRoutingNumber(e.target.value)}
                        fullWidth
                        required
                        inputProps={{ maxLength: 9 }}
                    />
                    <TextField
                        label="Account Number"
                        value={accountNumber}
                        onChange={(e) => setAccountNumber(e.target.value)}
                        fullWidth
                        required
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
                    disabled={!isValid || isSubmitting}
                >
                    {isSubmitting ? "Creating..." : "Create"}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default CreateBankAccountDialog;
