import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField } from "@mui/material";
import { useState } from "react";
import { CreateBankAccountRequest } from "../../../../types/administration/banks";
import {
    handleAccountNumberInput,
    handleRoutingNumberInput,
    validateAccountNumber,
    validateRoutingNumber
} from "../../../../utils/bankValidation";

interface CreateBankAccountDialogProps {
    open: boolean;
    onClose: () => void;
    onCreate: (request: CreateBankAccountRequest) => Promise<void>;
    bankId: number;
}

const CreateBankAccountDialog = ({ open, onClose, onCreate, bankId }: CreateBankAccountDialogProps) => {
    const [routingNumber, setRoutingNumber] = useState("");
    const [accountNumber, setAccountNumber] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [touched, setTouched] = useState({
        routingNumber: false,
        accountNumber: false
    });

    // Validate fields
    const routingNumberError = validateRoutingNumber(routingNumber);
    const accountNumberError = validateAccountNumber(accountNumber);
    const isValid = !routingNumberError && !accountNumberError;

    const handleSubmit = async () => {
        // Mark all fields as touched to show validation errors
        setTouched({
            routingNumber: true,
            accountNumber: true
        });

        // Only submit if valid
        if (!isValid) {
            return;
        }

        setIsSubmitting(true);
        try {
            await onCreate({
                bankId,
                routingNumber,
                accountNumber,
                isPrimary: false,
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
        setTouched({
            routingNumber: false,
            accountNumber: false
        });
        onClose();
    };

    const handleRoutingNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = handleRoutingNumberInput(e.target.value);
        setRoutingNumber(value);
    };

    const handleAccountNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = handleAccountNumberInput(e.target.value);
        setAccountNumber(value);
    };

    return (
        <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
            <DialogTitle>Add Bank Account</DialogTitle>
            <DialogContent>
                <Stack spacing={2} sx={{ mt: 1 }}>
                    <TextField
                        label="Routing Number"
                        value={routingNumber}
                        onChange={handleRoutingNumberChange}
                        onBlur={() => setTouched(prev => ({ ...prev, routingNumber: true }))}
                        fullWidth
                        required
                        error={touched.routingNumber && !!routingNumberError}
                        helperText={touched.routingNumber && routingNumberError ? routingNumberError : "Enter 9-digit routing number"}
                        inputProps={{ 
                            maxLength: 9,
                            inputMode: "numeric" as const
                        }}
                    />
                    <TextField
                        label="Account Number"
                        value={accountNumber}
                        onChange={handleAccountNumberChange}
                        onBlur={() => setTouched(prev => ({ ...prev, accountNumber: true }))}
                        fullWidth
                        required
                        error={touched.accountNumber && !!accountNumberError}
                        helperText={touched.accountNumber && accountNumberError ? accountNumberError : "Max 34 characters"}
                        inputProps={{ maxLength: 34 }}
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
