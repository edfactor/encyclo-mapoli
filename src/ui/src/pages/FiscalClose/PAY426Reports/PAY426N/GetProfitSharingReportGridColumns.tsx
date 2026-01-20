import InfoOutlinedIcon from "@mui/icons-material/InfoOutlined";
import { Tooltip } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import {
  CrossReferenceValidationGroup,
  ValidationResponse
} from "../../../../types/validation/cross-reference-validation";
import {
  createAgeColumn,
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createPointsColumn,
  createSSNColumn,
  createStatusColumn,
  createStoreColumn,
  createYesOrNoColumn
} from "../../../../utils/gridColumnFactory";

/**
 * Helper function to get validation result for a specific group
 */
const getValidationForGroup = (
  validationData: ValidationResponse | null,
  groupName: string,
  checkValue: number
): { isValid: boolean; expectedValue: string; narrative: string } | null => {
  if (!validationData?.validationGroups) return null;

  const group = validationData.validationGroups.find((g: CrossReferenceValidationGroup) => g.groupName === groupName);

  if (!group || !group.validations || group.validations.length === 0 || group.validations[0].expectedValue === null)
    return {
      isValid: false,
      expectedValue: "??",
      narrative: "Profit Sharing Summary hasn't been completed for this year."
    };

  return {
    isValid: group.validations[0].expectedValue == checkValue,
    expectedValue: group.validations[0].expectedValue + "",
    narrative:
      group.validations[0].expectedValue == checkValue
        ? "Value matches Profit Sharing Summary."
        : "Profit Sharing Summary value is " + group.validations[0].expectedValue + "."
  };
};

/**
 * Cell renderer for currency values with validation icon in pinned rows
 */
const createValidatedCurrencyCellRenderer = (
  validationData: ValidationResponse | null,
  validationGroupName: string
) => {
  return (params: ICellRendererParams) => {
    const value = params.value ?? 0;
    const formattedValue = numberToCurrency(value);

    // Only show validation icon for pinned total row
    if (params.data?._isPinnedTotal && validationData) {
      const validation = getValidationForGroup(validationData, validationGroupName, value);

      if (validation) {
        return (
          <span style={{ display: "flex", alignItems: "center", justifyContent: "flex-end", gap: "4px" }}>
            <Tooltip title={validation.narrative}>
              <InfoOutlinedIcon
                sx={{
                  fontSize: 16,
                  color: validation.isValid ? "#22c55e" : "#ef4444"
                }}
              />
            </Tooltip>
            <span>{formattedValue}</span>
          </span>
        );
      }
    }

    return formattedValue;
  };
};

/**
 * Cell renderer for the fullName column that shows employee count validation
 */
const createValidatedNameCellRenderer = (validationData: ValidationResponse | null) => {
  return (params: ICellRendererParams) => {
    const rawValue = params.value ?? "";
    // Strip "TOTAL EMPS: " prefix if present and trim
    const value = rawValue.replace(/^TOTAL EMPS:\s*/i, "").trim();

    // Only show validation icon for pinned total row
    if (params.data?._isPinnedTotal && validationData) {
      const validation = getValidationForGroup(validationData, "Members", value);

      if (validation) {
        return (
          <span style={{ display: "flex", alignItems: "center", gap: "4px" }}>
            <Tooltip title={validation.narrative}>
              <InfoOutlinedIcon
                sx={{
                  fontSize: 16,
                  color: validation.isValid ? "#22c55e" : "#ef4444"
                }}
              />
            </Tooltip>
            <span>{rawValue}</span>
          </span>
        );
      }
    }

    return value;
  };
};

export const GetProfitSharingReportGridColumns = (validationData: ValidationResponse | null = null): ColDef[] => {
  return [
    createBadgeColumn({}),
    {
      ...createNameColumn({}),
      cellRenderer: createValidatedNameCellRenderer(validationData)
    },
    createStoreColumn({}),
    {
      headerName: "Type",
      field: "employeeTypeCode",
      colId: "employeeTypeCode",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    createDateColumn({
      headerName: "Date of Birth",
      field: "dateOfBirth"
    }),
    createAgeColumn({}),
    createSSNColumn(),
    {
      ...createCurrencyColumn({
        headerName: "Wages",
        field: "wages"
      }),
      cellRenderer: createValidatedCurrencyCellRenderer(validationData, "Wages")
    },
    createHoursColumn({}),
    createPointsColumn({}),
    createYesOrNoColumn({
      headerName: "New",
      field: "isNew",
      colId: "isNew"
    }),
    createStatusColumn({
      field: "employeeStatus",
      valueFormatter: (params) => {
        const value = typeof params.value === "string" ? params.value.toLowerCase() : params.value;
        if (value === "a") return "Active";
        if (value === "i") return "Inactive";
        if (value === "t") return "Terminated";
        return params.value;
      }
    }),
    {
      ...createCurrencyColumn({
        headerName: "Balance",
        field: "balance"
      }),
      cellRenderer: createValidatedCurrencyCellRenderer(validationData, "Balance")
    },
    createCountColumn({
      headerName: "Years in Plan",
      field: "yearsInPlan"
    }),
    createDateColumn({
      headerName: "Inactive date",
      field: "terminationDate"
    })
  ];
};
