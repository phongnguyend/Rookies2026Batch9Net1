"use client";

import {
  type FocusEvent,
  type Dispatch,
  type FormEvent,
  type ReactNode,
  type SetStateAction,
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

const genderOptions = [
  { label: "Female", value: Gender.Female },
  { label: "Male", value: Gender.Male },
];

type FieldErrors = Partial<Record<"dateOfBirth" | "gender" | "joinedDate" | "type", string>>;
type RequiredDateField = "dateOfBirth" | "joinedDate";

const editUserValidationMessages = {
  dateOfBirthRequired: "'Date Of Birth' must not be empty.",
  dateOfBirthUnder18: "User is under 18. Please select a different date",
  dateOfBirthInFuture:
    "Date of birth cannot be in the future. Please select a different date",
  joinedDateRequired: "'Joined Date' must not be empty.",
  joinedDateNotLaterThanDateOfBirth:
    "Joined date is not later than Date of Birth. Please select a different date",
  joinedDateUnder18:
    "User must be at least 18 years old on the joined date. Please select a different date",
  joinedDateWeekend:
    "Joined date is Saturday or Sunday. Please select a different date",
  invalidGender: "Invalid gender. Please select a valid gender",
  invalidType: "Invalid type. Please select a valid type",
};

const requiredDateMessages = {
  dateOfBirth: editUserValidationMessages.dateOfBirthRequired,
  joinedDate: editUserValidationMessages.joinedDateRequired,
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

  return <p className="mt-1 text-xs text-error">{message}</p>;
}

function FormFieldRow({
  label,
  error,
  children,
}: {
  label: string;
  error?: string;
  children: ReactNode;
}) {
  return (
    <div className="grid grid-cols-1 gap-y-1 sm:grid-cols-[128px_minmax(0,1fr)] sm:gap-x-5">
      <label className="text-sm text-gray-800 sm:pt-[7px]">{label}</label>
      <div>
        {children}
        <FieldErrorMessage message={error} />
      </div>
    </div>
  );
}

function getDateInputValue(container: HTMLDivElement) {
  return container.querySelector("input")?.value.trim() ?? "";
}

const formatDateInputValue = (date: Date | null) => {
  if (!date) {
    return "";
  }

  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0");

  return `${day}/${month}/${date.getFullYear()}`;
};

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

  const { data: user, isLoading } = useGetUserForEditQuery(userId, {
    skip: !userId,
  });

  if (!userId) {
    return (
      <div className="min-h-screen bg-white text-[#333]">
        <main className="min-w-0 flex-1 px-4 sm:px-6 lg:pl-16 xl:pl-24">
          <h2 className="mb-6 text-xl font-bold text-primary">Edit User</h2>
          <p className="text-sm text-gray-700">Missing user id.</p>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white text-[#333]" data-testid="tabManagerUser">
      <div className="flex min-w-0">
        <main className="min-w-0 flex-1 px-4 sm:px-6 lg:pl-16 xl:pl-24">
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
  const [dateInputValues, setDateInputValues] = useState<
    Record<RequiredDateField, string>
  >({
    dateOfBirth: formatDateInputValue(utcDateToLocalDate(user.dateOfBirth)),
    joinedDate: formatDateInputValue(utcDateToLocalDate(user.joinedDate)),
  });
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  const redirectToUsers = () => {
    router.push("/admin/users");
  };

  const syncEmptyDateField = (
    field: RequiredDateField,
    setValue: Dispatch<SetStateAction<Date | null>>,
    container: HTMLDivElement,
  ) => {
    const inputValue = getDateInputValue(container);

    setDateInputValues((current) => ({
      ...current,
      [field]: inputValue,
    }));

    if (!inputValue) {
      setValue(null);
    }
  };

  const handleDateBlur = (
    field: RequiredDateField,
    setValue: Dispatch<SetStateAction<Date | null>>,
    event: FocusEvent<HTMLDivElement>,
  ) => {
    if (
      event.relatedTarget instanceof Node &&
      event.currentTarget.contains(event.relatedTarget)
    ) {
      return;
    }

    const inputValue = getDateInputValue(event.currentTarget);
    const isEmpty = !inputValue;

    if (isEmpty) {
      setValue(null);
    }

    setDateInputValues((current) => ({
      ...current,
      [field]: inputValue,
    }));

    setFieldErrors((current) => ({
      ...current,
      [field]: isEmpty ? requiredDateMessages[field] : undefined,
    }));
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

    if (
      dateInputValues[field] &&
      (fieldError === requiredDateMessages[field] ||
        validationErrors[field] === requiredDateMessages[field])
    ) {
      return undefined;
    }

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
      if (isApiErrorResponse(error) && error.status === 400) {
        const validationErrors = getValidationErrors(error);

        if (Object.keys(validationErrors).length > 0) {
          setFieldErrors(validationErrors);
          return;
        }
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
    <form className="w-full max-w-[560px] space-y-3" onSubmit={handleSubmit}>
      <FormFieldRow label="First Name">
        <input
          type="text"
          data-testid="txtEditFirstName"
          disabled
          value={user.firstName}
          className="h-[33px] w-full rounded border border-gray-400 bg-gray-100 px-3 text-sm text-gray-800 outline-none"
        />
      </FormFieldRow>

      <FormFieldRow label="Last Name">
        <input
          type="text"
          data-testid="txtEditLastName"
          disabled
          value={user.lastName}
          className="h-[33px] w-full rounded border border-gray-400 bg-gray-100 px-3 text-sm text-gray-800 outline-none"
        />
      </FormFieldRow>

      <FormFieldRow label="Date of Birth" error={displayedFieldErrors.dateOfBirth}>
        <div
          onInputCapture={(event) => {
            syncEmptyDateField("dateOfBirth", setDateOfBirth, event.currentTarget);
          }}
          onClickCapture={(event) => {
            const container = event.currentTarget;

            window.setTimeout(() => {
              syncEmptyDateField("dateOfBirth", setDateOfBirth, container);
            });
          }}
          onBlurCapture={(event) => {
            handleDateBlur("dateOfBirth", setDateOfBirth, event);
          }}
        >
          <DatePickerInput
            value={dateOfBirth}
            onChange={(value) => {
              setDateOfBirth(value);
              if (value) {
                setDateInputValues((current) => ({
                  ...current,
                  dateOfBirth: formatDateInputValue(value),
                }));
              }
              setFieldErrors((current) => ({ ...current, dateOfBirth: undefined }));
            }}
            placeholder="Date of Birth"
            width="w-full"
            txtInputTestId="dtpEditDateOfBirth"
          />
        </div>
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

      <FormFieldRow label="Joined Date" error={displayedFieldErrors.joinedDate}>
        <div
          onInputCapture={(event) => {
            syncEmptyDateField("joinedDate", setJoinedDate, event.currentTarget);
          }}
          onClickCapture={(event) => {
            const container = event.currentTarget;

            window.setTimeout(() => {
              syncEmptyDateField("joinedDate", setJoinedDate, container);
            });
          }}
          onBlurCapture={(event) => {
            handleDateBlur("joinedDate", setJoinedDate, event);
          }}
        >
          <DatePickerInput
            value={joinedDate}
            onChange={(value) => {
              setJoinedDate(value);
              if (value) {
                setDateInputValues((current) => ({
                  ...current,
                  joinedDate: formatDateInputValue(value),
                }));
              }
              setFieldErrors((current) => ({ ...current, joinedDate: undefined }));
            }}
            placeholder="Joined Date"
            width="w-full"
            txtInputTestId="dtpEditJoinedDate"
          />
        </div>
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

      <div className="flex flex-col-reverse gap-3 pt-6 sm:flex-row sm:justify-end sm:gap-7">
        <button
          type="submit"
          data-testid="btnSaveEditUser"
          disabled={isSaveDisabled}
          className="h-[35px] w-full rounded bg-primary px-5 text-sm font-semibold text-white transition hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-60 sm:w-auto"
        >
          {isSaving ? "Saving..." : "Save"}
        </button>

        <button
          type="button"
          data-testid="btnCancelEditUser"
          onClick={redirectToUsers}
          className="h-[35px] w-full rounded border border-gray-400 bg-white px-4 text-sm text-gray-600 transition hover:bg-gray-50 sm:w-auto"
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
