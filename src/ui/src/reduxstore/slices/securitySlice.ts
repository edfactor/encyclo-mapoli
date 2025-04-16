import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ImpersonationRoles } from "reduxstore/types";

export interface SecurityState {
  token: string | null;
  userGroups: string[];
  userRoles: string[];
  userPermissions: string[];
  username: string;
  performLogout: boolean;
  appUser: AppUser | null;
  impersonating: ImpersonationRoles | null;
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
  impersonating: null
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
    setImpersonating: (state, action: PayloadAction<ImpersonationRoles | null>) => {
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
      state.impersonating = null;
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
