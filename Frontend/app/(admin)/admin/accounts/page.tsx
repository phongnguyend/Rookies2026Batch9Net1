"use client";

import { useState } from "react";
import DataTable, { type ColumnDef, type SortItem } from "@/features/shared/components/DataTable";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import { SortDirection } from "@/lib/api/base.types";
import { useGetUsersQuery } from "@/features/accounts/accounts.api";
import { AccountRole, type GetUsersRequest, type UserRow } from "@/features/accounts/accounts.types";

const typeFilters = [
  { id: AccountRole.Admin, label: "Admin" },
  { id: AccountRole.Staff, label: "Staff" },
];

const pageSize = 10;

type FilterType = AccountRole | "All";

export default function AccountsPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [selectedType, setSelectedType] = useState<FilterType>("All");
  const [sorts, setSorts] = useState<SortItem[]>([]);

  const queryParams: GetUsersRequest = {
    pageNumber: page,
    pageSize,
    ...(search ? { searchTerm: search } : {}),
    ...(selectedType !== "All" ? { type: selectedType } : {}),
    ...(sorts[0]?.key ? { sortBy: sorts[0]!.key } : {}),
    ...(sorts[0]?.direction === SortDirection.Desc ? { sortDesc: true } : {}),
  };

  const { data, isLoading } = useGetUsersQuery(queryParams);
  const users = data?.items ?? [];

  const columns: ColumnDef<UserRow>[] = [
    { key: "staffCode", header: "Staff Code", sortable: true },
    { key: "fullName", header: "Full Name", sortable: true },
    { key: "userName", header: "Username", sortable: true },
    { key: "joinedDate", header: "Joined Date", sortable: true },
    { key: "userType", header: "Type", sortable: true },
    {
      key: "actions",
      header: "",
      className: "w-[140px] text-right",
      render: (user) => (
        <div className="flex justify-end gap-2">
          <button
            type="button"
            title="Edit"
            className="inline-flex h-9 w-9 items-center justify-center rounded-md text-slate-700 transition hover:bg-slate-100"
          >
            ✎
          </button>
          <button
            type="button"
            title="Disable"
            className="inline-flex h-9 w-9 items-center justify-center rounded-md text-red-700 transition hover:bg-red-100"
          >
            ⊗
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="min-h-screen bg-white text-[#333]">
      <div className="flex">
        <main className="flex-1 px-32 pt-24">
          <h2 className="mb-6 text-xl font-bold text-primary">User List</h2>

          <div className="mb-6 mt-4 flex items-center justify-between">
            <div className="flex gap-5 items-center">
              <DropdownFilter
                items={typeFilters}
                values={selectedType === "All" ? [] : [selectedType]}
                placeholder="Type"
                width="w-40"
                getKey={(item) => item.id}
                getLabel={(item) => item.label}
                onChange={(values) => {
                  const nextType = values.length === 1 ? (values[0] as AccountRole) : "All";
                  setSelectedType(nextType);
                  setPage(1);
                }}
                allLabel="All"
              />
            </div>

            <div className="flex items-center gap-4">
              <SearchInput
                value={search}
                placeholder="Search users..."
                onChange={(value) => setSearch(value)}
                onSearch={(value) => {
                  setSearch(value);
                  setPage(1);
                }}
              />
              <button
                type="button"
                className="rounded bg-primary px-5 py-2 font-semibold text-white"
              >
                Create new user
              </button>
            </div>
          </div>

          <div className="overflow-x-auto bg-white">
            <DataTable
              data={users}
              columns={columns}
              sorts={sorts}
              onSortChange={(newSorts) => setSorts(newSorts)}
              isLoading={isLoading}
              emptyMessage="No users found."
            />
          </div>

          <Pagination
            pageNumber={data?.pageNumber ?? page}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? pageSize}
            totalCount={data?.totalCount ?? users.length}
            hasPreviousPage={Boolean(data?.hasPreviousPage)}
            hasNextPage={Boolean(data?.hasNextPage)}
            onPageChange={(nextPage) => setPage(nextPage)}
          />
        </main>
      </div>
    </div>
  );
}
