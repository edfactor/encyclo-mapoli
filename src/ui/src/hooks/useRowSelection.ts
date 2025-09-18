import { useCallback, useState } from "react";

export const useRowSelection = () => {
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);

  const addRowToSelection = useCallback((id: number) => {
    setSelectedRowIds(prev => [...prev, id]);
  }, []);

  const removeRowFromSelection = useCallback((id: number) => {
    setSelectedRowIds(prev => prev.filter(rowId => rowId !== id));
  }, []);

  const clearSelection = useCallback(() => {
    setSelectedRowIds([]);
  }, []);

  const isRowSelected = useCallback((id: number) => {
    return selectedRowIds.includes(id);
  }, [selectedRowIds]);

  const hasSelectedRows = selectedRowIds.length > 0;

  return {
    selectedRowIds,
    hasSelectedRows,
    addRowToSelection,
    removeRowFromSelection,
    clearSelection,
    isRowSelected,
  };
};