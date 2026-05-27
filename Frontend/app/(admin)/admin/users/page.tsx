"use client";

import {
  type MouseEvent,
  type ReactNode,
  useCallback,
  useState,
} from "react";
import { useRouter, useSearchParams } from "next/navigation";
import DataTable, {
  type ColumnDef,
  type SortItem,
} from "@/features/users/components/DataTable";
import DropdownFilter from "@/features/users/components/DropdownFilter";
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

function ActionIconButton({
  label,
  children,
  className = "text-slate-700 hover:text-primary",
  disabled = false,
  testId,
  onClick,
}: {
  label: string;
  children: ReactNode;
  className?: string;
  disabled?: boolean;
  testId?: string;
  onClick: (event: MouseEvent<HTMLButtonElement>) => void;
}) {
  return (
    <span className="group relative inline-flex h-8 w-8 items-center justify-center">
      <button
        type="button"
        aria-label={label}
        disabled={disabled}
        onClick={onClick}
        className={`inline-flex h-8 w-8 items-center justify-center bg-transparent p-0 shadow-none outline-none transition disabled:cursor-not-allowed disabled:opacity-35 ${className}`}
        data-testid={testId}
      >
        {children}
      </button>
      <span className="pointer-events-none absolute -top-8 left-1/2 z-10 -translate-x-1/2 whitespace-nowrap rounded bg-neutral px-2 py-1 text-xs font-medium text-neutral-content opacity-0 shadow-sm transition-opacity group-hover:opacity-100 group-focus-within:opacity-100">
        {label}
      </span>
    </span>
  );
}

function EditIcon() {
  return (
    <svg
      aria-hidden="true"
      viewBox="0 0 24 24"
      className="h-5 w-5"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M14.75 4.25 19.75 9.25" />
      <path d="M18.25 2.75a2.12 2.12 0 0 1 3 3L8.5 18.5 3.75 20.25 5.5 15.5 18.25 2.75Z" />
      <path d="M5.5 15.5 8.5 18.5" />
    </svg>
  );
}

function DisableIcon() {
  return (
    <svg
      aria-hidden="true"
      viewBox="0 0 24 24"
      className="h-5 w-5"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <circle cx="12" cy="12" r="8" />
      <path d="M9 9 15 15" />
      <path d="M15 9 9 15" />
    </svg>
  );
}

export default function UsersPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const pageParam = Number(searchParams.get("page") ?? "1");
  const queryPage = Number.isNaN(pageParam) || pageParam < 1 ? 1 : pageParam;
  const querySearch = searchParams.get("search") ?? "";
  const sortByParam = searchParams.get("sortBy") ?? "";
  const sortDescParam = searchParams.get("sortDesc");
  const typeParam = searchParams.get("type");

  // The type filter is stored in the URL as comma-separated values.
  // Invalid values are dropped so only supported user roles reach the UI/API.
  const selectedTypes = (typeParam?.split(",") ?? []).filter(
    (type): type is UserRoles =>
      type === UserRoles.Admin || type === UserRoles.Staff,
  );
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
      router.replace(queryString ? `?${queryString}` : "/admin/users", {
        scroll: false,
      });
    },
    [router, searchParams],
  );

  const queryParams: GetUsersRequest = {
    pageNumber: queryPage,
    pageSize,
    ...(querySearch ? { searchTerm: querySearch } : {}),

    // Backend currently accepts a single type value. When both roles are
    // selected, the dropdown normalizes to "All", so no type filter is sent.
    ...(selectedTypes.length === 1 ? { type: selectedTypes[0] } : {}),
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
    {
      key: "staffCode",
      header: "Staff Code",
      sortable: true,
      headerTestId: "lblStaffCode",
    },
    {
      key: "fullName",
      header: "Full Name",
      sortable: true,
      headerTestId: "lblFullName",
    },
    {
      key: "userName",
      header: "Username",
      sortable: true,
      headerTestId: "lblUsername",
    },
    {
      key: "joinedDate",
      header: "Joined Date",
      sortable: true,
      headerTestId: "lblJoinedDate",
    },
    {
      key: "userType",
      header: "Type",
      sortable: true,
      headerTestId: "lblUserType",
    },
    {
      key: "actions",
      header: "",
      className: "w-[72px] text-left",
      render: (user) => (
        <div className="flex justify-start gap-2">
          <ActionIconButton
            label="Edit"
            testId="btnEditUser"
            onClick={(event) => {
              event.stopPropagation();
              // Navigate to edit user page for user.id
            }}
          >
            <EditIcon />
          </ActionIconButton>

          <ActionIconButton
            label="Disable"
            className="text-red-700 hover:text-red-800"
            disabled={!user.canBeDisabled}
            testId="btnDisableUser"
            onClick={(event) => {
              event.stopPropagation();
              if (!user.canBeDisabled) {
                return;
              }

              // Disable user logic goes here
            }}
          >
            <DisableIcon />
          </ActionIconButton>
        </div>
      ),
    },
  ];

  return (
    <div className="min-h-screen bg-white text-[#333]" data-testid="tabManagerUser">
      <div className="flex">
        <main className="flex-1">
          <h2 className="mb-6 text-xl font-bold text-primary">User List</h2>

          <div className="mb-6 mt-4 flex items-center justify-between">
            <div className="flex gap-5">
              <div data-testid="ddlFilterType">
                <DropdownFilter
                  items={typeFilters}
                  values={selectedTypes}
                  placeholder="Type"
                  width="w-40"
                  getKey={(item) => item.id}
                  getLabel={(item) => item.label}
                  onChange={(values) => {
                    // Empty values mean "All"; otherwise keep selected roles in
                    // the URL so refresh/back navigation preserves the filter.
                    updateQueryParams({
                      page: 1,
                      type: values.length > 0 ? values.join(",") : null,
                    });
                  }}
                  allLabel="All"
                  getTestIdAll={() => "ddlFilterAll"}
                  getTestId={(item) =>
                    item.id === UserRoles.Admin
                      ? "ddlFilterAdmin"
                      : "ddlFilterStaff"
                  }
                />
              </div>
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
                txtInputTestId="txtSearchUser"
                btnSearchTestId="btnSearchUser"
              />

              <button
                type="button"
                data-testid="btnCreateUser"
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
              tableTestId="dgdUserList"
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
            btnPreviousPageTestId="btnPrevPage"
            btnNextPageTestId="btnNextPage"
          />
        </main>
      </div>
    </div>
  );
}
