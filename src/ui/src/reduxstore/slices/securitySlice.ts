import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ImpersonationRoles } from "reduxstore/types";
import EnvironmentUtils from "../../utils/environmentUtils";

// CRITICAL DEV/QA FUNCTIONALITY:
// We intentionally persist impersonation roles to localStorage ONLY in Development/QA.
// This supports rapid debugging/testing workflows across refreshes.
// Do NOT remove this without providing an equivalent dev/qa-only mechanism.
// NOTE: Must stay in sync with the key in RouterSubAssembly.
const ImpersonatingRolesStorageKey = "impersonatingRoles";

function getInitialImpersonatingRoles(): ImpersonationRoles[] {
  if (!EnvironmentUtils.isDevelopmentOrQA) {
    return [];
  }

  if (typeof window === "undefined") {
    return [];
  }

  try {
    const raw = window.localStorage?.getItem(ImpersonatingRolesStorageKey);
    if (!raw) {
      return [];
    }

    const parsed = JSON.parse(raw) as unknown;
    if (!Array.isArray(parsed)) {
      return [];
    }

    const allowedRoleValues = new Set<string>(Object.values(ImpersonationRoles));
    return parsed.filter(
      (x): x is ImpersonationRoles => typeof x === "string" && allowedRoleValues.has(x)
    );
  } catch {
    return [];
  }
}

export interface SecurityState {
  token: string | null;
  userGroups: string[];
  userRoles: string[];
  userPermissions: string[];
  username: string;
  performLogout: boolean;
  appUser: AppUser | null;
  impersonating: ImpersonationRoles[];
}

export type AppUser = {
  permissions: string[];
  userName: string;
  userEmail: string;
  storeId: number | null;
  canImpersonate: boolean;
  impersonatibleRoles: string[];
};

const initialState: SecurityState = {
  token: null,
  userRoles: [],
  userPermissions: [],
  userGroups: [],
  username: "",
  performLogout: false,
  appUser: null,
  impersonating: getInitialImpersonatingRoles()
};

export const securitySlice = createSlice({
  name: "security",
  initialState,
  reducers: {
    setUserRoles: (state, action: PayloadAction<string[]>) => {
      state.userRoles = action.payload;
    },
    setUserPermissions: (state, action: PayloadAction<string[]>) => {
      state.userPermissions = action.payload;
    },
    setUsername: (state, action: PayloadAction<string>) => {
      state.username = action.payload;
    },
    setPerformLogout: (state, action: PayloadAction<boolean>) => {
      state.performLogout = action.payload;
    },
    setToken: (state, action: PayloadAction<string>) => {
      state.token = action.payload;
    },
    setUserInfo: (state, action: PayloadAction<AppUser>) => {
      state.appUser = action.payload;
    },
    setImpersonating: (state, action: PayloadAction<ImpersonationRoles[]>) => {
      state.impersonating = action.payload;
    },
    setUserGroups: (state, action: PayloadAction<string[]>) => {
      state.userGroups = action.payload;
    },
    clearUserData: (state) => {
      state.token = "";
      state.appUser = null;
      state.userGroups = [];
      state.username = "";
      state.userRoles = [];
      state.userPermissions = [];
      state.performLogout = false;
      state.impersonating = [];
    }
  }
});

export const {
  setToken,
  setUserPermissions,
  setUserRoles,
  setUsername,
  setPerformLogout,
  setImpersonating,
  setUserGroups,
  clearUserData
} = securitySlice.actions;
export default securitySlice.reducer;
