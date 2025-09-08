import { useCallback } from "react";
import { ExecutiveHoursAndDollars } from "reduxstore/types";

interface UseExecutiveModalProps {
  isOpen: boolean;
  selectedExecutives: ExecutiveHoursAndDollars[];
  onClose: () => void;
  onSearch: (searchForm: any) => void;
  onSelectExecutives: (executives: ExecutiveHoursAndDollars[]) => void;
  onAddExecutives: () => void;
  isSearching: boolean;
}

const useExecutiveModal = ({
  isOpen,
  selectedExecutives,
  onClose,
  onSearch,
  onSelectExecutives,
  onAddExecutives,
  isSearching
}: UseExecutiveModalProps) => {
  
  const handleModalSearch = useCallback((searchForm: any) => {
    onSearch(searchForm);
  }, [onSearch]);

  const handleModalReset = useCallback(() => {
    onSelectExecutives([]);
  }, [onSelectExecutives]);

  const handleExecutiveSelection = useCallback((executives: ExecutiveHoursAndDollars[]) => {
    onSelectExecutives(executives);
  }, [onSelectExecutives]);

  const handleAddToMainGrid = useCallback(() => {
    if (selectedExecutives.length > 0) {
      onAddExecutives();
    }
  }, [selectedExecutives.length, onAddExecutives]);

  const canAddExecutives = selectedExecutives.length > 0;

  return {
    isModalOpen: isOpen,
    selectedExecutives,
    isModalSearching: isSearching,
    canAddExecutives,
    
    handleModalSearch,
    handleModalReset,
    handleExecutiveSelection,
    handleAddToMainGrid,
    handleModalClose: onClose
  };
};

export default useExecutiveModal;