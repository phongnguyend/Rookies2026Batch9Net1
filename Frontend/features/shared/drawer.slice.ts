import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface DrawerState {
  isDrawerOpen: boolean;
}

const initialState: DrawerState = {
  isDrawerOpen: false,
};

export const drawerSlice = createSlice({
  name: "drawerSlice",
  initialState,
  reducers: {
    toggleDrawer: (state) => {
      state.isDrawerOpen = !state.isDrawerOpen;
    },
    setDrawerOpen: (state, action: PayloadAction<boolean>) => {
      state.isDrawerOpen = action.payload;
    },
  },
});

export const { toggleDrawer, setDrawerOpen } = drawerSlice.actions;
export default drawerSlice.reducer;
