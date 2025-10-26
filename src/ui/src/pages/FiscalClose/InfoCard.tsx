import { ViewListOutlined } from "@mui/icons-material";
import { Box, Button, Card, Table, TableBody, TableCell, TableContainer, TableRow, Typography } from "@mui/material";
import { FC, ReactNode } from "react";
import { VIEW } from "../../constants";

export type InfoCardProps = {
  title: string;
  data: {
    [key: string]: string;
  };
  button?: ReactNode;
  handleClick?: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
  buttonDisabled?: boolean;
  valid?: boolean;
};

export const InfoCard: FC<InfoCardProps> = ({
  title,
  handleClick,
  buttonDisabled = false,
  valid = true,
  button = (
    <Button
      size="medium"
      color="secondary"
      sx={{ minWidth: "80px" }}
      variant="outlined"
      onClick={handleClick}
      disabled={buttonDisabled}
      startIcon={<ViewListOutlined color={buttonDisabled ? "disabled" : "secondary"} />}>
      {VIEW}
    </Button>
  ),
  data
}) => {
  return (
    <Card
      sx={{
        padding: "12px",
        background: valid ? "var(--grey-50, #FAFAFA)" : "#DB153215",
        borderRadius: "2px",
        height: "150px"
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
            color: "#0258A5",
            paddingRight: "8px"
          }}>
          {title}
        </Typography>
        {button}
      </Box>
      <TableContainer>
        <Table size="small">
          <TableBody>
            {Object.entries(data).map((data, index) => {
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
                      {data[0]}
                    </Typography>
                  </TableCell>
                  <TableCell style={{ width: "1%", whiteSpace: "nowrap", padding: "8px 16px 8px 0", border: "none" }}>
                    {data[1]}
                  </TableCell>
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
