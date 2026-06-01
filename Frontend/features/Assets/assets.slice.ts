import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface AssetState {
  isCreatedNewAsset: boolean;
}

const initialState: AssetState = {
  isCreatedNewAsset: false,
};

const assetSlice = createSlice({
  name: "asset",
  initialState,
  reducers: {
    setCreatedNewAsset(state, action: PayloadAction<boolean>) {
      state.isCreatedNewAsset = action.payload;
    },
  },
});

export const { setCreatedNewAsset } = assetSlice.actions;
export default assetSlice.reducer;
