import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Grid2 from "@mui/material/Grid2";
import React from "react";

export type totalsGridProps = {
  displayData: (string | number)[][];
  leftColumnHeaders: string[];
  topRowHeaders: string[];
  makeNegativesRed?: boolean;
  tablePadding?: string;
  dataCellStyle?: React.CSSProperties;
  headerCellStyle?: React.CSSProperties;
};

export type totalsRow = {
  name: string;
  data: (string | number)[];
};

export const TotalsGrid: React.FC<totalsGridProps> = ({
  displayData,
  leftColumnHeaders,
  topRowHeaders,
  makeNegativesRed = true,
  tablePadding,
  dataCellStyle,
  headerCellStyle
}) => {
  const rows: totalsRow[] = displayData.map((row, index) => {
    return { name: leftColumnHeaders[index], data: row };
  });
  const dataCellStyleDefault: React.CSSProperties = {
    height: "32px",
    padding: "0px 12px 0px 18px",
    opacity: "0px",
    borderColor: "#DDE2EB",
    fontWeight: "400",
    fontSize: "14px",
    lineHeight: "143%",
    textAlign: "left"
  };
  const negativeDataCellStyle: React.CSSProperties = {
    ...dataCellStyleDefault,
    color: "#E53935"
  };

  const appliedHeaderCellStyle: React.CSSProperties = headerCellStyle ?? {
    ...dataCellStyleDefault,
    fontWeight: "700"
  };

  return (
    <Grid2
      size={{ xs: 12, md: 12, lg: 12, xl: 12 }}
      padding={tablePadding ?? "24px"}>
      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow>
              {topRowHeaders.map((header, index) => {
                return (
                  <TableCell
                    key={index}
                    sx={appliedHeaderCellStyle}>
                    {header}
                  </TableCell>
                );
              })}
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow
                key={row.name || "tempKey"}
                sx={{
                  "&:last-child td, &:last-child th": { border: 0 }
                }}>
                {leftColumnHeaders.length != 0 && leftColumnHeaders[0] != "" && (
                  <TableCell
                    component="th"
                    scope="row"
                    size="small"
                    sx={appliedHeaderCellStyle}>
                    {row.name}
                  </TableCell>
                )}
                {row.data.map((value, index) => {
                  let appliedDataCellStyle = dataCellStyle ?? dataCellStyleDefault;
                  if (
                    makeNegativesRed &&
                    ((typeof value === "string" && value.charAt(0) === "-") || (typeof value === "number" && value < 0))
                  ) {
                    value = typeof value === "string" ? `(${value.slice(1)})` : `(${Math.abs(value)})`;
                    appliedDataCellStyle = negativeDataCellStyle;
                  }
                  return (
                    <TableCell
                      key={index}
                      sx={appliedDataCellStyle}>
                      {value}
                    </TableCell>
                  );
                })}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Grid2>
  );
};

export default TotalsGrid;
