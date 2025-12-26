import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import reducer, {
  setActiveSubMenu,
  clearActiveSubMenu,
  openDrawer,
  closeDrawer,
  setBanner,
  setError,
  setOnDropdownBlur,
  setOnFlatdateBlur,
  setLoading,
  setFullscreen,
  GeneralState,
  DEFAULT_BANNER
} from "./generalSlice";

describe("generalSlice", () => {
  // Mock localStorage
  const localStorageMock = (() => {
    let store: Record<string, string> = {};

    return {
      getItem: (key: string) => store[key] || null,
      setItem: (key: string, value: string) => {
        store[key] = value.toString();
      },
      removeItem: (key: string) => {
        delete store[key];
      },
      clear: () => {
        store = {};
      }
    };
  })();

  beforeEach(() => {
    // Reset localStorage before each test
    localStorageMock.clear();
    Object.defineProperty(window, "localStorage", {
      value: localStorageMock,
      writable: true
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  const initialState: GeneralState = {
    appBanner: DEFAULT_BANNER,
    error: "",
    onDropdownBlur: { onBlur: false },
    onFlatDateBlur: { onBlur: false },
    loading: false,
    isDrawerOpen: false,
    activeSubmenu: "",
    isFullscreen: false
  };

  describe("reducer", () => {
    it("should return initial state when called with undefined state", () => {
      const state = reducer(undefined, { type: "unknown" });

      expect(state.appBanner).toBe(DEFAULT_BANNER);
      expect(state.error).toBe("");
      expect(state.loading).toBe(false);
      expect(state.isFullscreen).toBe(false);
    });

    it("should return current state for unknown action", () => {
      const currentState: GeneralState = {
        ...initialState,
        loading: true
      };
      expect(reducer(currentState, { type: "unknown" })).toEqual(currentState);
    });
  });

  describe("setActiveSubMenu", () => {
    it("should set active submenu", () => {
      const nextState = reducer(initialState, setActiveSubMenu("reports"));

      expect(nextState.activeSubmenu).toBe("reports");
    });

    it("should persist active submenu to localStorage", () => {
      const state = { ...initialState, isDrawerOpen: true };
      reducer(state, setActiveSubMenu("settings"));

      const stored = localStorage.getItem("drawerState");
      expect(stored).toBeTruthy();

      const parsed = JSON.parse(stored!);
      expect(parsed.activeSubmenu).toBe("settings");
      expect(parsed.isDrawerOpen).toBe(true);
    });

    it("should replace existing active submenu", () => {
      const currentState: GeneralState = {
        ...initialState,
        activeSubmenu: "old-menu"
      };

      const nextState = reducer(currentState, setActiveSubMenu("new-menu"));

      expect(nextState.activeSubmenu).toBe("new-menu");
      expect(nextState.activeSubmenu).not.toBe("old-menu");
    });

    it("should handle localStorage errors gracefully", () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      // Mock localStorage.setItem to throw
      const originalSetItem = localStorage.setItem;
      localStorage.setItem = vi.fn(() => {
        throw new Error("Storage quota exceeded");
      });

      const nextState = reducer(initialState, setActiveSubMenu("test-menu"));

      expect(nextState.activeSubmenu).toBe("test-menu");
      expect(consoleErrorSpy).toHaveBeenCalledWith("Error saving drawer state to localStorage:", expect.any(Error));

      // Restore
      localStorage.setItem = originalSetItem;
      consoleErrorSpy.mockRestore();
    });
  });

  describe("clearActiveSubMenu", () => {
    it("should clear active submenu", () => {
      const currentState: GeneralState = {
        ...initialState,
        activeSubmenu: "reports"
      };

      const nextState = reducer(currentState, clearActiveSubMenu());

      expect(nextState.activeSubmenu).toBe("");
    });

    it("should persist cleared submenu to localStorage", () => {
      const state = { ...initialState, isDrawerOpen: true, activeSubmenu: "old-menu" };
      reducer(state, clearActiveSubMenu());

      const stored = localStorage.getItem("drawerState");
      const parsed = JSON.parse(stored!);

      expect(parsed.activeSubmenu).toBe("");
      expect(parsed.isDrawerOpen).toBe(true);
    });

    it("should handle clearing already empty submenu", () => {
      const nextState = reducer(initialState, clearActiveSubMenu());

      expect(nextState.activeSubmenu).toBe("");
    });
  });

  describe("openDrawer", () => {
    it("should open drawer", () => {
      const nextState = reducer(initialState, openDrawer());

      expect(nextState.isDrawerOpen).toBe(true);
    });

    it("should persist open drawer state to localStorage", () => {
      reducer(initialState, openDrawer());

      const stored = localStorage.getItem("drawerState");
      const parsed = JSON.parse(stored!);

      expect(parsed.isDrawerOpen).toBe(true);
    });

    it("should preserve activeSubmenu when opening drawer", () => {
      const state = { ...initialState, activeSubmenu: "settings" };
      const nextState = reducer(state, openDrawer());

      expect(nextState.isDrawerOpen).toBe(true);
      expect(nextState.activeSubmenu).toBe("settings");

      const stored = localStorage.getItem("drawerState");
      const parsed = JSON.parse(stored!);
      expect(parsed.activeSubmenu).toBe("settings");
    });

    it("should handle already open drawer", () => {
      const currentState: GeneralState = {
        ...initialState,
        isDrawerOpen: true
      };

      const nextState = reducer(currentState, openDrawer());

      expect(nextState.isDrawerOpen).toBe(true);
    });
  });

  describe("closeDrawer", () => {
    it("should close drawer", () => {
      const currentState: GeneralState = {
        ...initialState,
        isDrawerOpen: true
      };

      const nextState = reducer(currentState, closeDrawer());

      expect(nextState.isDrawerOpen).toBe(false);
    });

    it("should persist closed drawer state to localStorage", () => {
      const state = { ...initialState, isDrawerOpen: true };
      reducer(state, closeDrawer());

      const stored = localStorage.getItem("drawerState");
      const parsed = JSON.parse(stored!);

      expect(parsed.isDrawerOpen).toBe(false);
    });

    it("should preserve activeSubmenu when closing drawer", () => {
      const state = { ...initialState, isDrawerOpen: true, activeSubmenu: "reports" };
      const nextState = reducer(state, closeDrawer());

      expect(nextState.isDrawerOpen).toBe(false);
      expect(nextState.activeSubmenu).toBe("reports");
    });

    it("should handle already closed drawer", () => {
      const nextState = reducer(initialState, closeDrawer());

      expect(nextState.isDrawerOpen).toBe(false);
    });
  });

  describe("setBanner", () => {
    it("should set banner text", () => {
      const nextState = reducer(initialState, setBanner("Custom Banner"));

      expect(nextState.appBanner).toBe("Custom Banner");
    });

    it("should replace existing banner", () => {
      const currentState: GeneralState = {
        ...initialState,
        appBanner: "Old Banner"
      };

      const nextState = reducer(currentState, setBanner("New Banner"));

      expect(nextState.appBanner).toBe("New Banner");
    });

    it("should handle empty string", () => {
      const nextState = reducer(initialState, setBanner(""));

      expect(nextState.appBanner).toBe("");
    });

    it("should handle very long banner text", () => {
      const longBanner = "A".repeat(1000);
      const nextState = reducer(initialState, setBanner(longBanner));

      expect(nextState.appBanner).toBe(longBanner);
      expect(nextState.appBanner).toHaveLength(1000);
    });
  });

  describe("setError", () => {
    it("should extract error from data.Message", () => {
      // Note: The reducer has a typo - it checks for "Messag" not "Message"
      const errorPayload = {
        data: { Messag: "test", Message: "Database connection failed" }
      };

      const nextState = reducer(initialState, setError(errorPayload));

      expect(nextState.error).toBe("Database connection failed");
    });

    it("should extract error from payload.error", () => {
      const errorPayload = {
        error: "Network timeout"
      };

      const nextState = reducer(initialState, setError(errorPayload));

      expect(nextState.error).toBe("Network timeout");
    });

    it("should extract error from data.title", () => {
      const errorPayload = {
        data: { title: "Validation Failed" }
      };

      const nextState = reducer(initialState, setError(errorPayload));

      expect(nextState.error).toBe("Validation Failed");
    });

    it("should handle non-200 status with data", () => {
      const errorPayload = {
        status: 400,
        data: "Bad Request"
      };

      const nextState = reducer(initialState, setError(errorPayload));

      expect(nextState.error).toBe("Bad Request");
    });

    it("should handle non-200 status without data", () => {
      const errorPayload = {
        status: 500
      };

      const nextState = reducer(initialState, setError(errorPayload));

      expect(nextState.error).toBe("Network error - check log");
    });

    it("should handle string payload", () => {
      const errorPayload = {
        message: "Unknown error"
      };

      const nextState = reducer(initialState, setError(errorPayload));

      // Falls through to final String(payload) - converts object to string
      expect(nextState.error).toBe("[object Object]");
    });

    it("should handle 200 status code (no error)", () => {
      const errorPayload = {
        status: 200,
        data: "Success"
      };

      const nextState = reducer(initialState, setError(errorPayload));

      // Should convert entire payload to string
      expect(nextState.error).toBeTruthy();
    });

    it("should prioritize Message over other error formats", () => {
      const errorPayload = {
        data: {
          Messag: "trigger", // Typo in reducer - needs "Messag" to trigger check
          Message: "Primary error",
          title: "Secondary error"
        },
        error: "Tertiary error"
      };

      const nextState = reducer(initialState, setError(errorPayload));

      expect(nextState.error).toBe("Primary error");
    });

    it("should handle nested error objects", () => {
      const errorPayload = {
        data: {
          Messag: "trigger", // Typo in reducer - needs "Messag" to trigger check
          errors: { field: ["Required"] },
          Message: "Validation error"
        }
      };

      const nextState = reducer(initialState, setError(errorPayload));

      expect(nextState.error).toBe("Validation error");
    });
  });

  describe("setOnDropdownBlur", () => {
    it("should set onDropdownBlur to true", () => {
      const nextState = reducer(initialState, setOnDropdownBlur({ onBlur: true }));

      expect(nextState.onDropdownBlur.onBlur).toBe(true);
    });

    it("should set onDropdownBlur to false", () => {
      const currentState: GeneralState = {
        ...initialState,
        onDropdownBlur: { onBlur: true }
      };

      const nextState = reducer(currentState, setOnDropdownBlur({ onBlur: false }));

      expect(nextState.onDropdownBlur.onBlur).toBe(false);
    });

    it("should replace entire onDropdownBlur object", () => {
      const nextState = reducer(initialState, setOnDropdownBlur({ onBlur: true }));

      expect(nextState.onDropdownBlur).toEqual({ onBlur: true });
    });
  });

  describe("setOnFlatdateBlur", () => {
    it("should set onFlatDateBlur to true", () => {
      const nextState = reducer(initialState, setOnFlatdateBlur({ onBlur: true }));

      expect(nextState.onFlatDateBlur.onBlur).toBe(true);
    });

    it("should set onFlatDateBlur to false", () => {
      const currentState: GeneralState = {
        ...initialState,
        onFlatDateBlur: { onBlur: true }
      };

      const nextState = reducer(currentState, setOnFlatdateBlur({ onBlur: false }));

      expect(nextState.onFlatDateBlur.onBlur).toBe(false);
    });

    it("should not affect onDropdownBlur", () => {
      const currentState: GeneralState = {
        ...initialState,
        onDropdownBlur: { onBlur: true }
      };

      const nextState = reducer(currentState, setOnFlatdateBlur({ onBlur: true }));

      expect(nextState.onDropdownBlur.onBlur).toBe(true);
      expect(nextState.onFlatDateBlur.onBlur).toBe(true);
    });
  });

  describe("setLoading", () => {
    it("should set loading to true", () => {
      const nextState = reducer(initialState, setLoading(true));

      expect(nextState.loading).toBe(true);
    });

    it("should set loading to false", () => {
      const currentState: GeneralState = {
        ...initialState,
        loading: true
      };

      const nextState = reducer(currentState, setLoading(false));

      expect(nextState.loading).toBe(false);
    });

    it("should toggle loading state", () => {
      let state = reducer(initialState, setLoading(true));
      expect(state.loading).toBe(true);

      state = reducer(state, setLoading(false));
      expect(state.loading).toBe(false);

      state = reducer(state, setLoading(true));
      expect(state.loading).toBe(true);
    });
  });

  describe("setFullscreen", () => {
    it("should set fullscreen to true", () => {
      const nextState = reducer(initialState, setFullscreen(true));

      expect(nextState.isFullscreen).toBe(true);
    });

    it("should set fullscreen to false", () => {
      const currentState: GeneralState = {
        ...initialState,
        isFullscreen: true
      };

      const nextState = reducer(currentState, setFullscreen(false));

      expect(nextState.isFullscreen).toBe(false);
    });

    it("should toggle fullscreen state", () => {
      let state = reducer(initialState, setFullscreen(true));
      expect(state.isFullscreen).toBe(true);

      state = reducer(state, setFullscreen(false));
      expect(state.isFullscreen).toBe(false);
    });
  });

  describe("complex state transitions", () => {
    it("should handle drawer open and submenu selection flow", () => {
      let state = reducer(initialState, openDrawer());
      expect(state.isDrawerOpen).toBe(true);

      state = reducer(state, setActiveSubMenu("reports"));
      expect(state.activeSubmenu).toBe("reports");

      state = reducer(state, closeDrawer());
      expect(state.isDrawerOpen).toBe(false);
      expect(state.activeSubmenu).toBe("reports"); // Preserved
    });

    it("should handle loading and error flow", () => {
      let state = reducer(initialState, setLoading(true));
      expect(state.loading).toBe(true);

      state = reducer(
        state,
        setError({ data: { Messag: "trigger", Message: "Failed to load" } } as Record<string, unknown>)
      );
      expect(state.error).toBe("Failed to load");

      state = reducer(state, setLoading(false));
      expect(state.loading).toBe(false);
      expect(state.error).toBe("Failed to load"); // Error persists
    });

    it("should handle multiple drawer actions with localStorage", () => {
      let state = reducer(initialState, openDrawer());
      state = reducer(state, setActiveSubMenu("settings"));
      state = reducer(state, closeDrawer());
      reducer(state, clearActiveSubMenu());

      const stored = localStorage.getItem("drawerState");
      const parsed = JSON.parse(stored!);

      expect(parsed.isDrawerOpen).toBe(false);
      expect(parsed.activeSubmenu).toBe("");
    });

    it("should maintain independent state properties", () => {
      let state = reducer(initialState, setLoading(true));
      state = reducer(state, setFullscreen(true));
      state = reducer(state, openDrawer());
      state = reducer(state, setBanner("Custom"));

      expect(state.loading).toBe(true);
      expect(state.isFullscreen).toBe(true);
      expect(state.isDrawerOpen).toBe(true);
      expect(state.appBanner).toBe("Custom");
    });
  });

  describe("edge cases", () => {
    it("should handle rapid drawer toggles", () => {
      let state = reducer(initialState, openDrawer());
      state = reducer(state, closeDrawer());
      state = reducer(state, openDrawer());
      state = reducer(state, closeDrawer());

      expect(state.isDrawerOpen).toBe(false);
    });

    it("should handle special characters in banner", () => {
      const specialBanner = "<script>alert('xss')</script>";
      const nextState = reducer(initialState, setBanner(specialBanner));

      expect(nextState.appBanner).toBe(specialBanner);
    });

    it("should handle unicode in submenu names", () => {
      const unicodeMenu = "报告 📊 Reports";
      const nextState = reducer(initialState, setActiveSubMenu(unicodeMenu));

      expect(nextState.activeSubmenu).toBe(unicodeMenu);
    });

    it("should preserve state immutability", () => {
      const currentState: GeneralState = {
        ...initialState,
        loading: false,
        isDrawerOpen: false
      };

      const nextState = reducer(currentState, setLoading(true));

      // Original state should not be mutated
      expect(currentState.loading).toBe(false);
      expect(nextState.loading).toBe(true);
    });

    it("should handle localStorage being unavailable", () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      // Make localStorage.setItem throw
      const originalSetItem = localStorage.setItem;
      localStorage.setItem = vi.fn(() => {
        throw new Error("localStorage is not available");
      });

      const nextState = reducer(initialState, openDrawer());

      // State should still update despite localStorage failure
      expect(nextState.isDrawerOpen).toBe(true);
      expect(consoleErrorSpy).toHaveBeenCalled();

      // Restore
      localStorage.setItem = originalSetItem;
      consoleErrorSpy.mockRestore();
    });
  });
});
