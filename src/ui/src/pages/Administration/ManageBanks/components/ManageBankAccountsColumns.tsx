import { Block, Star, StarBorder } from "@mui/icons-material";
import { IconButton, Stack, Tooltip } from "@mui/material";
import { ColDef } from "ag-grid-community";
import { BankAccountDto } from "../../../../types/administration/banks";
import { createDateColumn } from "../../../../utils/gridColumnFactory";

export const GetManageBankAccountsColumns = (
  validationErrors: Record<number, { routingNumber?: string; accountNumber?: string }>,
  handleDisableAccount: (accountId: number) => void,
  handleSetPrimary: (accountId: number) => void
): ColDef[] => {
  return [
    {
      field: "id",
      headerName: "ID",
      colId: "id",
      minWidth: 80,
      editable: false,
      sortable: true,
      type: "rightAligned"
    },
    {
      field: "routingNumber",
      headerName: "Routing Number",
      colId: "routingNumber",
      minWidth: 150,
      editable: true,
      sortable: true,
      cellStyle: (params) => {
        const accountId = params.data?.id;
        const error = validationErrors[accountId]?.routingNumber;
        return error ? { backgroundColor: "#ffebee" } : undefined;
      },
      tooltipValueGetter: (params) => {
        const accountId = params.data?.id;
        const error = validationErrors[accountId]?.routingNumber;
        return error || params.value || "";
      }
    },
    {
      field: "accountNumber",
      headerName: "Account Number",
      colId: "accountNumber",
      minWidth: 150,
      editable: true,
      sortable: true,
      cellStyle: (params) => {
        const accountId = params.data?.id;
        const error = validationErrors[accountId]?.accountNumber;
        return error ? { backgroundColor: "#ffebee" } : undefined;
      },
      tooltipValueGetter: (params) => {
        const accountId = params.data?.id;
        const error = validationErrors[accountId]?.accountNumber;
        if (error) return error;
        // Otherwise show masked value in tooltip
        return params.value || "";
      },
      valueFormatter: (params) => {
        const value = params.value as string;
        if (!value) return "";
        // Mask all but last 4 digits
        return value.length > 4 ? "******" + value.slice(-4) : value;
      }
    },
    {
      field: "isPrimary",
      headerName: "Primary",
      colId: "isPrimary",
      minWidth: 100,
      editable: false,
      sortable: true,
      headerClass: "center-align",
      cellClass: "center-align",
      valueFormatter: (params) => (params.value ? "Yes" : "No")
    },
    createDateColumn({
      headerName: "Effective Date",
      field: "effectiveDate",
      minWidth: 150,
      sortable: true
    }),
    createDateColumn({
      headerName: "Discontinued",
      field: "discontinuedDate",
      minWidth: 150,
      sortable: true
    }),
    {
      field: "isDisabled",
      headerName: "Disabled",
      colId: "isDisabled",
      minWidth: 100,
      editable: false,
      sortable: true,
      headerClass: "center-align",
      cellClass: "center-align",
      valueFormatter: (params) => (params.value ? "Yes" : "No")
    },
    {
      headerName: "Actions",
      colId: "actions",
      minWidth: 120,
      editable: false,
      sortable: false,
      cellRenderer: (params: { data: BankAccountDto }) => {
        const account = params.data;
        return (
          <Stack
            direction="row"
            spacing={0.5}>
            {!account.isPrimary && !account.isDisabled && (
              <Tooltip title="Set as Primary">
                <IconButton
                  size="small"
                  color="primary"
                  onClick={() => handleSetPrimary(account.id)}>
                  <StarBorder fontSize="small" />
                </IconButton>
              </Tooltip>
            )}
            {account.isPrimary && (
              <Tooltip title="Primary Account">
                <IconButton
                  size="small"
                  disabled>
                  <Star
                    fontSize="small"
                    color="primary"
                  />
                </IconButton>
              </Tooltip>
            )}
            <Tooltip title="Disable Account">
              <span>
                <IconButton
                  size="small"
                  color="error"
                  disabled={account.isDisabled}
                  onClick={() => handleDisableAccount(account.id)}>
                  <Block fontSize="small" />
                </IconButton>
              </span>
            </Tooltip>
          </Stack>
        );
      }
    }
  ];
};
