import { useCallback, useState } from "react";
import { useLazyDownloadAdhocProfLetter73FormLetterQuery } from "../../../../reduxstore/api/AdhocProfLetter73Api";
import { AdhocProfLetter73FilterParams } from "../AdhocProfLetter73SearchFilter";

export const useAdhocProfLetter73Print = (
  filterParams: AdhocProfLetter73FilterParams | null,
  selectedRows: Record<string, unknown>[]
) => {
  const [triggerDownload] = useLazyDownloadAdhocProfLetter73FormLetterQuery();
  
  // Print dialog state
  const [isPrintDialogOpen, setIsPrintDialogOpen] = useState(false);
  const [printContent, setPrintContent] = useState<string>("");
  const [error, setError] = useState<string | null>(null);
  const [isDownloading, setIsDownloading] = useState(false);

  const handlePrint = useCallback(async () => {
    if (!filterParams?.profitYear) {
      console.error("No profit year selected");
      return;
    }

    if (selectedRows.length === 0) {
      console.error("No rows selected");
      return;
    }

    // Clear previous errors
    setError(null);

    const profitYear = filterParams.profitYear.getFullYear();
    
    // Find badge number field (could be badgeNumber, Badge Number, Badge_Number, etc.)
    const findBadgeNumberField = (row: Record<string, unknown>): string | null => {
      // Try exact matches first
      if (typeof row.badgeNumber === 'string' && row.badgeNumber.length > 0) return row.badgeNumber;
      if (typeof row.BadgeNumber === 'string' && row.BadgeNumber.length > 0) return row.BadgeNumber;
      if (typeof row.badge_number === 'string' && row.badge_number.length > 0) return row.badge_number;
      
      // Try case-insensitive search
      const badgeKey = Object.keys(row).find(key => 
        key.toLowerCase().replace(/[_\s]/g, '') === 'badgenumber'
      );
      
      if (badgeKey && typeof row[badgeKey] === 'string' && (row[badgeKey] as string).length > 0) {
        return row[badgeKey] as string;
      }
      
      // Try numeric badge numbers
      if (typeof row.badgeNumber === 'number') return String(row.badgeNumber);
      if (typeof row.BadgeNumber === 'number') return String(row.BadgeNumber);
      
      return null;
    };
    
    const badgeNumbers = selectedRows
      .map(findBadgeNumberField)
      .filter((badge): badge is string => badge !== null && badge.length > 0);

    if (badgeNumbers.length === 0) {
      setError("No valid badge numbers found in selected rows. Please ensure the data includes badge number information.");
      return;
    }

    setIsDownloading(true);
    try {
      const result = await triggerDownload({
        profitYear: profitYear,
        badgeNumbers: badgeNumbers
      });

      if (result.error) {
        setError("Failed to generate form letter. Please try again.");
        return;
      }

      if (result.data) {
        // Convert blob to string
        const text = await (result.data as Blob).text();
        setPrintContent(text);
        setIsPrintDialogOpen(true);
      }
    } catch (err) {
      console.error("Error downloading prof letter 73 form letter:", err);
      setError("An unexpected error occurred. Please try again.");
    } finally {
      setIsDownloading(false);
    }
  }, [filterParams, selectedRows, triggerDownload]);

  const printFormLetter = useCallback((content: string) => {
    const escapeHtml = (text: string) => {
      const div = document.createElement('div');
      div.textContent = text;
      return div.innerHTML;
    };
    const printWindow = window.open("", "_blank");
    if (printWindow) {
      printWindow.document.write(`
        <html>
          <head>
            <title>Prof Letter 73</title>
            <style>
              body {
                font-family: monospace;
                font-size: 12px;
                white-space: pre-wrap;
                margin: 20px;
              }
              @media print {
                body { margin: 0; }
                @page {
                  margin: 0;
                  size: auto;
                }
              }
            </style>
          </head>
          <body>
            ${escapeHtml(content)}
          </body>
        </html>
      `);
      printWindow.document.close();
      printWindow.focus();
      printWindow.print();
      setTimeout(() => printWindow.close(), 1000);
    } else {
      setError("Failed to open print window. Please check your popup blocker settings.");
    }
  }, []);

  return {
    handlePrint,
    isDownloading,
    isPrintDialogOpen,
    setIsPrintDialogOpen,
    printContent,
    printFormLetter,
    error,
    clearError: () => setError(null)
  };
};
