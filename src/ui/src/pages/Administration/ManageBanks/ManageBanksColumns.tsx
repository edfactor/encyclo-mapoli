import { AccountBalance, Block } from "@mui/icons-material";
import { IconButton, Stack, Tooltip } from "@mui/material";
import { ColDef, ValueParserParams } from "ag-grid-community";
import { BankDto } from "../../../types/administration/banks";
import {
    createCityColumn,
    createPhoneColumn,
    createStateColumn,
    createYesOrNoColumn
} from "../../../utils/gridColumnFactory";

interface GetManageBanksColumnsOptions {
  handleDisableBank: (bankId: number) => void;
  setActiveTab: (tab: number, bank: { id: number; name: string } | null) => void;
}

export const GetManageBanksColumns = (
  options: GetManageBanksColumnsOptions
): ColDef[] => {
  const { handleDisableBank, setActiveTab } = options;

  return [
    {
      field: "id",
      headerName: "ID",
      width: 80,
      editable: false,
      sortable: true
    } as ColDef,
    {
      field: "name",
      headerName: "Bank Name",
      width: 200,
      editable: true,
      sortable: true
    },
    {
      field: "officeType",
      headerName: "Office Type",
      width: 150,
      editable: true,
      sortable: true
    },
    {
      ...createCityColumn({
        field: "city",
        headerName: "City",
        minWidth: 150
      }),
      editable: true
    },
    {
      ...createStateColumn({
        field: "state",
        headerName: "State",
        minWidth: 100
      }),
      editable: true,
      valueParser: (params: ValueParserParams) => {
        const value = params.newValue?.trim().toUpperCase();
        return value && value.length <= 2 ? value : params.oldValue;
      }
    },
    {
      ...createPhoneColumn({
        field: "phone",
        headerName: "Phone",
        minWidth: 150
      }),
      editable: true
    },
    {
      field: "status",
      headerName: "Status",
      width: 150,
      editable: true,
      sortable: true
    },
    {
      field: "accountCount",
      headerName: "Accounts",
      width: 100,
      editable: false,
      sortable: true
    },
    {
      ...createYesOrNoColumn({
        field: "isDisabled",
        headerName: "Disabled",
        minWidth: 100
      }),
      editable: false
    },
    {
      headerName: "Actions",
      width: 120,
      editable: false,
      cellRenderer: (params: { data: BankDto }) => {
        const bank = params.data;
        return (
          <Stack direction="row" spacing={0.5}>
            <Tooltip title="Manage Accounts">
              <IconButton
                size="small"
                color="primary"
                onClick={() => {
                  setActiveTab(1, { id: bank.id, name: bank.name });
                }}
              >
                <AccountBalance fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Disable Bank">
              <span>
                <IconButton
                  size="small"
                  color="error"
                  disabled={bank.isDisabled}
                  onClick={() => handleDisableBank(bank.id)}
                >
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
