import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface ReportState {
  isDownloading: boolean;
  hasNotifiedReady: boolean;
  lastJobStatus: string | null;
}

const initialState: ReportState = {
  isDownloading: false,
  hasNotifiedReady: false,
  lastJobStatus: null,
};

export const reportSlice = createSlice({
  name: "reportSlice",
  initialState,
  reducers: {
    setDownloading: (state, action: PayloadAction<boolean>) => {
      state.isDownloading = action.payload;
    },
    setHasNotifiedReady: (state, action: PayloadAction<boolean>) => {
      state.hasNotifiedReady = action.payload;
    },
    setLastJobStatus: (state, action: PayloadAction<string | null>) => {
      state.lastJobStatus = action.payload;
    },
    resetReportState: (state) => {
      state.isDownloading = false;
      state.hasNotifiedReady = false;
      state.lastJobStatus = null;
    },
  },
});

export const {
  setDownloading,
  setHasNotifiedReady,
  setLastJobStatus,
  resetReportState,
} = reportSlice.actions;

export default reportSlice.reducer;
