import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface ModalState {
  isOpen: boolean;
  title: string;
  body: string;
  confirmLabel: string;
  cancelLabel: string;
  confirmActionType: string | null;
  confirmPayload: any;
}

const initialState: ModalState = {
  isOpen: false,
  title: "",
  body: "",
  confirmLabel: "Confirm",
  cancelLabel: "Cancel",
  confirmActionType: null,
  confirmPayload: null,
};

export const modalSlice = createSlice({
  name: "modalSlice",
  initialState,
  reducers: {
    showModal: (
      state,
      action: PayloadAction<{
        title: string;
        body: string;
        confirmLabel?: string;
        cancelLabel?: string;
        confirmActionType?: string;
        confirmPayload?: any;
      }>
    ) => {
      state.isOpen = true;
      state.title = action.payload.title;
      state.body = action.payload.body;
      state.confirmLabel = action.payload.confirmLabel ?? "Confirm";
      state.cancelLabel = action.payload.cancelLabel ?? "Cancel";
      state.confirmActionType = action.payload.confirmActionType ?? null;
      state.confirmPayload = action.payload.confirmPayload ?? null;
    },
    hideModal: (state) => {
      state.isOpen = false;
      state.title = "";
      state.body = "";
      state.confirmLabel = "Confirm";
      state.cancelLabel = "Cancel";
      state.confirmActionType = null;
      state.confirmPayload = null;
    },
  },
});

export const { showModal, hideModal } = modalSlice.actions;
export default modalSlice.reducer;
