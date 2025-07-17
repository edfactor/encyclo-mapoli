export default class EnvironmentUtils {
  public static get isDevelopment(): boolean {
    return import.meta.env.VITE_APP_ENVIRONMENT === "development";
  }

  public static get postLogoutRedirectUri(): string {
    return "https://marketbasket.okta.com/login/default";
  }

  public static get isQA(): boolean {
    return import.meta.env.VITE_APP_ENVIRONMENT === "qa";
  }

  public static get isDevelopmentOrQA(): boolean {
    return this.isDevelopment || this.isQA;
  }

  public static get isUAT(): boolean {
    return import.meta.env.VITE_APP_ENVIRONMENT === "uat";
  }

  public static get isProduction(): boolean {
    return import.meta.env.VITE_APP_ENVIRONMENT === "production";
  }

  public static get isOktaEnabled(): boolean {
    return import.meta.env.VITE_REACT_APP_OKTA_ENABLED === "true";
  }

  public static get envMode(): "development" | "qa" | "uat" | "production" {
    return import.meta.env.VITE_APP_ENVIRONMENT;
  }
}
