import Paper from "@mui/material/Paper";
import Table from "@mui/material/Table";
import TableContainer from "@mui/material/TableContainer";
import TableFooter from "@mui/material/TableFooter";
import TablePagination from "@mui/material/TablePagination";
import TableRow from "@mui/material/TableRow";
import * as React from "react";
import { FC } from "react";
import { ICommon } from "../ICommon";
import PaginationOption from "./PaginationOption";

export interface IPaginationProps extends ICommon {
  recordCount: number;
  pageNumber: number;
  setPageNumber: (page: number) => void;
  pageSize: number;
  setPageSize: (size: number) => void;
  rowsPerPageOptions?: number[];
}

export const Pagination: FC<IPaginationProps> = ({
  recordCount,
  pageNumber,
  setPageNumber,
  pageSize,
  setPageSize,
  rowsPerPageOptions,
  ...props
}) => {
  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    setPageNumber(newPage + 1);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setPageSize(parseInt(event.target.value));
    setPageNumber(1);
  };

  return (
    <TableContainer
      component={Paper}
      {...props}>
      <Table
        sx={{ minWidth: 500 }}
        aria-label="custom pagination table">
        <TableFooter>
          <TableRow>
            <TablePagination
              rowsPerPageOptions={rowsPerPageOptions || [10, 25, 50, 100, 150, 200]}
              colSpan={3}
              count={recordCount}
              rowsPerPage={pageSize}
              page={pageNumber}
              SelectProps={{
                inputProps: {
                  "aria-label": "rows per page"
                },
                native: true
              }}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
              ActionsComponent={() => (
                <PaginationOption
                  pageNumber={pageNumber}
                  pageSize={pageSize}
                  count={recordCount}
                  onPageChange={handleChangePage}
                />
              )}
            />
          </TableRow>
        </TableFooter>
      </Table>
    </TableContainer>
  );
};
export default Pagination;
