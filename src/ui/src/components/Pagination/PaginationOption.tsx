import { Box, IconButton } from "@mui/material";
import FirstPageIcon from "@mui/icons-material/FirstPage";
import KeyboardArrowLeft from "@mui/icons-material/KeyboardArrowLeft";
import KeyboardArrowRight from "@mui/icons-material/KeyboardArrowRight";
import LastPageIcon from "@mui/icons-material/LastPage";
import { FC } from "react";

interface PaginationOptionActionsProps {
  count: number;
  pageNumber: number;
  pageSize: number;
  onPageChange: (event: React.MouseEvent<HTMLButtonElement>, newPage: number) => void;
}

const PaginationOption: FC<PaginationOptionActionsProps> = ({ onPageChange, pageNumber, pageSize, count }) => {
  const calculatedPages = Math.ceil(count / pageSize);
  const handleFirstPageButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    onPageChange(event, 0);
  };

  const handleBackButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    onPageChange(event, pageNumber - 1);
  };

  const handleNextButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    onPageChange(event, pageNumber + 1);
  };

  const handleLastPageButtonClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    onPageChange(event, Math.max(0, calculatedPages - 1));
  };

  return (
    <Box sx={{ flexShrink: 0, ml: 2.5 }}>
      <IconButton
        onClick={handleFirstPageButtonClick}
        disabled={pageNumber === 0}
        aria-label="first page">
        <FirstPageIcon />
      </IconButton>
      <IconButton
        onClick={handleBackButtonClick}
        disabled={pageNumber === 0}
        aria-label="previous page">
        <KeyboardArrowLeft />
      </IconButton>
      Page {pageNumber + 1} of {calculatedPages}
      <IconButton
        onClick={handleNextButtonClick}
        disabled={pageNumber >= calculatedPages - 1}
        aria-label="next page">
        <KeyboardArrowRight />
      </IconButton>
      <IconButton
        onClick={handleLastPageButtonClick}
        disabled={pageNumber >= calculatedPages - 1}
        aria-label="last page">
        <LastPageIcon />
      </IconButton>
    </Box>
  );
};

export default PaginationOption;
