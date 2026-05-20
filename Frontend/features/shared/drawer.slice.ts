import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface DrawerState {
  isSidebarOpen: boolean;
}

const initialState: DrawerState = {
  isSidebarOpen: false,
};

export const drawerSlice = createSlice({
  name: "drawerSlice",
  initialState,
  reducers: {
    toggleSidebar: (state) => {
      state.isSidebarOpen = !state.isSidebarOpen;
    },
    setSidebarOpen: (state, action: PayloadAction<boolean>) => {
      state.isSidebarOpen = action.payload;
    },
  },
});

export const { toggleSidebar, setSidebarOpen } = drawerSlice.actions;
export default drawerSlice.reducer;
