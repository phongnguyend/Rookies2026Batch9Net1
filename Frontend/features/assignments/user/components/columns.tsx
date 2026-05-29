import { ViewUserAssignments } from "../user-assignment.types";
import { displayAssignmentState } from "@/utils/assignment.utils";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import { formatDate } from "@/utils/datetime.utils";
import { AssignmentState } from "@/lib/api/base.types";
import { ColumnDef } from "@/features/shared/components/SingleSortDataTable";

export function createColumns(handlers: {
  onAcceptClick: (
    assignment: ViewUserAssignments.UserAssignmentSummary,
  ) => void;
  onDeclineClick: (
    assignment: ViewUserAssignments.UserAssignmentSummary,
  ) => void;
  onReturnClick: (
    assignment: ViewUserAssignments.UserAssignmentSummary,
  ) => void;
}): ColumnDef<ViewUserAssignments.UserAssignmentSummary>[] {
  return [
    {
      key: "assetCode",
      header: "Asset Code",
      sortable: true,
      headerTestId: "btnSortAssetCode",
      cellTestId: (_, index) => `colAssetCode-${index}`,
      className: "w-32",
    },
    {
      key: "assetName",
      header: "Asset Name",
      sortable: true,
      headerTestId: "btnSortAssetName",
      cellTestId: (_, index) => `colAssetName-${index}`,
      className: "w-60",
    },
    {
      key: "category",
      header: "Category",
      sortable: true,
      headerTestId: "btnSortCategory",
      cellTestId: (_, index) => `colCategory-${index}`,
      className: "w-36",
    },
    {
      key: "assignedDate",
      header: "Assigned Date",
      sortable: true,
      render: (assignment) => formatDate(assignment.assignedDate),
      headerTestId: "btnSortAssignedDate",
      cellTestId: (_, index) => `colAssignedDate-${index}`,
      className: "w-36",
    },
    {
      key: "state",
      header: "State",
      sortable: true,
      render: (assignment) => displayAssignmentState(assignment.state),
      headerTestId: "btnSortState",
      cellTestId: (_, index) => `colState-${index}`,
      className: "w-40",
    },
    {
      key: "actions",
      header: "",
      className: "w-36",
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
            onAccept={handlers.onAcceptClick}
            onDecline={handlers.onDeclineClick}
            onReturn={handlers.onReturnClick}
          />
        );
      },
    },
  ];
}
