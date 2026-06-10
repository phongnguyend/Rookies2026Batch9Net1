"use client";

import { type ReactNode, useCallback, useEffect, useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import SingleSortDataTable, {
  type ColumnDef,
  type SortItem,
} from "@/features/shared/components/SingleSortDataTable";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import { SortDirection } from "@/lib/api/base.types";
import UserDetailModal from "@/features/users/components/UserDetailModal";
import {
  useGetUsersQuery,
  useLazyCanDisableUserQuery,
  useDisableUserMutation,
} from "@/features/users/users.api";
import {
  UserRoles,
  type GetUsersRequest,
  type UserRow,
} from "@/features/users/users.types";
import { useAppDispatch, useAppSelector } from "@/lib/redux/hooks";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import ConfirmModal from "@/features/shared/components/Modal/ConfirmModal";
import { formatDate } from "@/utils/datetime.utils";
import { CircleX, Pencil } from "lucide-react";
import SingleSelectDropdown from "@/features/shared/components/SingleSelectDropdown";

const typeFilters = [
  { id: UserRoles.Admin, label: "Admin" },
  { id: UserRoles.Staff, label: "Staff" },
];

const pageSize = 10;
const defaultSort: SortItem = {
  key: "staffCode",
  direction: SortDirection.Asc,
};

function UserActionButton({
  title,
  testId,
  disabled = false,
  className,
  children,
  onClick,
}: {
  title: string;
  testId: string;
  disabled?: boolean;
  className: string;
  children: ReactNode;
  onClick: () => void;
}) {
  return (
    <button
      type="button"
      disabled={disabled}
      onClick={(event) => {
        event.stopPropagation();
        onClick();
      }}
      data-testid={testId}
      className={`${className} cursor-pointer disabled:cursor-not-allowed disabled:opacity-30`}
      title={title}
    >
      {children}
    </button>
  );
}

export default function UsersPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pathname = usePathname();

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

  const [temporarySort, setTemporarySort] = useState<string | null>(() => {
    if (typeof window === "undefined") {
      return null;
    }

    return sessionStorage.getItem("usersTemporarySort");
  });

  const sorts: SortItem[] = sortByParam
    ? [
        {
          key: sortByParam,
          direction:
            sortDescParam === "true" ? SortDirection.Desc : SortDirection.Asc,
        },
      ]
    : [defaultSort];

  const displayedSorts = temporarySort ? [] : sorts;

  const [searchState, setSearchState] = useState({
    inputValue: querySearch,
    urlValue: querySearch,
  });
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [disablingUserId, setDisablingUserId] = useState<string | null>(null);
  const [showCanDisableModal, setShowCanDisableModal] =
    useState<boolean>(false);
  const [showConfirmDisableModal, setShowConfirmDisableModal] =
    useState<boolean>(false);

  const dispatch = useAppDispatch();
  const currentUser = useAppSelector((state) => state.authSlice.user);
  const [triggerCanDisable] = useLazyCanDisableUserQuery();
  const [disableUser, { isLoading: isDisabling }] = useDisableUserMutation();

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
    ...(selectedTypes.length === 1 ? { type: selectedTypes[0] } : {}),
    sortBy:
      temporarySort === "createdDateDesc"
        ? "createdDate"
        : temporarySort === "updatedDateDesc"
          ? "updatedDate"
          : (sorts[0]?.key ?? defaultSort.key),
    ...(temporarySort || sorts[0]?.direction === SortDirection.Desc
      ? { sortDesc: true }
      : {}),
  };

  const { data, isLoading } = useGetUsersQuery(queryParams);
  const users = data?.items ?? [];

  const handleSortChange = (newSorts: SortItem[]) => {
    setTemporarySort(null);
    sessionStorage.removeItem("usersTemporarySort");

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

  useEffect(() => {
    if (temporarySort) {
      sessionStorage.removeItem("usersTemporarySort");
    }
  }, [temporarySort]);

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
      render: (user) => (user.joinedDate ? formatDate(user.joinedDate) : ""),
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
      className: "w-[96px] text-left !overflow-visible",
      render: (user) => {
        const isSelf =
          user.userName.toLowerCase() === currentUser?.username?.toLowerCase();
        return (
          <div className="flex items-center gap-3">
            <UserActionButton
              title="Edit"
              testId="btnEditUser"
              className="text-green-600"
              onClick={() => {
                router.push(
                  `/admin/users/edit?id=${encodeURIComponent(user.id)}`,
                );
              }}
            >
              <Pencil className="text-gray-500" size={20} strokeWidth={3} />
            </UserActionButton>

            <div
              className={isSelf ? "tooltip tooltip-left" : ""}
              data-tip={isSelf ? "You cannot disable your self" : undefined}
            >
              <UserActionButton
                title="Disable"
                testId="btnDisableUser"
                disabled={isSelf}
                className="text-red-400"
                onClick={async () => {
                  try {
                    setDisablingUserId(user.id);
                    const result = await triggerCanDisable({
                      targetUserId: user.id,
                    }).unwrap();
                    if (result.canDisable) {
                      setShowConfirmDisableModal(true);
                    } else {
                      setShowCanDisableModal(true);
                    }
                  } catch (err) {
                    console.error(
                      "Failed to check if user can be disabled:",
                      err,
                    );
                    dispatch(
                      enqueueToast({
                        message:
                          "Cannoy disable user right now. Please try again.",
                        type: ToastType.Error,
                      }),
                    );
                    setDisablingUserId(null);
                  }
                }}
              >
                <CircleX size={20} strokeWidth={3} />
              </UserActionButton>
            </div>
          </div>
        );
      },
    },
  ];

  const currentUrl = `${pathname}?${searchParams.toString()}`;

  return (
    <div className="mb-10" data-testid="tabManagerUser">
      <div className="flex min-w-0">
        <main className="min-w-0 flex-1">
          <h2 className="mb-6 text-xl font-bold text-primary">User List</h2>

          <div className="my-4 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
            <div className="relative" data-testid="ddlFilterType">
              <SingleSelectDropdown
                  items={typeFilters}
                  value={selectedTypes[0]}
                  placeholder="Type"
                  getKey={(item) => item.id}
                  getLabel={(item) => item.label}
                  onChange={(value) => {
                    updateQueryParams({
                      page: 1,
                      type: value ?? null,
                    });
                  }}
                  getTestIdAll="ddlFilterAll"
                  getTestId={(item) =>
                    item.id === UserRoles.Admin ? "ddlFilterAdmin" : "ddlFilterStaff"
                  }
                />
            </div>


            <div className="flex flex-col gap-3 sm:flex-row sm:items-center lg:gap-3">
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
                onClick={() =>
                  router.push(
                    `/admin/users/create?returnUrl=${encodeURIComponent(currentUrl)}`,
                  )
                }
                className="h-9 hover:bg-red-600 w-full sm:w-64 not-first:rounded bg-primary px-5 py-2 font-semibold text-white whitespace-nowrap text-sm sm:text-base cursor-pointer"
              >
                Create new user
              </button>
            </div>
          </div>

          <div className="relative">
            <div data-testid="dgdUserList">
              <SingleSortDataTable
                data={users}
                columns={columns}
                sorts={displayedSorts}
                onSortChange={handleSortChange}
                onRowClick={(user) => setSelectedUserId(user.id)}
                isLoading={isLoading}
                emptyMessage="No users found."
              />
            </div>

            <UserDetailModal
              userId={selectedUserId}
              isOpen={selectedUserId !== null}
              onClose={() => setSelectedUserId(null)}
            />

            {/* Warn when cannot disable */}
            <ConfirmModal
              isOpen={showCanDisableModal}
              onClose={() => {
                setShowCanDisableModal(false);
                setDisablingUserId(null);
              }}
              title="Can not disable user"
              body={
                <>
                  There are valid assignments belonging to this user.
                  <br />
                  Please close all assignments before disabling user.
                </>
              }
              modalTestId="dlgCanNotDisableUser"
              closeBtnTestId="btnCloseDisablePopup"
            />

            {/* Confirm disable */}
            <ConfirmModal
              isOpen={showConfirmDisableModal}
              onClose={() => {
                setShowConfirmDisableModal(false);
                setDisablingUserId(null);
              }}
              onYes={async () => {
                if (!disablingUserId) return;
                try {
                  await disableUser({ targetUserId: disablingUserId }).unwrap();
                  dispatch(
                    enqueueToast({
                      message: "User was disabled successfully.",
                      type: ToastType.Success,
                    }),
                  );
                } catch (err) {
                  console.error("Failed to disable user:", err);
                  dispatch(
                    enqueueToast({
                      message: "Failed to disable user.",
                      type: ToastType.Error,
                    }),
                  );
                } finally {
                  setShowConfirmDisableModal(false);
                  setDisablingUserId(null);
                }
              }}
              title="Are you sure?"
              body="Do you want to disable this user?"
              yesButtonLabel="Disable"
              noButtonLabel="Cancel"
              isLoading={isDisabling}
              modalTestId="dlgConfirmDisableUser"
              confirmBtnTestId="btnConfirmDisableUser"
              cancelBtnTestId="btnCancelDisableUser"
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
