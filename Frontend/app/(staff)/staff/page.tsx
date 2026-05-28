"use client";

import { UserAssignmentsDataTable } from "@/features/assignments/user/components/data-table";

export default function StaffHomePage() {
  return (
    <div data-testid="mnuMyAssignment">
      <div className="mb-6 text-xl font-bold text-primary">My Assignment</div>
      <UserAssignmentsDataTable></UserAssignmentsDataTable>
    </div>
  );
}
