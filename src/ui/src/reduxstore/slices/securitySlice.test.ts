import { describe, it, expect } from "vitest";
import reducer, {
  setUserRoles,
  setUserPermissions,
  setUsername,
  setPerformLogout,
  setToken,
  setImpersonating,
  setUserGroups,
  clearUserData,
  SecurityState,
  AppUser
} from "./securitySlice";
import { ImpersonationRoles } from "reduxstore/types";

describe("securitySlice", () => {
  const initialState: SecurityState = {
    token: null,
    userRoles: [],
    userPermissions: [],
    userGroups: [],
    username: "",
    performLogout: false,
    appUser: null,
    impersonating: []
  };

  const mockAppUser: AppUser = {
    permissions: ["read", "write"],
    userName: "testuser",
    userEmail: "test@example.com",
    storeId: 123,
    canImpersonate: true,
    impersonatibleRoles: ["admin", "manager"]
  };

  describe("reducer", () => {
    it("should return initial state when called with undefined state", () => {
      expect(reducer(undefined, { type: "unknown" })).toEqual(initialState);
    });

    it("should return current state for unknown action", () => {
      const currentState: SecurityState = {
        ...initialState,
        token: "test-token"
      };
      expect(reducer(currentState, { type: "unknown" })).toEqual(currentState);
    });
  });

  describe("setUserRoles", () => {
    it("should set user roles", () => {
      const roles = ["admin", "user", "manager"];
      const nextState = reducer(initialState, setUserRoles(roles));

      expect(nextState.userRoles).toEqual(roles);
      expect(nextState.userRoles).toHaveLength(3);
    });

    it("should replace existing user roles", () => {
      const currentState: SecurityState = {
        ...initialState,
        userRoles: ["old-role"]
      };
      const newRoles = ["new-role-1", "new-role-2"];

      const nextState = reducer(currentState, setUserRoles(newRoles));

      expect(nextState.userRoles).toEqual(newRoles);
      expect(nextState.userRoles).not.toContain("old-role");
    });

    it("should handle empty array", () => {
      const currentState: SecurityState = {
        ...initialState,
        userRoles: ["admin", "user"]
      };

      const nextState = reducer(currentState, setUserRoles([]));

      expect(nextState.userRoles).toEqual([]);
      expect(nextState.userRoles).toHaveLength(0);
    });
  });

  describe("setUserPermissions", () => {
    it("should set user permissions", () => {
      const permissions = ["read", "write", "delete"];
      const nextState = reducer(initialState, setUserPermissions(permissions));

      expect(nextState.userPermissions).toEqual(permissions);
    });

    it("should replace existing permissions", () => {
      const currentState: SecurityState = {
        ...initialState,
        userPermissions: ["read"]
      };

      const nextState = reducer(currentState, setUserPermissions(["write", "execute"]));

      expect(nextState.userPermissions).toEqual(["write", "execute"]);
    });
  });

  describe("setUsername", () => {
    it("should set username", () => {
      const nextState = reducer(initialState, setUsername("john.doe"));

      expect(nextState.username).toBe("john.doe");
    });

    it("should replace existing username", () => {
      const currentState: SecurityState = {
        ...initialState,
        username: "old-user"
      };

      const nextState = reducer(currentState, setUsername("new-user"));

      expect(nextState.username).toBe("new-user");
    });

    it("should handle empty string", () => {
      const currentState: SecurityState = {
        ...initialState,
        username: "existing-user"
      };

      const nextState = reducer(currentState, setUsername(""));

      expect(nextState.username).toBe("");
    });
  });

  describe("setPerformLogout", () => {
    it("should set performLogout to true", () => {
      const nextState = reducer(initialState, setPerformLogout(true));

      expect(nextState.performLogout).toBe(true);
    });

    it("should set performLogout to false", () => {
      const currentState: SecurityState = {
        ...initialState,
        performLogout: true
      };

      const nextState = reducer(currentState, setPerformLogout(false));

      expect(nextState.performLogout).toBe(false);
    });

    it("should toggle performLogout state", () => {
      let state = reducer(initialState, setPerformLogout(true));
      expect(state.performLogout).toBe(true);

      state = reducer(state, setPerformLogout(false));
      expect(state.performLogout).toBe(false);
    });
  });

  describe("setToken", () => {
    it("should set authentication token", () => {
      const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test";
      const nextState = reducer(initialState, setToken(token));

      expect(nextState.token).toBe(token);
    });

    it("should replace existing token", () => {
      const currentState: SecurityState = {
        ...initialState,
        token: "old-token"
      };

      const nextState = reducer(currentState, setToken("new-token"));

      expect(nextState.token).toBe("new-token");
    });

    it("should handle empty token", () => {
      const currentState: SecurityState = {
        ...initialState,
        token: "existing-token"
      };

      const nextState = reducer(currentState, setToken(""));

      expect(nextState.token).toBe("");
    });
  });

  // Note: setUserInfo is defined in the slice but not exported, so it cannot be tested directly.
  // The appUser field can only be tested via clearUserData which sets it to null.

  describe("setImpersonating", () => {
    it("should set impersonating roles", () => {
      const impersonatingRoles: ImpersonationRoles[] = [ImpersonationRoles.Auditor, ImpersonationRoles.FinanceManager];

      const nextState = reducer(initialState, setImpersonating(impersonatingRoles));

      expect(nextState.impersonating).toEqual(impersonatingRoles);
      expect(nextState.impersonating).toHaveLength(2);
    });

    it("should replace existing impersonating roles", () => {
      const oldRoles: ImpersonationRoles[] = [ImpersonationRoles.Auditor];

      const currentState: SecurityState = {
        ...initialState,
        impersonating: oldRoles
      };

      const newRoles: ImpersonationRoles[] = [ImpersonationRoles.ItDevOps];

      const nextState = reducer(currentState, setImpersonating(newRoles));

      expect(nextState.impersonating).toEqual(newRoles);
      expect(nextState.impersonating).toHaveLength(1);
    });

    it("should handle empty impersonating array", () => {
      const currentState: SecurityState = {
        ...initialState,
        impersonating: [ImpersonationRoles.ProfitSharingAdministrator]
      };

      const nextState = reducer(currentState, setImpersonating([]));

      expect(nextState.impersonating).toEqual([]);
      expect(nextState.impersonating).toHaveLength(0);
    });
  });

  describe("setUserGroups", () => {
    it("should set user groups", () => {
      const groups = ["finance", "hr", "admin"];
      const nextState = reducer(initialState, setUserGroups(groups));

      expect(nextState.userGroups).toEqual(groups);
      expect(nextState.userGroups).toHaveLength(3);
    });

    it("should replace existing user groups", () => {
      const currentState: SecurityState = {
        ...initialState,
        userGroups: ["old-group"]
      };

      const nextState = reducer(currentState, setUserGroups(["new-group-1", "new-group-2"]));

      expect(nextState.userGroups).toEqual(["new-group-1", "new-group-2"]);
    });

    it("should handle empty groups array", () => {
      const currentState: SecurityState = {
        ...initialState,
        userGroups: ["finance", "hr"]
      };

      const nextState = reducer(currentState, setUserGroups([]));

      expect(nextState.userGroups).toEqual([]);
    });
  });

  describe("clearUserData", () => {
    it("should reset all user data to initial state", () => {
      const populatedState: SecurityState = {
        token: "test-token",
        userRoles: ["admin", "user"],
        userPermissions: ["read", "write"],
        userGroups: ["finance"],
        username: "testuser",
        performLogout: true,
        appUser: mockAppUser,
        impersonating: [ImpersonationRoles.ProfitSharingAdministrator]
      };

      const nextState = reducer(populatedState, clearUserData());

      expect(nextState.token).toBe("");
      expect(nextState.appUser).toBeNull();
      expect(nextState.userGroups).toEqual([]);
      expect(nextState.username).toBe("");
      expect(nextState.userRoles).toEqual([]);
      expect(nextState.userPermissions).toEqual([]);
      expect(nextState.performLogout).toBe(false);
      expect(nextState.impersonating).toEqual([]);
    });

    it("should clear user data when called on initial state", () => {
      const nextState = reducer(initialState, clearUserData());

      expect(nextState.token).toBe("");
      expect(nextState.appUser).toBeNull();
      expect(nextState.userGroups).toEqual([]);
      expect(nextState.username).toBe("");
      expect(nextState.userRoles).toEqual([]);
      expect(nextState.userPermissions).toEqual([]);
      expect(nextState.performLogout).toBe(false);
      expect(nextState.impersonating).toEqual([]);
    });

    it("should maintain separate state instances after clearing", () => {
      const populatedState: SecurityState = {
        ...initialState,
        token: "test-token",
        username: "testuser"
      };

      const nextState = reducer(populatedState, clearUserData());

      // Original state should not be mutated
      expect(populatedState.token).toBe("test-token");
      expect(nextState.token).toBe("");
    });
  });

  describe("complex state transitions", () => {
    it("should handle multiple sequential actions", () => {
      let state = reducer(initialState, setToken("jwt-token"));
      state = reducer(state, setUsername("john.doe"));
      state = reducer(state, setUserRoles(["admin"]));
      state = reducer(state, setUserPermissions(["read", "write"]));

      expect(state.token).toBe("jwt-token");
      expect(state.username).toBe("john.doe");
      expect(state.userRoles).toEqual(["admin"]);
      expect(state.userPermissions).toEqual(["read", "write"]);
    });

    it("should handle logout flow correctly", () => {
      let state: SecurityState = {
        ...initialState,
        token: "active-token",
        username: "user@example.com",
        userRoles: ["user"]
      };

      state = reducer(state, setPerformLogout(true));
      expect(state.performLogout).toBe(true);

      state = reducer(state, clearUserData());
      expect(state.token).toBe("");
      expect(state.username).toBe("");
      expect(state.appUser).toBeNull();
      expect(state.performLogout).toBe(false);
    });

    it("should handle impersonation flow", () => {
      const impersonationRoles: ImpersonationRoles[] = [ImpersonationRoles.ProfitSharingAdministrator];

      let state = reducer(initialState, setImpersonating(impersonationRoles));
      expect(state.impersonating).toEqual(impersonationRoles);

      // Stop impersonating
      state = reducer(state, setImpersonating([]));
      expect(state.impersonating).toEqual([]);
    });
  });

  describe("edge cases", () => {
    it("should handle very long arrays", () => {
      const manyRoles = Array.from({ length: 100 }, (_, i) => `role-${i}`);
      const nextState = reducer(initialState, setUserRoles(manyRoles));

      expect(nextState.userRoles).toHaveLength(100);
      expect(nextState.userRoles[0]).toBe("role-0");
      expect(nextState.userRoles[99]).toBe("role-99");
    });

    it("should preserve unmodified state properties", () => {
      const currentState: SecurityState = {
        ...initialState,
        token: "existing-token",
        username: "existing-user",
        userRoles: ["existing-role"]
      };

      const nextState = reducer(currentState, setUserPermissions(["new-permission"]));

      // Only userPermissions should change
      expect(nextState.token).toBe("existing-token");
      expect(nextState.username).toBe("existing-user");
      expect(nextState.userRoles).toEqual(["existing-role"]);
      expect(nextState.userPermissions).toEqual(["new-permission"]);
    });
  });
});
