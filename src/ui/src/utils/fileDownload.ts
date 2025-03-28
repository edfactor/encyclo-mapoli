// src/utils/fileDownload.ts
export const downloadFileFromResponse = async (fetchPromise: Promise<{ data: Blob }>, filename: string) => {
  try {
    const result = await fetchPromise;
    const blob = result.data;
    if (!blob) throw new Error("Failed to download file");

    const url = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error("Download failed:", error);
  }
};
