import { LookupUsers } from "@/features/users/users.types";
import { ColumnDef } from "../../SingleSortDataTable";

export function createColumns(
  pendingUser: LookupUsers.LookupUsersSummary | null,
  onSelect: (user: LookupUsers.LookupUsersSummary) => void,
): ColumnDef<LookupUsers.LookupUsersSummary>[] {
  return [
    {
      key: "selected",
      header: "",
      sortable: false,
      cellTestId: (_, index) => `rdoUser-${index}`,
      className: "w-8 overflow-visible text-center",
      render: (user) => {
        return (
          <input
            type="radio"
            name="user-selection"
            checked={pendingUser?.id === user.id}
            onChange={() => onSelect(user)}
            onClick={(e) => e.stopPropagation()}
            tabIndex={-1} // ← ẩn radio khỏi Tab order
            className="w-4 h-4 appearance-auto accent-primary cursor-pointer"
          />
        );
      },
    },
    {
      key: "staffCode",
      header: "Staff Code",
      sortable: true,
      headerTestId: "btnSortStaffCode",
      cellTestId: (_, index) => `colStaffCode-${index}`,
      className: "w-16",
    },
    {
      key: "fullName",
      header: "Full Name",
      sortable: true,
      headerTestId: "btnSortFullName",
      cellTestId: (_, index) => `colFullName-${index}`,
      className: "w-40",
    },
    {
      key: "type",
      header: "Type",
      sortable: true,
      headerTestId: "btnSortType",
      cellTestId: (_, index) => `colType-${index}`,
      className: "w-20",
    },
  ];
}
