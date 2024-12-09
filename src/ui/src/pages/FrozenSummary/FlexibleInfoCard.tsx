import { ViewListOutlined } from "@mui/icons-material";
import { Box, Button, Card, Table, TableBody, TableCell, TableContainer, TableRow, Typography } from "@mui/material";
import { FC, ReactNode } from "react";

export type CardValue = {
  value1: string;
  value2?: string;
  value3?: string;
};

export type FlexibleInfoCardProps = {
  title: string;
  data: {
    [key: string]: CardValue;
  };
  button?: ReactNode;
  handleClick?: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
  valid?: boolean;
  buttonDisabled?: boolean;
};

export const FlexibleInfoCard: FC<FlexibleInfoCardProps> = ({
  title,
  handleClick,
  valid = true,
  buttonDisabled = false,
  button = (
    <Button
      size="medium"
      variant="outlined"
      onClick={handleClick}
      disabled={buttonDisabled}
      startIcon={<ViewListOutlined color={buttonDisabled ? "disabled" : "primary"} />}>
      View
    </Button>
  ),
  data
}) => {
  return (
    <Card
      sx={{
        padding: "12px",
        background: valid ? "var(--grey-50, #FAFAFA)" : "#DB153215",
        borderRadius: "2px"
      }}
      variant="outlined">
      <Box
        sx={{
          display: "flex",
          flexDirection: "row",
          justifyContent: "space-between",
          alignItems: "center",
          paddingBottom: "8px"
        }}>
        <Typography
          variant="h2"
          sx={{
            color: "#0258A5"
          }}>
          {title}
        </Typography>
        {button}
      </Box>
      <TableContainer>
        <Table size="small">
          <TableBody>
            {Object.entries(data).map(([key, values], index) => {
              return (
                <TableRow key={index}>
                  <TableCell
                    style={{
                      width: "1%",
                      whiteSpace: "nowrap",
                      padding: "8px 16px 8px 0",
                      border: "none"
                    }}>
                    <Typography
                      fontWeight="bold"
                      variant="body2">
                      {key}
                    </Typography>
                  </TableCell>
                  <TableCell style={{ width: "1%", whiteSpace: "nowrap", padding: "8px 16px 8px 0", border: "none" }}>
                    {values.value1}
                  </TableCell>
                  {values.value2 !== undefined && (
                    <TableCell style={{ width: "1%", whiteSpace: "nowrap", padding: "8px 16px 8px 0", border: "none" }}>
                      {values.value2}
                    </TableCell>
                  )}
                  {values.value3 !== undefined && (
                    <TableCell style={{ width: "1%", whiteSpace: "nowrap", padding: "8px 16px 8px 0", border: "none" }}>
                      {values.value3}
                    </TableCell>
                  )}
                  <TableCell style={{ border: "0" }} />
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>
    </Card>
  );
};
