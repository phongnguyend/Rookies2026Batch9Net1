import { RootState } from "@/lib/redux/store";

export const selectPromotedAssignment = (state: RootState) =>
  state.adminAssignmentListUi.promotedAssignment;
