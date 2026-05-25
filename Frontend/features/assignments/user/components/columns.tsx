import { ColumnDef } from "@/features/shared/components/DataTable";
import { ViewUserAssignments } from "../user-assignment.types";
import { displayAssignmentState } from "@/utils/assignment.utils";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import { formatDate } from "@/utils/datetime.utils";
import { AssignmentState } from "@/lib/api/base.types";

export const columns: ColumnDef<ViewUserAssignments.UserAssignmentSummary>[] = [
  {
    key: "assetCode",
    header: "Asset Code",
    sortable: true,
  },
  {
    key: "assetName",
    header: "Asset Name",
    sortable: true,
  },
  {
    key: "category",
    header: "Category",
    sortable: true,
  },
  {
    key: "assignedDate",
    header: "Assigned Date",
    sortable: true,
    render: (assignment) => formatDate(assignment.assignedDate),
  },
  {
    key: "state",
    header: "State",
    sortable: true,
    render: (assignment) => displayAssignmentState(assignment.state),
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
