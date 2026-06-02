"use client";

import { type FormEvent, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import RadioGroup from "@/features/users/components/RadioGroup";
import UserTypeDropdown from "@/features/users/components/UserTypeDropdown";
import {
  useEditUserMutation,
  useGetUserForEditQuery,
} from "@/features/users/users.api";
import {
  Gender,
  UserRoles,
  type GetUserForEditResponse,
} from "@/features/users/users.types";
import { useAppDispatch } from "@/lib/redux/hooks";
import {
  localDateToUtcIso,
  utcDateToLocalDate,
} from "@/utils/datetime.utils";

const genderOptions = [
  { label: "Female", value: Gender.Female },
  { label: "Male", value: Gender.Male },
];

export default function EditUserPage() {
  const searchParams = useSearchParams();
  const userId = searchParams.get("userId") ?? "";

  const { data: user, isLoading } = useGetUserForEditQuery(userId, {
    skip: !userId,
  });

  if (!userId) {
    return (
      <div className="min-h-screen bg-white text-[#333]">
        <main className="min-w-0 flex-1">
          <h2 className="mb-6 text-xl font-bold text-primary">Edit User</h2>
          <p className="text-sm text-gray-700">Missing user id.</p>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white text-[#333]" data-testid="tabManagerUser">
      <div className="flex min-w-0">
        <main className="min-w-0 flex-1">
          <h2 className="mb-6 text-xl font-bold text-primary">Edit User</h2>
          {isLoading && <p className="text-sm text-gray-700">Loading...</p>}
          {!isLoading && user && <EditUserForm userId={userId} user={user} />}
        </main>
      </div>
    </div>
  );
}

function EditUserForm({
  userId,
  user,
}: {
  userId: string;
  user: GetUserForEditResponse;
}) {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const [editUser, { isLoading: isSaving }] = useEditUserMutation();

  const [dateOfBirth, setDateOfBirth] = useState<Date | null>(
    utcDateToLocalDate(user.dateOfBirth),
  );
  const [joinedDate, setJoinedDate] = useState<Date | null>(
    utcDateToLocalDate(user.joinedDate),
  );
  const [selectedGender, setSelectedGender] = useState<Gender>(user.gender);
  const [selectedType, setSelectedType] = useState<UserRoles>(user.userType);

  const redirectToUsers = () => {
    router.push("/admin/users");
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!dateOfBirth || !joinedDate) {
      dispatch(
        enqueueToast({
          message: "Please complete all required fields.",
          type: ToastType.Error,
        }),
      );
      return;
    }

    try {
      await editUser({
        userId,
        dateOfBirth: localDateToUtcIso(dateOfBirth),
        gender: selectedGender,
        joinedDate: localDateToUtcIso(joinedDate),
        type: selectedType,
      }).unwrap();

      dispatch(
        enqueueToast({
          message: "User updated successfully.",
          type: ToastType.Success,
        }),
      );
      redirectToUsers();
    } catch {
      dispatch(
        enqueueToast({
          message: "Failed to update user. Please try again.",
          type: ToastType.Error,
        }),
      );
    }
  };

  return (
    <form className="w-[390px] space-y-3" onSubmit={handleSubmit}>
      <div className="flex items-center gap-5">
        <label className="w-[92px] text-sm text-gray-800">First Name</label>
        <input
          type="text"
          disabled
          value={user.firstName}
          className="h-[33px] w-[265px] rounded border border-gray-400 bg-gray-100 px-3 text-sm text-gray-800 outline-none"
        />
      </div>

      <div className="flex items-center gap-5">
        <label className="w-[92px] text-sm text-gray-800">Last Name</label>
        <input
          type="text"
          disabled
          value={user.lastName}
          className="h-[33px] w-[265px] rounded border border-gray-400 bg-gray-100 px-3 text-sm text-gray-800 outline-none"
        />
      </div>

      <div className="flex items-center gap-5">
        <label className="w-[92px] text-sm text-gray-800">Date of Birth</label>
        <DatePickerInput
          value={dateOfBirth}
          onChange={setDateOfBirth}
          placeholder="Date of Birth"
          width="w-[265px]"
          txtInputTestId="txtDateOfBirth"
        />
      </div>

      <div className="flex items-center gap-5">
        <label className="w-[92px] text-sm text-gray-800">Gender</label>
        <RadioGroup
          name="gender"
          items={genderOptions}
          value={selectedGender}
          getKey={(item) => item.value}
          getLabel={(item) => item.label}
          onChange={(value) => setSelectedGender(value as Gender)}
        />
      </div>

      <div className="flex items-center gap-5">
        <label className="w-[92px] text-sm text-gray-800">Joined Date</label>
        <DatePickerInput
          value={joinedDate}
          onChange={setJoinedDate}
          placeholder="Joined Date"
          width="w-[265px]"
          txtInputTestId="txtJoinedDate"
        />
      </div>

      <div className="flex items-center gap-5">
        <label className="w-[92px] text-sm text-gray-800">Type</label>
        <UserTypeDropdown value={selectedType} onChange={setSelectedType} />
      </div>

      <div className="flex justify-end gap-7 pt-6">
        <button
          type="submit"
          disabled={isSaving}
          className="h-[35px] rounded bg-primary px-5 text-sm font-semibold text-white transition hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isSaving ? "Saving..." : "Save"}
        </button>

        <button
          type="button"
          onClick={redirectToUsers}
          className="h-[35px] rounded border border-gray-400 bg-white px-4 text-sm text-gray-600 transition hover:bg-gray-50"
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
