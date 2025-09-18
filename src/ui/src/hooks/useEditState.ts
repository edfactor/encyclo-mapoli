import { useCallback, useState } from "react";

interface EditValue {
  value: number;
  hasError: boolean;
}

export const useEditState = () => {
  const [editedValues, setEditedValues] = useState<Record<string, EditValue>>({});
  const [loadingRowIds, setLoadingRowIds] = useState<Set<number>>(new Set());

  const updateEditedValue = useCallback((rowKey: string, value: number, hasError: boolean) => {
    setEditedValues(prev => ({
      ...prev,
      [rowKey]: { value, hasError }
    }));
  }, []);

  const removeEditedValue = useCallback((rowKey: string) => {
    setEditedValues(prev => {
      const updated = { ...prev };
      delete updated[rowKey];
      return updated;
    });
  }, []);

  const clearEditedValues = useCallback((rowKeys: string[]) => {
    setEditedValues(prev => {
      const updated = { ...prev };
      rowKeys.forEach(key => delete updated[key]);
      return updated;
    });
  }, []);

  const clearAllEditedValues = useCallback(() => {
    setEditedValues({});
  }, []);

  const addLoadingRow = useCallback((rowId: number) => {
    setLoadingRowIds(prev => new Set(Array.from(prev).concat(rowId)));
  }, []);

  const removeLoadingRow = useCallback((rowId: number) => {
    setLoadingRowIds(prev => {
      const newSet = new Set(Array.from(prev));
      newSet.delete(rowId);
      return newSet;
    });
  }, []);

  const addLoadingRows = useCallback((rowIds: number[]) => {
    setLoadingRowIds(prev => {
      const newSet = new Set(Array.from(prev));
      rowIds.forEach(id => newSet.add(id));
      return newSet;
    });
  }, []);

  const removeLoadingRows = useCallback((rowIds: number[]) => {
    setLoadingRowIds(prev => {
      const newSet = new Set(Array.from(prev));
      rowIds.forEach(id => newSet.delete(id));
      return newSet;
    });
  }, []);

  const getEditedValue = useCallback((rowKey: string, fallbackValue?: number) => {
    return editedValues[rowKey]?.value ?? fallbackValue;
  }, [editedValues]);

  const hasEditedValue = useCallback((rowKey: string) => {
    return rowKey in editedValues;
  }, [editedValues]);

  const hasAnyEdits = Object.keys(editedValues).length > 0;
  const isRowLoading = useCallback((rowId: number) => loadingRowIds.has(rowId), [loadingRowIds]);

  return {
    editedValues,
    loadingRowIds,
    hasAnyEdits,
    updateEditedValue,
    removeEditedValue,
    clearEditedValues,
    clearAllEditedValues,
    getEditedValue,
    hasEditedValue,
    addLoadingRow,
    removeLoadingRow,
    addLoadingRows,
    removeLoadingRows,
    isRowLoading,
  };
};