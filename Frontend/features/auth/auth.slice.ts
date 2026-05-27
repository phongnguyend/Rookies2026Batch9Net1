import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { UserRoles } from "@/features/users/users.types";

export interface AuthState {
  user: {
    username: string;
    role: UserRoles;
    locationName: string;
    isFirstLogin: boolean;
  } | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  error: string | null;
}

const initialState: AuthState = {
  user: null,
  isLoading: true,
  isAuthenticated: false,
  error: null,
};

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {

    // spinner occurs
    loginStart: (state) => {
      state.isLoading = true;
    },

    // save user data
    loginSuccess: (state, action: PayloadAction<{ username: string; role: UserRoles; isFirstLogin: boolean; locationName: string }>) => {
      state.isLoading = false;
      state.user = action.payload;
      state.isAuthenticated = true;
      state.error = null;
    },

    // handle error message
    loginFailure: (state, action: PayloadAction<string>) => {
      state.isLoading = false;
      state.isAuthenticated = false;
      state.error = action.payload;
    },

    completeFirstLogin: (state) => {
      if (state.user) {
        state.user.isFirstLogin = false;
      }
    },

    completeLoading: (state) => {
      state.isLoading = false;
    },

    // logout
    logout: () => { return { ...initialState, isLoading: false } }
  }
})

export const { loginStart, loginSuccess, loginFailure, completeFirstLogin, completeLoading, logout } = authSlice.actions;
export default authSlice.reducer;
