import { UserAssignmentsDataTable } from "@/features/assignments/user/components/data-table";

export default function StaffHomePage() {
  return (
    <>
      <div className="text-lg font-bold text-primary mb-2">My Assignment</div>
      <UserAssignmentsDataTable></UserAssignmentsDataTable>
    </>
  );
}
