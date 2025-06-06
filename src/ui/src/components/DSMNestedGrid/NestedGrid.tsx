import React from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Box,
  Typography
} from '@mui/material';
import { NestedGridRow, INestedGridRowData, INestedGridColumn } from './NestedGridRow';

export interface INestedGridProps {
  data: INestedGridRowData[];
  columns: INestedGridColumn[];
  renderNestedContent: (row: INestedGridRowData, isExpanded: boolean) => React.ReactNode;
  title?: string;
  className?: string;
  onRowExpand?: (row: INestedGridRowData, isExpanded: boolean) => void;
  showTitle?: boolean;
  expandedBackgroundColor?: string;
}

export const NestedGrid: React.FC<INestedGridProps> = ({ 
  data, 
  columns,
  renderNestedContent,
  title,
  className = "",
  onRowExpand,
  showTitle = true,
  expandedBackgroundColor = 'white'
}) => {
  return (
    <Box className={className} sx={{ width: '100%' }}>
      {showTitle && title && (
        <Box sx={{ px: 3, py: 2 }}>
          <Typography
            variant="h2"
            sx={{ 
              color: '#0258A5',
              fontWeight: 'bold'
            }}>
            {title}
          </Typography>
        </Box>
      )}

      <TableContainer sx={{ width: '100%' }}>
        <Table aria-label="nested data table" sx={{ width: '100%' }}>
          <TableHead>
            <TableRow sx={{ backgroundColor: 'rgba(2, 88, 165, 0.1)' }}>
              {columns.map((column) => (
                <TableCell 
                  key={column.key}
                  align={column.align || 'left'}
                  sx={{ 
                    fontWeight: 600, 
                    color: '#0258A5',
                    py: 2,
                    width: column.width
                  }}>
                  {column.label}
                </TableCell>
              ))}
              <TableCell sx={{ width: 48 }} />
            </TableRow>
          </TableHead>
          <TableBody>
            {data.map((row) => (
              <NestedGridRow 
                key={row.id} 
                row={row}
                columns={columns}
                renderNestedContent={renderNestedContent}
                onRowExpand={onRowExpand}
                expandedBackgroundColor={expandedBackgroundColor}
              />
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};

export default NestedGrid; 