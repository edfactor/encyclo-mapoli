import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createCurrencyColumn, createNameColumn } from "utils/gridColumnFactory";

export const ProfitShareUpdateGridColumns = (): ColDef[] => {
  return [
    createBadgeColumn({ field: "badge" }),
    // According to the back end, in MemberFinancials.cs, the PSN is currently a string
    // version of the badge, so I am removing it for now. - EJL
    // createBadgeColumn({ headerName: "PSN", field: "psn" }),
    createNameColumn({ field: "name" }),
    createCurrencyColumn({
      headerName: "Beginning Balance",
      field: "beginningAmount"
    }),
    createCurrencyColumn({
      headerName: "Contributions",
      field: "contributions"
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "allEarnings"
    }),
    createCurrencyColumn({
      headerName: "Earnings2",
      field: "allSecondaryEarnings"
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "incomingForfeitures"
    }),
    createCurrencyColumn({
      headerName: "Distributions",
      field: "distributions"
    }),
    createCurrencyColumn({
      headerName: "Military/Paid Allocation",
      field: "pxfer"
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance"
    })
  ];
};
