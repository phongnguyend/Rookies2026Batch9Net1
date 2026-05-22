"use client";

import { useState } from "react";
import DataTable, {
  type ColumnDef,
} from "@/features/shared/components/DataTable";
import Pagination from "@/features/shared/components/Pagination";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import RadioGroup from "@/features/shared/components/RadioGroup";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import { fakeUserSkins, type User } from "@/features/tests/test.types";
import { useGetUsersQuery } from "@/features/tests/test.api";
import SearchInput from "@/features/shared/components/SearchInput";
import ConfirmModal from "@/features/shared/components/Modal/ConfirmModal";
import { useAppDispatch } from "@/lib/redux/hooks";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import Image from "next/image";

export default function TestPage() {
  const dispatch = useAppDispatch();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [selectedSkins, setSelectedSkins] = useState<string[]>([]);
  const [selectedSkin, setSelectedSkin] = useState("black");
  const [createdDate, setCreatedDate] = useState<Date | null>(null);

  // Testing purpose on the yes or no question, or only yes for modal
  const [isAcceptOrDeclineLoading, setAcceptOrDeclineMutation] =
    useState(false);
  const [isYesLoading, setIsYesMutation] = useState(false);
  const [isModalYesAndCancelOpen, setIsModalYesAndCancelOpen] = useState(false);
  const [isModalAcceptAndDeclineOpen, setIsModalAcceptAndDeclineOpen] =
    useState(false);

  const limit = 10;

  const { data, isLoading } = useGetUsersQuery({
    page,
    limit,
    search,
    skins: selectedSkins,
    createdDate,
  });

  const users = data?.items ?? [];

  if (isLoading) {
    return <div>loading...</div>;
  }

  const columns: ColumnDef<User>[] = [
    {
      key: "no",
      header: "No. ▼",
      render: (_user, index) => (page - 1) * limit + index + 1,
    },
    {
      key: "avatar",
      header: "Avatar",
      render: (user) => (
        <Image
          src={user.avatar}
          alt={user.name}
          className="h-9 w-9 rounded-full object-cover"
        />
      ),
    },
    { key: "name", header: "Name ▼" },
    { key: "city", header: "City ▼" },
    { key: "country", header: "Country ▼" },
    { key: "address", header: "Address ▼" },
    { key: "skin", header: "Skin ▼" },
    {
      key: "actions",
      header: "",
      className: "w-[120px]",
      render: (user) => (
        <DataTableButtonActions
          row={user}
          onAccept={(row) => console.log("accept", row)}
          onDecline={(row) => console.log("decline", row)}
          onReturn={(row) => console.log("return", row)}
        />
      ),
    },
  ];

  return (
    <div className="min-h-screen bg-white text-[#333]">
      <div className="flex">
        <main className="flex-1 px-32 pt-24">
          <h2 className="mb-6 text-xl font-bold text-primary">User List</h2>
          {/* Radio Group */}
          <RadioGroup
            label="Skin"
            name="skin"
            items={fakeUserSkins}
            value={selectedSkin}
            getKey={(skin) => skin.skin}
            getLabel={(skin) => skin.skin}
            onChange={(value) => {
              console.log(value);
              setSelectedSkin(value);
            }}
          />

          <div className="mb-6 mt-4 flex items-center justify-between">
            <div className="flex gap-5">
              {/* Dropdown  */}
              <DropdownFilter
                items={fakeUserSkins}
                values={selectedSkins}
                placeholder="Skins"
                getKey={(skin) => skin.skin}
                getLabel={(skin) => skin.skin}
                onChange={(values) => {
                  console.log(values);
                  setSelectedSkins(values);
                  setPage(1);
                }}
              />

              {/* DatePicker */}
              <DatePickerInput
                value={createdDate}
                onChange={(date) => {
                  setCreatedDate(date);
                  setPage(1);
                }}
                placeholder="Created Date"
              />
            </div>

            <div className="flex gap-8">
              {/* Search Input */}
              <SearchInput
                value={search}
                placeholder="Search..."
                onChange={(value) => {
                  setSearch(value);
                }}
                onSearch={(value) => {
                  console.log("Click Search ne...", value);
                  setSearch(value);
                  setPage(1);
                }}
              />

              <button className="rounded bg-primary px-5 py-2 font-semibold text-white">
                Create new user
              </button>

              <button
                onClick={() => setIsModalYesAndCancelOpen(true)}
                className="rounded border border-primary px-5 py-2 font-semibold text-primary hover:bg-primary/5 transition-colors duration-150"
              >
                Test Yes and Cancel
              </button>

              <button
                onClick={() => setIsModalAcceptAndDeclineOpen(true)}
                className="rounded border border-primary px-5 py-2 font-semibold text-primary hover:bg-primary/5 transition-colors duration-150"
              >
                Test Yes and No
              </button>

              {/* Toast Tests */}
              <button
                onClick={() =>
                  dispatch(
                    enqueueToast({
                      message: "Success! User created successfully.",
                      type: ToastType.Success,
                    }),
                  )
                }
                className="rounded bg-success px-4 py-2 font-semibold text-white hover:bg-success/90 transition-all duration-150 active:scale-95"
              >
                Success Toast
              </button>

              <button
                onClick={() =>
                  dispatch(
                    enqueueToast({
                      message: "Error! Failed to delete user.",
                      type: ToastType.Error,
                    }),
                  )
                }
                className="rounded bg-error px-4 py-2 font-semibold text-white hover:bg-error/90 transition-all duration-150 active:scale-95"
              >
                Error Toast
              </button>

              <button
                onClick={() =>
                  dispatch(
                    enqueueToast({
                      message: "Warning! Please double check your input.",
                      type: ToastType.Warning,
                    }),
                  )
                }
                className="rounded bg-warning px-4 py-2 font-semibold text-warning-content hover:bg-warning/90 transition-all duration-150 active:scale-95"
              >
                Warning Toast
              </button>

              <button
                onClick={() =>
                  dispatch(
                    enqueueToast({
                      message: "Info: Account status updated.",
                      type: ToastType.Info,
                    }),
                  )
                }
                className="rounded bg-info px-4 py-2 font-semibold text-info-content hover:bg-info/90 transition-all duration-150 active:scale-95"
              >
                Info Toast
              </button>
            </div>
          </div>

          {/* Data Table */}
          <DataTable<User>
            data={users}
            columns={columns}
            isLoading={isLoading}
            emptyMessage="No users found."
          />

          {/* Pagination */}
          <Pagination
            pageNumber={data?.pageNumber ?? page}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? limit}
            totalCount={data?.totalCount ?? 0}
            hasPreviousPage={data?.hasPreviousPage ?? false}
            hasNextPage={data?.hasNextPage ?? false}
            onPageChange={setPage}
          />
        </main>
      </div>

      {/* Example of Do you want to returning request*/}
      <ConfirmModal
        isOpen={isModalAcceptAndDeclineOpen}
        onClose={() => setIsModalAcceptAndDeclineOpen(false)}
        title="Are you sure?"
        body={
          <div>
            <p className="mb-2">
              Do you want to create a returning request for this asset
            </p>
            <p className="text-sm text-red-500 font-semibold">
              Warning: This action is permanent and cannot be undone.
            </p>
          </div>
        }
        yesButtonLabel="Accept"
        noButtonLabel="Decline"
        isLoading={isAcceptOrDeclineLoading}
        onYes={() => {
          setAcceptOrDeclineMutation(true);
          setTimeout(() => {
            setAcceptOrDeclineMutation(false);
            setIsModalAcceptAndDeclineOpen(false);
            console.log("Accept returning successfully!");
          }, 1500);
        }}
        onNo={() => {
          setAcceptOrDeclineMutation(true);
          setTimeout(() => {
            setAcceptOrDeclineMutation(false);
            setIsModalAcceptAndDeclineOpen(false);
            console.log("Decline retunring successfully!");
          }, 1500);
        }}
      />

      {/* Example of deleting a user showing modal*/}
      <ConfirmModal
        isOpen={isModalYesAndCancelOpen}
        onClose={() => setIsModalYesAndCancelOpen(false)}
        title="Delete User"
        body={
          <div>
            <p className="mb-2">Are you sure you want to delete this user?</p>
            <p className="text-sm text-red-500 font-semibold">
              Warning: This action is permanent and cannot be undone.
            </p>
          </div>
        }
        yesButtonLabel="Delete"
        isLoading={isYesLoading}
        onYes={() => {
          setIsYesMutation(true);
          setTimeout(() => {
            setIsYesMutation(false);
            setIsModalYesAndCancelOpen(false);
            console.log("User deleted successfully!");
          }, 1500);
        }}
      />
    </div>
  );
}
