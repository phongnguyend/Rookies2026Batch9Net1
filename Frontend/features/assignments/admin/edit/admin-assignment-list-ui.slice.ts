import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Assignment } from "../assignments.types";

interface AdminAssignmentListUiState {
  promotedAssignment?: Assignment;
}

const initialState: AdminAssignmentListUiState = {};

const adminAssignmentListUiSlice = createSlice({
  name: "adminAssignmentListUi",
  initialState,
  reducers: {
    setPromotedAssignment(state, action: PayloadAction<Assignment>) {
      state.promotedAssignment = action.payload;
    },

    clearPromotedAssignment(state) {
      state.promotedAssignment = undefined;
    },
  },
});

export const { setPromotedAssignment, clearPromotedAssignment } =
  adminAssignmentListUiSlice.actions;

export default adminAssignmentListUiSlice.reducer;
