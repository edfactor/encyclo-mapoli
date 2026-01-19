import { ColDef, ValueFormatterParams, ValueParserParams } from "ag-grid-community";
import { mmDDYYFormat } from "../../../utils/dateUtils";
import {
  createAgeColumn,
  createCountColumn,
  createDateColumn,
  createNameColumn,
  createYearColumn
} from "../../../utils/gridColumnFactory";

const parseNumber = (params: ValueParserParams) => {
  const parsed = Number.parseFloat(String(params.newValue ?? ""));
  return Number.isFinite(parsed) ? parsed : params.oldValue;
};

const formatFourDecimals = (params: ValueFormatterParams) => {
  const value = params.value;
  return typeof value === "number" && Number.isFinite(value) ? value.toFixed(4) : "";
};

type RateColumnOptions = {
  headerName: string;
  field: string;
  minWidth?: number;
  maxWidth?: number;
  sortable?: boolean;
  editable?: boolean;
};

const createRateColumn = ({
  headerName,
  field,
  minWidth = 100,
  maxWidth,
  sortable = true,
  editable = true
}: RateColumnOptions): ColDef => {
  const column = createCountColumn({
    headerName,
    field,
    minWidth,
    maxWidth,
    alignment: "right",
    sortable
  });

  return {
    ...column,
    editable,
    filter: false,
    valueParser: parseNumber,
    valueFormatter: formatFourDecimals
  };
};

export const getManageAnnuityRatesColumns = (): ColDef[] => {
  return [
    {
      ...createYearColumn({
        headerName: "Year",
        field: "year",
        minWidth: 60,
        maxWidth: 80,
        alignment: "right"
      }),
      editable: false,
      filter: false
    },
    {
      ...createAgeColumn({
        headerName: "Age",
        field: "age",
        minWidth: 55,
        maxWidth: 80
      }),
      editable: false,
      filter: false
    },
    createRateColumn({
      headerName: "Single Rate",
      field: "singleRate",
      minWidth: 80
    }),
    createRateColumn({
      headerName: "Joint Rate",
      field: "jointRate",
      minWidth: 80
    }),
    {
      ...createNameColumn({
        headerName: "User Modified",
        field: "userModified",
        minWidth: 150
      }),
      editable: false,
      filter: false
    },
    {
      ...createDateColumn({
        headerName: "Date Modified",
        field: "dateModified",
        minWidth: 150,
        alignment: "left",
        valueFormatter: (params) => (params.value ? mmDDYYFormat(params.value) : "")
      }),
      editable: false,
      filter: false
    }
  ];
};

export const getCopyAnnuityRatesColumns = (): ColDef[] => {
  return [
    {
      ...createAgeColumn({
        headerName: "Age",
        field: "age",
        minWidth: 70
      }),
      editable: false,
      filter: false
    },
    createRateColumn({
      headerName: "Single Rate",
      field: "singleRate",
      minWidth: 120,
      sortable: false
    }),
    createRateColumn({
      headerName: "Joint Rate",
      field: "jointRate",
      minWidth: 120,
      sortable: false
    })
  ];
};
