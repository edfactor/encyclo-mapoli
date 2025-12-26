import { describe, expect, it } from "vitest";
import EnvironmentUtils from "./environmentUtils";

describe("EnvironmentUtils", () => {
  describe("isDevelopment", () => {
    it("should return true when environment is development", () => {
      // Test based on the actual current environment
      const actualEnv = import.meta.env.VITE_APP_ENVIRONMENT;
      if (actualEnv === "development") {
        expect(EnvironmentUtils.isDevelopment).toBe(true);
      } else {
        expect(EnvironmentUtils.isDevelopment).toBe(false);
      }
    });
  });

  describe("isQA", () => {
    it("should return true when environment is qa", () => {
      const actualEnv = import.meta.env.VITE_APP_ENVIRONMENT;
      if (actualEnv === "qa") {
        expect(EnvironmentUtils.isQA).toBe(true);
      } else {
        expect(EnvironmentUtils.isQA).toBe(false);
      }
    });
  });

  describe("isUAT", () => {
    it("should return true when environment is uat", () => {
      const actualEnv = import.meta.env.VITE_APP_ENVIRONMENT;
      if (actualEnv === "uat") {
        expect(EnvironmentUtils.isUAT).toBe(true);
      } else {
        expect(EnvironmentUtils.isUAT).toBe(false);
      }
    });
  });

  describe("isProduction", () => {
    it("should return true when environment is production", () => {
      const actualEnv = import.meta.env.VITE_APP_ENVIRONMENT;
      if (actualEnv === "production") {
        expect(EnvironmentUtils.isProduction).toBe(true);
      } else {
        expect(EnvironmentUtils.isProduction).toBe(false);
      }
    });
  });

  describe("isDevelopmentOrQA", () => {
    it("should return true when environment is development or qa", () => {
      const actualEnv = import.meta.env.VITE_APP_ENVIRONMENT;
      const expected = actualEnv === "development" || actualEnv === "qa";
      expect(EnvironmentUtils.isDevelopmentOrQA).toBe(expected);
    });
  });

  describe("isOktaEnabled", () => {
    it("should return boolean based on OKTA_ENABLED setting", () => {
      const actualOkta = import.meta.env.VITE_REACT_APP_OKTA_ENABLED;
      const expected = actualOkta === "true";
      expect(EnvironmentUtils.isOktaEnabled).toBe(expected);
    });
  });

  describe("envMode", () => {
    it("should return the current environment mode", () => {
      const actualEnv = import.meta.env.VITE_APP_ENVIRONMENT;
      expect(EnvironmentUtils.envMode).toBe(actualEnv);
    });
  });

  describe("postLogoutRedirectUri", () => {
    it("should return Okta logout URL", () => {
      expect(EnvironmentUtils.postLogoutRedirectUri).toBe("https://marketbasket.okta.com/login/default");
    });

    it("should return same value regardless of environment", () => {
      const url1 = EnvironmentUtils.postLogoutRedirectUri;
      const url2 = EnvironmentUtils.postLogoutRedirectUri;
      expect(url1).toBe(url2);
      expect(url1).toBe("https://marketbasket.okta.com/login/default");
    });
  });

  describe("environment detection consistency", () => {
    it("should only have one environment as true at a time", () => {
      const results = {
        development: EnvironmentUtils.isDevelopment,
        qa: EnvironmentUtils.isQA,
        uat: EnvironmentUtils.isUAT,
        production: EnvironmentUtils.isProduction
      };

      // Count how many are true
      const trueCount = Object.values(results).filter((v) => v === true).length;

      // Should be exactly one (or zero if unknown environment)
      expect(trueCount).toBeLessThanOrEqual(1);
    });

    it("should have consistent envMode with is* properties", () => {
      const envMode = EnvironmentUtils.envMode;

      switch (envMode) {
        case "development":
          expect(EnvironmentUtils.isDevelopment).toBe(true);
          expect(EnvironmentUtils.isDevelopmentOrQA).toBe(true);
          expect(EnvironmentUtils.isQA).toBe(false);
          expect(EnvironmentUtils.isUAT).toBe(false);
          expect(EnvironmentUtils.isProduction).toBe(false);
          break;
        case "qa":
          expect(EnvironmentUtils.isQA).toBe(true);
          expect(EnvironmentUtils.isDevelopmentOrQA).toBe(true);
          expect(EnvironmentUtils.isDevelopment).toBe(false);
          expect(EnvironmentUtils.isUAT).toBe(false);
          expect(EnvironmentUtils.isProduction).toBe(false);
          break;
        case "uat":
          expect(EnvironmentUtils.isUAT).toBe(true);
          expect(EnvironmentUtils.isDevelopmentOrQA).toBe(false);
          expect(EnvironmentUtils.isDevelopment).toBe(false);
          expect(EnvironmentUtils.isQA).toBe(false);
          expect(EnvironmentUtils.isProduction).toBe(false);
          break;
        case "production":
          expect(EnvironmentUtils.isProduction).toBe(true);
          expect(EnvironmentUtils.isDevelopmentOrQA).toBe(false);
          expect(EnvironmentUtils.isDevelopment).toBe(false);
          expect(EnvironmentUtils.isQA).toBe(false);
          expect(EnvironmentUtils.isUAT).toBe(false);
          break;
      }
    });
  });

  describe("property types and values", () => {
    it("should return boolean for all is* properties", () => {
      expect(typeof EnvironmentUtils.isDevelopment).toBe("boolean");
      expect(typeof EnvironmentUtils.isQA).toBe("boolean");
      expect(typeof EnvironmentUtils.isUAT).toBe("boolean");
      expect(typeof EnvironmentUtils.isProduction).toBe("boolean");
      expect(typeof EnvironmentUtils.isDevelopmentOrQA).toBe("boolean");
      expect(typeof EnvironmentUtils.isOktaEnabled).toBe("boolean");
    });

    it("should return envMode from environment variable", () => {
      const actualEnv = import.meta.env.VITE_APP_ENVIRONMENT;
      expect(EnvironmentUtils.envMode).toBe(actualEnv);
    });

    it("should return string for postLogoutRedirectUri", () => {
      expect(typeof EnvironmentUtils.postLogoutRedirectUri).toBe("string");
      expect(EnvironmentUtils.postLogoutRedirectUri).toContain("okta.com");
    });
  });

  describe("static class behavior", () => {
    it("should be able to access properties without instantiation", () => {
      expect(() => EnvironmentUtils.isDevelopment).not.toThrow();
      expect(() => EnvironmentUtils.envMode).not.toThrow();
      expect(() => EnvironmentUtils.postLogoutRedirectUri).not.toThrow();
    });

    it("should return consistent values on multiple accesses", () => {
      const isDev1 = EnvironmentUtils.isDevelopment;
      const isDev2 = EnvironmentUtils.isDevelopment;
      expect(isDev1).toBe(isDev2);

      const envMode1 = EnvironmentUtils.envMode;
      const envMode2 = EnvironmentUtils.envMode;
      expect(envMode1).toBe(envMode2);
    });
  });
});
