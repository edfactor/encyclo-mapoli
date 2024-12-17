// src/utils/fileDownload.ts
export const downloadFileFromResponse = async (fetchPromise: Promise<Response>, filename: string) => {
  try {
    const response = await fetchPromise;
    if (!response.ok) throw new Error("Failed to download file");

    const blob = await response.blob();
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
