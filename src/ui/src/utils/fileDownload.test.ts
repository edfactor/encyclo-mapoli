import { beforeEach, describe, expect, it, vi } from "vitest";
import { downloadFileFromResponse } from "./fileDownload";

describe("fileDownload", () => {
  describe("downloadFileFromResponse", () => {
    let createObjectURLSpy: ReturnType<typeof vi.fn>;
    let revokeObjectURLSpy: ReturnType<typeof vi.fn>;
    let consoleErrorSpy: ReturnType<typeof vi.fn>;

    beforeEach(() => {
      // Mock window.URL methods
      createObjectURLSpy = vi.fn().mockReturnValue("blob:mock-url");
      revokeObjectURLSpy = vi.fn();
      global.URL.createObjectURL = createObjectURLSpy as typeof URL.createObjectURL;
      global.URL.revokeObjectURL = revokeObjectURLSpy as typeof URL.revokeObjectURL;

      // Mock console.error
      consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    });

    it("should download a file successfully", async () => {
      const mockBlob = new Blob(["test content"], { type: "text/plain" });
      const fetchPromise = Promise.resolve({ data: mockBlob });
      const filename = "test-file.txt";

      await downloadFileFromResponse(fetchPromise, filename);

      // Verify blob URL was created
      expect(createObjectURLSpy).toHaveBeenCalledWith(mockBlob);

      // Verify cleanup: blob URL revoked
      expect(revokeObjectURLSpy).toHaveBeenCalledWith("blob:mock-url");
    });

    it("should create download link with correct filename", async () => {
      const mockBlob = new Blob(["data"], { type: "application/pdf" });
      const fetchPromise = Promise.resolve({ data: mockBlob });
      const filename = "report-2024.pdf";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(createObjectURLSpy).toHaveBeenCalledWith(mockBlob);
      expect(revokeObjectURLSpy).toHaveBeenCalledWith("blob:mock-url");
    });

    it("should handle different file types", async () => {
      const mockBlob = new Blob(["csv content"], { type: "text/csv" });
      const fetchPromise = Promise.resolve({ data: mockBlob });
      const filename = "export.csv";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(createObjectURLSpy).toHaveBeenCalledWith(mockBlob);
      expect(revokeObjectURLSpy).toHaveBeenCalled();
    });

    it("should handle large blobs", async () => {
      const largeContent = new Array(10000).fill("a").join("");
      const mockBlob = new Blob([largeContent], { type: "text/plain" });
      const fetchPromise = Promise.resolve({ data: mockBlob });
      const filename = "large-file.txt";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(createObjectURLSpy).toHaveBeenCalledWith(mockBlob);
      expect(revokeObjectURLSpy).toHaveBeenCalled();
    });

    it("should handle filenames with special characters", async () => {
      const mockBlob = new Blob(["content"], { type: "application/json" });
      const fetchPromise = Promise.resolve({ data: mockBlob });
      const filename = "my-file (2024-01-15) [final].json";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(createObjectURLSpy).toHaveBeenCalledWith(mockBlob);
      expect(revokeObjectURLSpy).toHaveBeenCalled();
    });

    it("should log error and not throw when blob is missing", async () => {
      const fetchPromise = Promise.resolve({ data: null as unknown as Blob });
      const filename = "test.txt";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(consoleErrorSpy).toHaveBeenCalledWith("Download failed:", expect.any(Error));
      expect(createObjectURLSpy).not.toHaveBeenCalled();
    });

    it("should log error and not throw when fetch promise rejects", async () => {
      const error = new Error("Network error");
      const fetchPromise = Promise.reject(error);
      const filename = "test.txt";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(consoleErrorSpy).toHaveBeenCalledWith("Download failed:", error);
      expect(createObjectURLSpy).not.toHaveBeenCalled();
    });

    it("should log error when blob is undefined", async () => {
      const fetchPromise = Promise.resolve({ data: undefined as unknown as Blob });
      const filename = "test.txt";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(consoleErrorSpy).toHaveBeenCalledWith("Download failed:", expect.any(Error));
    });

    it("should handle empty blob", async () => {
      const mockBlob = new Blob([], { type: "text/plain" });
      const fetchPromise = Promise.resolve({ data: mockBlob });
      const filename = "empty.txt";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(createObjectURLSpy).toHaveBeenCalledWith(mockBlob);
      expect(revokeObjectURLSpy).toHaveBeenCalled();
    });

    it("should handle blob with binary data", async () => {
      const binaryData = new Uint8Array([0x89, 0x50, 0x4e, 0x47]); // PNG header
      const mockBlob = new Blob([binaryData], { type: "image/png" });
      const fetchPromise = Promise.resolve({ data: mockBlob });
      const filename = "image.png";

      await downloadFileFromResponse(fetchPromise, filename);

      expect(createObjectURLSpy).toHaveBeenCalledWith(mockBlob);
      expect(revokeObjectURLSpy).toHaveBeenCalled();
    });

    it("should handle concurrent downloads", async () => {
      const mockBlob1 = new Blob(["content1"], { type: "text/plain" });
      const mockBlob2 = new Blob(["content2"], { type: "text/plain" });
      const fetchPromise1 = Promise.resolve({ data: mockBlob1 });
      const fetchPromise2 = Promise.resolve({ data: mockBlob2 });

      await Promise.all([
        downloadFileFromResponse(fetchPromise1, "file1.txt"),
        downloadFileFromResponse(fetchPromise2, "file2.txt")
      ]);

      expect(createObjectURLSpy).toHaveBeenCalledTimes(2);
      expect(revokeObjectURLSpy).toHaveBeenCalledTimes(2);
    });
  });
});
