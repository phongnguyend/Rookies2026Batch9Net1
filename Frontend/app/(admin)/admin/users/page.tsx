"use client";

import { useCallback, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import DataTable, {
  type ColumnDef,
  type SortItem,
} from "@/features/shared/components/DataTable";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import { SortDirection } from "@/lib/api/base.types";
import UserDetailModal from "@/features/users/components/UserDetailModal";
import { useGetUsersQuery } from "@/features/users/users.api";
import {
  UserRoles,
  type GetUsersRequest,
  type UserRow,
} from "@/features/users/users.types";

const typeFilters = [
  { id: UserRoles.Admin, label: "Admin" },
  { id: UserRoles.Staff, label: "Staff" },
];

const pageSize = 10;

export default function UsersPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const pageParam = Number(searchParams.get("page") ?? "1");
  const queryPage = Number.isNaN(pageParam) || pageParam < 1 ? 1 : pageParam;
  const querySearch = searchParams.get("search") ?? "";
  const sortByParam = searchParams.get("sortBy") ?? "";
  const sortDescParam = searchParams.get("sortDesc");
  const typeParam = searchParams.get("type");
  const selectedType =
    typeParam === UserRoles.Admin || typeParam === UserRoles.Staff
      ? typeParam
      : "All";
  const sorts: SortItem[] = sortByParam
    ? [
        {
          key: sortByParam,
          direction:
            sortDescParam === "true" ? SortDirection.Desc : SortDirection.Asc,
        },
      ]
    : [];

  const [searchState, setSearchState] = useState({
    inputValue: querySearch,
    urlValue: querySearch,
  });
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const searchInput =
    searchState.urlValue === querySearch ? searchState.inputValue : querySearch;

  const updateQueryParams = useCallback(
    (
      params: Partial<{
        page: number;
        search: string;
        type: string | null;
        sortBy: string | null;
        sortDesc: boolean | null;
      }>,
    ) => {
      const nextSearchParams = new URLSearchParams(searchParams.toString());

      if (params.page !== undefined) {
        if (params.page > 1) {
          nextSearchParams.set("page", String(params.page));
        } else {
          nextSearchParams.delete("page");
        }
      }

      if (params.search !== undefined) {
        const value = params.search.trim();
        if (value) {
          nextSearchParams.set("search", value);
        } else {
          nextSearchParams.delete("search");
        }
      }

      if (params.type !== undefined) {
        if (params.type && params.type !== "All") {
          nextSearchParams.set("type", params.type);
        } else {
          nextSearchParams.delete("type");
        }
      }

      if (params.sortBy !== undefined) {
        if (params.sortBy) {
          nextSearchParams.set("sortBy", params.sortBy);
        } else {
          nextSearchParams.delete("sortBy");
        }
      }

      if (params.sortDesc !== undefined) {
        if (params.sortDesc) {
          nextSearchParams.set("sortDesc", String(params.sortDesc));
        } else {
          nextSearchParams.delete("sortDesc");
        }
      }

      const queryString = nextSearchParams.toString();
      router.replace(queryString ? `?${queryString}` : "/admin/users");
    },
    [router, searchParams],
  );

  const queryParams: GetUsersRequest = {
    pageNumber: queryPage,
    pageSize,
    ...(querySearch ? { searchTerm: querySearch } : {}),
    ...(selectedType !== "All" ? { type: selectedType } : {}),
    ...(sorts[0]?.key ? { sortBy: sorts[0]!.key } : {}),
    ...(sorts[0]?.direction === SortDirection.Desc ? { sortDesc: true } : {}),
  };

  const { data, isLoading } = useGetUsersQuery(queryParams);
  const users = data?.items ?? [];

  const handleSortChange = (newSorts: SortItem[]) => {
    if (newSorts.length === 0) {
      updateQueryParams({ page: 1, sortBy: null, sortDesc: null });
      return;
    }

    const firstSort = newSorts[0];
    updateQueryParams({
      page: 1,
      sortBy: firstSort.key,
      sortDesc: firstSort.direction === SortDirection.Desc,
    });
  };

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
      render: () => (
        <div className="flex justify-end gap-2">
          <button
            type="button"
            onClick={(event) => {
              event.stopPropagation();
              // Navigate to edit user page for user.id
            }}
            title="Edit"
            className="inline-flex h-9 w-9 items-center justify-center rounded-md bg-white text-slate-700 transition hover:bg-slate-100"
          >
            ✎
          </button>

          <button
            type="button"
            onClick={(event) => {
              event.stopPropagation();
              // Disable user logic goes here
            }}
            title="Disable"
            className="inline-flex h-9 w-9 items-center justify-center rounded-md bg-red-50 text-red-700 transition hover:bg-red-100"
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
            <div className="flex gap-5">
              <DropdownFilter
                items={typeFilters}
                values={selectedType === "All" ? [] : [selectedType]}
                placeholder="Type"
                width="w-40"
                getKey={(item) => item.id}
                getLabel={(item) => item.label}
                onChange={(values) => {
                  const nextType =
                    values.length === 1 ? (values[0] as UserRoles) : "All";
                  updateQueryParams({
                    page: 1,
                    type: nextType !== "All" ? nextType : null,
                  });
                }}
                allLabel="All"
              />
            </div>

            <div className="flex gap-8">
              <SearchInput
                value={searchInput}
                placeholder="Search..."
                onChange={(value) =>
                  setSearchState({ inputValue: value, urlValue: querySearch })
                }
                onSearch={(value) => {
                  const nextSearch = value.trim();
                  setSearchState({
                    inputValue: nextSearch,
                    urlValue: nextSearch,
                  });
                  updateQueryParams({ page: 1, search: nextSearch });
                }}
              />

              <button
                type="button"
                onClick={() => {
                  // TODO: navigate to create user page, e.g. router.push('/admin/users/create')
                }}
                className="rounded bg-primary px-5 py-2 font-semibold text-white"
              >
                Create new user
              </button>
            </div>
          </div>

          <div className="relative">
            <DataTable
              data={users}
              columns={columns}
              sorts={sorts}
              onSortChange={handleSortChange}
              onRowClick={(user) => setSelectedUserId(user.id)}
              isLoading={isLoading}
              emptyMessage="No users found."
            />

            <UserDetailModal
              userId={selectedUserId}
              isOpen={selectedUserId !== null}
              onClose={() => setSelectedUserId(null)}
            />
          </div>

          <Pagination
            pageNumber={data?.pageNumber ?? queryPage}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? pageSize}
            totalCount={data?.totalCount ?? users.length}
            hasPreviousPage={Boolean(data?.hasPreviousPage)}
            hasNextPage={Boolean(data?.hasNextPage)}
            onPageChange={(nextPage) => updateQueryParams({ page: nextPage })}
          />
        </main>
      </div>
    </div>
  );
}
