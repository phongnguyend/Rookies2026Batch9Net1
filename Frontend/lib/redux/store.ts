import { configureStore } from "@reduxjs/toolkit";
import { baseApiSlice } from "../api/base.api";
import { drawerSlice } from "@/features/shared/drawer.slice";
import { toastSlice } from "@/features/shared/toast.slice";

export const makeStore = () => {
  return configureStore({
    reducer: {
      [baseApiSlice.reducerPath]: baseApiSlice.reducer,
      drawerSlice: drawerSlice.reducer,
      toastSlice: toastSlice.reducer,
    },
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware().concat(baseApiSlice.middleware),
  });
};

// Infer the type of makeStore
export type AppStore = ReturnType<typeof makeStore>;
// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<AppStore['getState']>;
export type AppDispatch = AppStore['dispatch'];
