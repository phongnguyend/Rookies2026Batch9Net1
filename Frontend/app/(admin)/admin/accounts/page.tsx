"use client";

import { useCallback, useEffect, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
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

export default function AccountsPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const pageParam = Number(searchParams.get("page") ?? "1");
  const queryPage = Number.isNaN(pageParam) || pageParam < 1 ? 1 : pageParam;
  const querySearch = searchParams.get("search") ?? "";
  const typeParam = searchParams.get("type");
  const selectedType =
    typeParam === AccountRole.Admin || typeParam === AccountRole.Staff
      ? typeParam
      : "All";

  const [searchInput, setSearchInput] = useState(querySearch);
  const [sorts, setSorts] = useState<SortItem[]>([]);

  useEffect(() => {
    setSearchInput(querySearch);
  }, [querySearch]);

  useEffect(() => {
    const sortByParam = searchParams.get("sortBy") ?? "";
    const sortDescParam = searchParams.get("sortDesc");

    if (!sortByParam) {
      setSorts([]);
      return;
    }

    setSorts([
      {
        key: sortByParam,
        direction:
          sortDescParam === "true" ? SortDirection.Desc : SortDirection.Asc,
      },
    ]);
  }, [searchParams]);

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
      router.replace(queryString ? `?${queryString}` : "/admin/accounts");
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
    setSorts(newSorts);

    if (newSorts.length === 0) {
      updateQueryParams({ sortBy: null, sortDesc: null });
      return;
    }

    const firstSort = newSorts[0];
    updateQueryParams({
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
      render: (user) => (
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
    <div className="min-h-screen bg-slate-50 px-4 py-6 text-slate-900 sm:px-6 lg:px-10">
      <div className="mx-auto w-full max-w-6xl rounded-3xl bg-white p-6 shadow-sm">
        <div className="mb-6">
          <h1 className="text-3xl font-semibold tracking-tight text-red-600">User List</h1>
        </div>

        <div className="mb-4 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <DropdownFilter
            items={typeFilters}
            values={selectedType === "All" ? [] : [selectedType]}
            placeholder="All"
            width="w-40"
            getKey={(item) => item.id}
            getLabel={(item) => item.label}
            onChange={(values) => {
              const nextType = values.length === 1 ? (values[0] as AccountRole) : "All";
              updateQueryParams({ page: 1, type: nextType !== "All" ? nextType : null });
            }}
            allLabel="All"
          />

          <div className="flex flex-wrap items-center gap-3">
            <SearchInput
              value={searchInput}
              placeholder="Search users..."
              width="w-72"
              onChange={(value) => setSearchInput(value)}
              onSearch={(value) => {
                updateQueryParams({ page: 1, search: value });
              }}
            />
            <button
              type="button"
              onClick={() => {
                // TODO: navigate to create user page, e.g. router.push('/admin/users/create')
              }}
              className="inline-flex items-center justify-center border rounded-md border-red-200 bg-red-600 px-5 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-red-700"
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
            onSortChange={handleSortChange}
            isLoading={isLoading}
            emptyMessage="No users found."
          />
        </div>

        <div className="mt-6">
          <Pagination
            pageNumber={data?.pageNumber ?? queryPage}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? pageSize}
            totalCount={data?.totalCount ?? users.length}
            hasPreviousPage={Boolean(data?.hasPreviousPage)}
            hasNextPage={Boolean(data?.hasNextPage)}
            onPageChange={(nextPage) => updateQueryParams({ page: nextPage })}
          />
        </div>
      </div>
    </div>
  );
}
