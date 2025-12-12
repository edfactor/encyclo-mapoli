import { describe, expect, it } from "vitest";
import { AdhocProfLetter73Api } from "../../../../reduxstore/api/AdhocProfLetter73Api";

describe("AdhocProfLetter73Api", () => {
  it("should have the correct API endpoints defined", () => {
    expect(AdhocProfLetter73Api.endpoints.getAdhocProfLetter73).toBeDefined();
    expect(AdhocProfLetter73Api.endpoints.downloadAdhocProfLetter73FormLetter).toBeDefined();
  });

  it("should have the correct reducer path", () => {
    expect(AdhocProfLetter73Api.reducerPath).toBe("adhocProfLetter73Api");
  });

  it("should configure getAdhocProfLetter73 endpoint correctly", () => {
    const endpoint = AdhocProfLetter73Api.endpoints.getAdhocProfLetter73;
    expect(endpoint).toBeDefined();
    expect(endpoint.name).toBe("getAdhocProfLetter73");
  });

  it("should configure downloadAdhocProfLetter73FormLetter endpoint correctly", () => {
    const endpoint = AdhocProfLetter73Api.endpoints.downloadAdhocProfLetter73FormLetter;
    expect(endpoint).toBeDefined();
    expect(endpoint.name).toBe("downloadAdhocProfLetter73FormLetter");
  });
});
