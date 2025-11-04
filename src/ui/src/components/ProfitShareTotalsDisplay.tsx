import { Box, Table, TableBody, TableCell, TableContainer, TableRow } from "@mui/material";
import { YearEndProfitSharingReportTotalsResponse } from "reduxstore/types";
import { numberToCurrency } from "smart-ui-library";

interface ProfitShareTotalsDisplayProps {
  totalsData: YearEndProfitSharingReportTotalsResponse | null | undefined;
}

const ProfitShareTotalsDisplay = ({ totalsData }: ProfitShareTotalsDisplayProps) => {
  if (!totalsData) return null;

  const tableStyles = { mb: 4.5 };

  const headerCellStyles = {
    fontWeight: "bold",
    fontSize: "0.9rem",
    py: 1.5,
    textAlign: "right"
  };

  const firstHeaderCellStyles = {
    fontWeight: "bold",
    fontSize: "0.9rem",
    py: 1.5,
    textAlign: "left"
  };

  const labelCellStyles = {
    fontWeight: "bold",
    py: 1.5,
    fontSize: "0.9rem",
    pl: 3,
    textAlign: "left"
  };

  const dataCellStyles = {
    textAlign: "right",
    py: 1.5,
    fontSize: "0.9rem"
  };

  return (
    <Box>
      <TableContainer sx={tableStyles}>
        <Table size="small">
          <TableBody>
            <TableRow>
              <TableCell
                sx={firstHeaderCellStyles}
                width="25%"></TableCell>
              <TableCell
                sx={headerCellStyles}
                width="25%">
                Wages
              </TableCell>
              <TableCell
                sx={headerCellStyles}
                width="25%">
                Hours
              </TableCell>
              <TableCell
                sx={headerCellStyles}
                width="25%">
                Points
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell sx={labelCellStyles}>Section Total</TableCell>
              <TableCell sx={dataCellStyles}>{numberToCurrency(totalsData?.wagesTotal || 0)}</TableCell>
              <TableCell sx={dataCellStyles}>{totalsData?.hoursTotal?.toLocaleString() || "0"}</TableCell>
              <TableCell sx={dataCellStyles}>{totalsData?.pointsTotal?.toLocaleString() || "0"}</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </TableContainer>

      <TableContainer sx={tableStyles}>
        <Table size="small">
          <TableBody>
            <TableRow>
              <TableCell
                sx={firstHeaderCellStyles}
                width="25%"></TableCell>
              <TableCell
                sx={headerCellStyles}
                width="18.75%">
                All Employees
              </TableCell>
              <TableCell
                sx={headerCellStyles}
                width="18.75%">
                New Employees
              </TableCell>
              <TableCell
                sx={headerCellStyles}
                width="18.75%">
                Employees &lt; 21
              </TableCell>
              <TableCell
                sx={headerCellStyles}
                width="18.75%">
                In-Plan
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell sx={labelCellStyles}>Employee Totals</TableCell>
              <TableCell sx={dataCellStyles}>{totalsData?.numberOfEmployees?.toString() || "0"}</TableCell>
              <TableCell sx={dataCellStyles}>{totalsData?.numberOfNewEmployees?.toString() || "0"}</TableCell>
              <TableCell sx={dataCellStyles}>{totalsData?.numberOfEmployeesUnder21?.toString() || "0"}</TableCell>
              <TableCell sx={dataCellStyles}>{totalsData?.numberOfEmployeesInPlan?.toString() || "0"}</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};

export default ProfitShareTotalsDisplay;
