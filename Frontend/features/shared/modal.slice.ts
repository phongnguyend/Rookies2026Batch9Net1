import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface ModalState {
  isOpen: boolean;
  title: string;
  body: string;
  yesButtonLabel: string;
  noButtonLabel: string;
  yesActionType: string | null;
  yesPayload: unknown;
  noActionType: string | null;
  noPayload: unknown;
}

const initialState: ModalState = {
  isOpen: false,
  title: "",
  body: "",
  yesButtonLabel: "Yes",
  noButtonLabel: "No",
  yesActionType: null,
  yesPayload: null,
  noActionType: null,
  noPayload: null,
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
        yesButtonLabel?: string;
        noButtonLabel?: string;
        yesActionType?: string;
        yesPayload?: unknown;
        noActionType?: string;
        noPayload?: unknown;
      }>
    ) => {
      state.isOpen = true;
      state.title = action.payload.title;
      state.body = action.payload.body;
      state.yesButtonLabel = action.payload.yesButtonLabel ?? "Yes";
      state.noButtonLabel = action.payload.noButtonLabel ?? "No";
      state.yesActionType = action.payload.yesActionType ?? null;
      state.yesPayload = action.payload.yesPayload ?? null;
      state.noActionType = action.payload.noActionType ?? null;
      state.noPayload = action.payload.noPayload ?? null;
    },
    hideModal: (state) => {
      state.isOpen = false;
      state.title = "";
      state.body = "";
      state.yesButtonLabel = "Yes";
      state.noButtonLabel = "No";
      state.yesActionType = null;
      state.yesPayload = null;
      state.noActionType = null;
      state.noPayload = null;
    },
  },
});

export const { showModal, hideModal } = modalSlice.actions;
export default modalSlice.reducer;