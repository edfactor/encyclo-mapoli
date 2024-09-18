import { PayloadAction, createSlice } from "@reduxjs/toolkit";

export interface SeucrityState {
  token: string | null;
  userRoles: string[];
  userPermissions: string[];
  username: string;
  performLogout: boolean;
  appUser: AppUser | null;
}

export type AppUser = {
  permissions: string[];
  userName: string;
  userEmail: string;
  storeId: number | null;
  canImpersonate: boolean;
  impersonatibleRoles: string[];
};

const initialState: SeucrityState = {
  token: null,
  userRoles: [],
  userPermissions: [],
  username: "",
  performLogout: false,
  appUser: null
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
    }
  }
});

export const { setToken, setUserPermissions, setUserRoles, setUsername, setPerformLogout } = securitySlice.actions;
export default securitySlice.reducer;
