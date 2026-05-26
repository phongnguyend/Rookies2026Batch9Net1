import { ViewUserAssignments } from "../user-assignment.types";
import { displayAssignmentState } from "@/utils/assignment.utils";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import { formatDate } from "@/utils/datetime.utils";
import { AssignmentState } from "@/lib/api/base.types";
import { ColumnDef } from "@/features/shared/components/SingleSortDataTable";

export const columns: ColumnDef<ViewUserAssignments.UserAssignmentSummary>[] = [
  {
    key: "assetCode",
    header: "Asset Code",
    sortable: true,
    headerTestId: "btnSortAssetCode",
    cellTestId: (_, index) => `colAssetCode-${index}`,
  },
  {
    key: "assetName",
    header: "Asset Name",
    sortable: true,
    headerTestId: "btnSortAssetName",
    cellTestId: (_, index) => `colAssetName-${index}`,
  },
  {
    key: "category",
    header: "Category",
    sortable: true,
    headerTestId: "btnSortCategory",
    cellTestId: (_, index) => `colCategory-${index}`,
  },
  {
    key: "assignedDate",
    header: "Assigned Date",
    sortable: true,
    headerTestId: "btnSortAssignedDate",
    render: (assignment) => formatDate(assignment.assignedDate),
    cellTestId: (_, index) => `colAssignedDate-${index}`,
  },
  {
    key: "state",
    header: "State",
    sortable: true,
    headerTestId: "btnSortState",
    render: (assignment) => displayAssignmentState(assignment.state),
    cellTestId: (_, index) => `colState-${index}`,
  },
  {
    key: "actions",
    header: "",
    render: (assignment) => {
      return (
        <DataTableButtonActions
          row={assignment}
          disabledAccept={
            assignment.state !== AssignmentState.WaitingForAcceptance
          }
          disabledDecline={
            assignment.state !== AssignmentState.WaitingForAcceptance
          }
          disabledReturn={
            assignment.state !== AssignmentState.Accepted ||
            assignment.isReturning
          }
          onAccept={(row) => console.log("accept", row)}
          onDecline={(row) => console.log("decline", row)}
          onReturn={(row) => console.log("return", row)}
        />
      );
    },
  },
];
