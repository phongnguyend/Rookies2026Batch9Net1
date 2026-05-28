"use client";

import { UserAssignmentsDataTable } from "@/features/assignments/user/components/data-table";

export default function AdminHomePage() {
  return (
    <div data-testid="mnuMyAssignment">
      <div className="mb-6 text-xl font-bold text-primary">My Assignment</div>
      <UserAssignmentsDataTable></UserAssignmentsDataTable>
    </div>
  );
}
