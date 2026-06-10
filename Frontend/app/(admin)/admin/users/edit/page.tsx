"use client";

import {
  type FormEvent,
  type ReactNode,
  useState,
} from "react";
import { useRouter, useSearchParams } from "next/navigation";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import RadioGroup from "@/features/users/components/RadioGroup";
import UserTypeDropdown from "@/features/users/components/UserTypeDropdown";
import type { ApiErrorResponse } from "@/lib/api/base.types";
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
import EntityNotFound from "@/features/shared/components/EntityNotFound";
import LoadingSpinner from "@/features/shared/components/LoadingSpinner";

const genderOptions = [
  { label: "Female", value: Gender.Female },
  { label: "Male", value: Gender.Male },
];

type FieldErrors = Partial<Record<"dateOfBirth" | "gender" | "joinedDate" | "type", string>>;
type RequiredDateField = "dateOfBirth" | "joinedDate";

const editUserValidationMessages = {
  dateOfBirthRequired: "Date of Birth is required",
  dateOfBirthUnder18: "User is under 18. Please select a different date",
  dateOfBirthOver90: "User age must not exceed 90 years.",
  dateOfBirthInFuture:
    "Date of birth cannot be in the future. Please select a different date",
  joinedDateRequired: "Joined Date is required",
  joinedDateNotLaterThanDateOfBirth:
    "Joined date is not later than Date of Birth. Please select a different date",
  joinedDateUnder18:
    "User must be at least 18 years old at the Joined Date",
  joinedDateWeekend:
    "Joined date is Saturday or Sunday. Please select a different date",
  invalidGender: "Invalid gender. Please select a valid gender",
  invalidType: "Invalid type. Please select a valid type",
};

const validGenderValues = new Set<string>(Object.values(Gender));
const validUserTypeValues = new Set<string>(Object.values(UserRoles));

const serverFieldMap: Record<string, keyof FieldErrors> = {
  dateofbirth: "dateOfBirth",
  gender: "gender",
  joineddate: "joinedDate",
  type: "type",
  usertype: "type",
};

const isApiErrorResponse = (error: unknown): error is ApiErrorResponse =>
  typeof error === "object" &&
  error !== null &&
  "status" in error;

const differentLocationMessage =
  "You are not allowed to edit the information of users in a different location.";

const getEditUserErrorMessage = (error: ApiErrorResponse) =>
  error.detail.includes("has a different location")
    ? differentLocationMessage
    : error.detail;

const getFieldName = (field: string): keyof FieldErrors | undefined => {
  const normalizedField = field.replace(/[^a-z]/gi, "").toLowerCase();

  return Object.entries(serverFieldMap).find(([serverField]) =>
    normalizedField.endsWith(serverField),
  )?.[1];
};

const getFieldMessage = (messages: unknown): string | undefined => {
  if (Array.isArray(messages)) {
    return messages
      .filter((message): message is string => typeof message === "string")
      .join(" ");
  }

  if (typeof messages === "string") {
    return messages;
  }

  return undefined;
};

const getValidationErrors = (error: ApiErrorResponse): FieldErrors => {
  const errors: FieldErrors = {};
  const rawErrors = error.errors as unknown;

  if (Array.isArray(rawErrors)) {
    rawErrors.forEach((fieldError) => {
      if (
        typeof fieldError !== "object" ||
        fieldError === null ||
        !("field" in fieldError) ||
        !("messages" in fieldError) ||
        typeof fieldError.field !== "string"
      ) {
        return;
      }

      const fieldName = getFieldName(fieldError.field);
      const message = getFieldMessage(fieldError.messages);

      if (fieldName && message) {
        errors[fieldName] = message;
      }
    });

    return errors;
  }

  if (typeof rawErrors === "object" && rawErrors !== null) {
    Object.entries(rawErrors).forEach(([field, messages]) => {
      const fieldName = getFieldName(field);
      const message = getFieldMessage(messages);

      if (fieldName && message) {
        errors[fieldName] = message;
      }
    });
  }

  return errors;
};

function FieldErrorMessage({ message }: { message?: string }) {
  if (!message) {
    return null;
  }

  return <p className="mt-1 text-sm text-error">{message}</p>;
}

function FormFieldRow({
  label,
  error,
  hideError = false,
  children,
}: {
  label: string;
  error?: string;
  hideError?: boolean;
  children: ReactNode;
}) {
  return (
    <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:gap-5">
      <label className="w-28 text-sm text-gray-700">{label}</label>
      <div className="flex-1">
        {children}
        {!hideError && <FieldErrorMessage message={error} />}
      </div>
    </div>
  );
}

const toDateOnly = (date: Date) =>
  new Date(date.getFullYear(), date.getMonth(), date.getDate());

const addYears = (date: Date, years: number) => {
  const next = toDateOnly(date);
  next.setFullYear(next.getFullYear() + years);

  if (next.getMonth() !== date.getMonth()) {
    next.setDate(0);
  }

  return next;
};

const getEditUserValidationErrors = ({
  dateOfBirth,
  gender,
  joinedDate,
  type,
}: {
  dateOfBirth: Date | null;
  gender: Gender;
  joinedDate: Date | null;
  type: UserRoles;
}): FieldErrors => {
  const errors: FieldErrors = {};
  const today = toDateOnly(new Date());
  const dateOfBirthOnly = dateOfBirth ? toDateOnly(dateOfBirth) : null;
  const joinedDateOnly = joinedDate ? toDateOnly(joinedDate) : null;

  if (!dateOfBirthOnly) {
    errors.dateOfBirth = editUserValidationMessages.dateOfBirthRequired;
  } else if (dateOfBirthOnly > today) {
    errors.dateOfBirth = editUserValidationMessages.dateOfBirthInFuture;
  } else if (dateOfBirthOnly > addYears(today, -18)) {
    errors.dateOfBirth = editUserValidationMessages.dateOfBirthUnder18;
  } else if (dateOfBirthOnly < addYears(today, -90)) {
    errors.dateOfBirth = editUserValidationMessages.dateOfBirthOver90;
  }

  if (!joinedDateOnly) {
    errors.joinedDate = editUserValidationMessages.joinedDateRequired;
  } else if (dateOfBirthOnly && joinedDateOnly <= dateOfBirthOnly) {
    errors.joinedDate =
      editUserValidationMessages.joinedDateNotLaterThanDateOfBirth;
  } else if (dateOfBirthOnly && joinedDateOnly < addYears(dateOfBirthOnly, 18)) {
    errors.joinedDate = editUserValidationMessages.joinedDateUnder18;
  } else if (joinedDateOnly.getDay() === 0 || joinedDateOnly.getDay() === 6) {
    errors.joinedDate = editUserValidationMessages.joinedDateWeekend;
  }

  if (!validGenderValues.has(gender)) {
    errors.gender = editUserValidationMessages.invalidGender;
  }

  if (!validUserTypeValues.has(type)) {
    errors.type = editUserValidationMessages.invalidType;
  }

  return errors;
};

export default function EditUserPage() {
  const searchParams = useSearchParams();
  const userId = searchParams.get("id") ?? "";

  const { data: user, isLoading, error } = useGetUserForEditQuery(userId, {
    skip: !userId,
  });

  if (isLoading) {
    return <LoadingSpinner />;
  }

  if (!userId || error || !user) {
    return (
      <div className="min-h-screen bg-white text-[#333]">
        <div className="min-w-0 flex-1">
          <EntityNotFound
            pageTitle="Edit User"
            entityName="User"
            action="edit"
            redirectPath="/admin/users"
            redirectText="Back to Manage Users"
          />
        </div>
      </div>
    );
  }

  return (
    <div className="w-full max-w-xl mb-10" data-testid="tabManagerUser">
      <div className="flex min-w-0">
        <div className="min-w-0 flex-1">
          <h1 className="mb-6 text-xl font-bold text-primary">Edit User</h1>
          {isLoading && <p className="text-sm text-gray-700">Loading...</p>}
          {!isLoading && user && <EditUserForm userId={userId} user={user} />}
        </div>
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
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  const redirectToUsers = () => {
    router.push("/admin/users");
  };

  const validationErrors = getEditUserValidationErrors({
    dateOfBirth,
    gender: selectedGender,
    joinedDate,
    type: selectedType,
  });
  const isSaveDisabled =
    isSaving || Object.keys(validationErrors).length > 0;
  const getDisplayedDateError = (field: RequiredDateField) => {
    const fieldError = fieldErrors[field];

    return validationErrors[field] ?? fieldError;
  };
  const displayedFieldErrors: FieldErrors = {
    ...fieldErrors,
    ...validationErrors,
    dateOfBirth: getDisplayedDateError("dateOfBirth"),
    joinedDate: getDisplayedDateError("joinedDate"),
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (Object.keys(validationErrors).length > 0) {
      setFieldErrors(validationErrors);
      return;
    }

    setFieldErrors({});

    if (!dateOfBirth || !joinedDate) {
      return;
    }

    try {
      await editUser({
        userId,
        dateOfBirth: localDateToUtcIso(dateOfBirth),
        gender: selectedGender,
        joinedDate: localDateToUtcIso(joinedDate),
        type: selectedType,
        concurrencyStamp: user.concurrencyStamp,
      }).unwrap();

      sessionStorage.setItem("usersTemporarySort", "updatedDateDesc");

      dispatch(
        enqueueToast({
          message: "User updated successfully.",
          type: ToastType.Success,
        }),
      );
      redirectToUsers();
    } catch (error) {
      if (isApiErrorResponse(error)) {
        if (error.status === 409) {
          dispatch(
            enqueueToast({
              message: getEditUserErrorMessage(error),
              type: ToastType.Error,
            }),
          );
          return;
        }

        if (error.status === 400) {
          const validationErrors = getValidationErrors(error);

          if (Object.keys(validationErrors).length > 0) {
            setFieldErrors(validationErrors);
            return;
          }
        }

        dispatch(
          enqueueToast({
            message: getEditUserErrorMessage(error),
            type: ToastType.Error,
          }),
        );
        return;
      }

      dispatch(
        enqueueToast({
          message: "Failed to update user. Please try again.",
          type: ToastType.Error,
        }),
      );
    }
  };

  return (
    <form
      className="mb-10 w-full max-w-xl space-y-5"
      onSubmit={handleSubmit}
    >
      <FormFieldRow label="First Name">
        <input
          type="text"
          data-testid="txtEditFirstName"
          disabled
          value={user.firstName}
          className="input input-bordered w-full"
        />
      </FormFieldRow>

      <FormFieldRow label="Last Name">
        <input
          type="text"
          data-testid="txtEditLastName"
          disabled
          value={user.lastName}
          className="input input-bordered w-full"
        />
      </FormFieldRow>

      <FormFieldRow
        label="Date of Birth"
        error={displayedFieldErrors.dateOfBirth}
        hideError
      >
        <DatePickerInput
          value={dateOfBirth}
          onChange={(value) => {
            setDateOfBirth(value);
            setFieldErrors((current) => ({ ...current, dateOfBirth: undefined }));
          }}
          placeholder="Date of Birth"
          width="w-full"
          txtInputTestId="dtpEditDateOfBirth"
          error={displayedFieldErrors.dateOfBirth}
          showToast={false}
        />
      </FormFieldRow>

      <FormFieldRow label="Gender" error={displayedFieldErrors.gender}>
        <RadioGroup
          name="gender"
          items={genderOptions}
          value={selectedGender}
          getKey={(item) => item.value}
          getLabel={(item) => item.label}
          getTestId={(item) =>
            item.value === Gender.Male ? "rdoEditMale" : "rdoEditFemale"
          }
          onChange={(value) => {
            setSelectedGender(value as Gender);
            setFieldErrors((current) => ({ ...current, gender: undefined }));
          }}
        />
      </FormFieldRow>

      <FormFieldRow
        label="Joined Date"
        error={displayedFieldErrors.joinedDate}
        hideError
      >
        <DatePickerInput
          value={joinedDate}
          onChange={(value) => {
            setJoinedDate(value);
            setFieldErrors((current) => ({ ...current, joinedDate: undefined }));
          }}
          placeholder="Joined Date"
          width="w-full"
          txtInputTestId="dtpEditJoinedDate"
          error={displayedFieldErrors.joinedDate}
          showToast={false}
        />
      </FormFieldRow>

      <FormFieldRow label="Type" error={displayedFieldErrors.type}>
        <UserTypeDropdown
          value={selectedType}
          onChange={(value) => {
            setSelectedType(value);
            setFieldErrors((current) => ({ ...current, type: undefined }));
          }}
          disabled={user.isCurrentUser}
          width="w-full"
          testId="ddlEditUserType"
        />
      </FormFieldRow>

      <div className="flex flex-col-reverse gap-3 pt-6 sm:flex-row sm:justify-end">
        <button
          type="submit"
          data-testid="btnSaveEditUser"
          disabled={isSaveDisabled}
          className="h-9 rounded-md bg-primary px-6 text-sm font-medium text-white hover:bg-red-600 disabled:cursor-not-allowed disabled:opacity-50 hover:cursor-pointer"
        >
          {isSaving ? "Saving..." : "Save"}
        </button>

        <button
          type="button"
          data-testid="btnCancelEditUser"
          onClick={redirectToUsers}
          className="hover:cursor-pointer h-9 rounded-md border border-gray-300 bg-white px-6 text-sm font-medium text-gray-700 hover:bg-gray-50"
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
