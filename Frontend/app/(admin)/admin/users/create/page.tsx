"use client";

import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import { Gender, UserRoles } from "@/features/users/users.types";
import RadioGroup from "@/features/shared/components/RadioGroup";
import { useCreateUserMutation } from "@/features/users/users.api";
import { useAppDispatch } from "@/lib/redux/hooks";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { useRouter, useSearchParams } from "next/navigation";
import DropdownFilter from "@/features/shared/components/DropdownFilter";

const genderItems = [Gender.Female, Gender.Male];
const nameRegex = /^[a-zA-Z]+(?: [a-zA-Z]+)*$/;

// ================= SCHEMA =================

const normalizeDate = (date: Date) => {
  const result = new Date(date);
  result.setHours(0, 0, 0, 0);
  return result;
};

const getToday = () => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return today;
};

const isUnder18 = (date: Date) => {
  const today = getToday();

  const maxDob = new Date(today);
  maxDob.setFullYear(maxDob.getFullYear() - 18);

  return normalizeDate(date) > maxDob;
};

const isOver90 = (date: Date) => {
  const today = getToday();

  const minDob = new Date(today);
  minDob.setFullYear(minDob.getFullYear() - 90);

  return normalizeDate(date) < minDob;
};

const isJoinedDateOverOneMonth = (date: Date) => {
  const today = getToday();

  const maxJoinedDate = new Date(today);
  maxJoinedDate.setMonth(maxJoinedDate.getMonth() + 1);

  return normalizeDate(date) > maxJoinedDate;
};

const isWeekend = (date: Date) => {
  const day = normalizeDate(date).getDay();
  return day === 0 || day === 6;
};

const isFutureDate = (date: Date) => {
  return normalizeDate(date) > getToday();
};

const createUserSchema = z
  .object({
    firstName: z
      .string()
      .trim()
      .min(1, "First Name is required.")
      .max(100, "First Name must not exceed 100 characters.")
      .regex(
        nameRegex,
        "First Name only allows alphabetic characters and spaces.",
      ),

    lastName: z
      .string()
      .trim()
      .min(1, "Last Name is required.")
      .max(100, "Last Name must not exceed 100 characters.")
      .regex(
        nameRegex,
        "Last Name only allows alphabetic characters and spaces.",
      ),

    dateOfBirth: z
      .date()
      .nullable()
      .refine((value): value is Date => value !== null, {
        message: "Date of Birth is required.",
      })
      .refine((value) => !isFutureDate(value), {
        message: "Date of Birth cannot be in the future",
      })
      .refine((value) => !isUnder18(value), {
        message: "User is under 18. Please select a different date.",
      })
      .refine((value) => !isOver90(value), {
        message: "User age must not exceed 90 years.",
      }),

    gender: z.enum(Gender, {
      error: "Gender is required.",
    }),

    joinedDate: z
      .date()
      .nullable()
      .refine((value): value is Date => value !== null, {
        message: "Joined Date is required.",
      })
      .refine((value) => !isJoinedDateOverOneMonth(value), {
        message:
          "Joined date must not exceed a month. Please select a different date",
      })
      .refine((value) => !isWeekend(value), {
        message:
          "Joined date is Saturday or Sunday. Please select a different date",
      }),

    userType: z.enum(UserRoles, {
      error: "Type is required.",
    }),
  })
  .superRefine((data, ctx) => {
    if (!data.dateOfBirth || !data.joinedDate) {
      return;
    }

    const dob = normalizeDate(data.dateOfBirth);
    const joinedDate = normalizeDate(data.joinedDate);

    if (joinedDate <= dob) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["joinedDate"],
        message:
          "Joined date must be later than Date of Birth. Please select a different date",
      });

      return;
    }

    const minJoinedDateByAge = new Date(dob);
    minJoinedDateByAge.setFullYear(dob.getFullYear() + 18);

    if (joinedDate < minJoinedDateByAge) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["joinedDate"],
        message: "User must be at least 18 years old at the Joined Date",
      });
    }
  });

type CreateUserFormInput = z.input<typeof createUserSchema>;
type CreateUserFormOutput = z.output<typeof createUserSchema>;

// ================= PAGE =================

export default function CreateUserPage() {
  const labelClass = "w-28 text-sm text-gray-700";

  const {
    register,
    watch,
    setValue,
    trigger,
    handleSubmit,
    reset,
    formState: { errors, isValid, isSubmitting, touchedFields },
  } = useForm<CreateUserFormInput, unknown, CreateUserFormOutput>({
    resolver: zodResolver(createUserSchema),
    mode: "onChange",
    defaultValues: {
      firstName: "",
      lastName: "",
      dateOfBirth: null,
      gender: Gender.Female,
      joinedDate: null,
      userType: UserRoles.Staff,
    },
  });

  const genderValue = watch("gender");
  const dateOfBirthValue = watch("dateOfBirth");
  const joinedDateValue = watch("joinedDate");
  const userTypeValue = watch("userType");

  const dateOfBirthError = touchedFields.dateOfBirth
    ? errors.dateOfBirth?.message
    : undefined;

  const joinedDateError = touchedFields.joinedDate
    ? errors.joinedDate?.message
    : undefined;

  const router = useRouter();
  const searchParams = useSearchParams();
  const [createUser] = useCreateUserMutation();
  const dispatch = useAppDispatch();

  const formatDateOnly = (date: Date) => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");

    return `${year}-${month}-${day}`;
  };

  const onSubmit = async (data: CreateUserFormOutput) => {
    try {
      console.log(formatDateOnly(data.joinedDate));
      await createUser({
        firstName: data.firstName,
        lastName: data.lastName,
        dayOfBirth: formatDateOnly(data.dateOfBirth),
        joinedDate: formatDateOnly(data.joinedDate),
        gender: data.gender,
        userType: data.userType,
      }).unwrap();

      dispatch(
        enqueueToast({
          message: "User created successfully.",
          type: ToastType.Success,
          testId: "txtCreateUserSuccess",
        }),
      );

      sessionStorage.setItem("usersTemporarySort", "createdDateDesc");
      router.push("/admin/users");
      reset();
    } catch (err: any) {
      console.log(err);
      dispatch(
        enqueueToast({
          message:
            err?.data?.detail ||
            err?.data?.title ||
            err?.detail ||
            err?.title ||
            "Failed to create user.",
          type: ToastType.Error,
          testId: "txtCreateUserError",
        }),
      );
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="w-full max-w-lg space-y-4 px-4 sm:px-0 mb-10">
      <h1 className="mb-6 text-xl font-bold text-primary">Create New User</h1>

      {/* First Name */}
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:gap-5">
        <label className={labelClass}>First Name</label>

        <div className="flex-1">
          <input
            {...register("firstName")}
            data-testid="txtUserFirstName"
            className="input input-bordered w-full"
          />

          {errors.firstName && (
            <p className="mt-1 text-sm text-error">
              {errors.firstName.message}
            </p>
          )}
        </div>
      </div>

      {/* Last Name */}
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:gap-5">
        <label className={labelClass}>Last Name</label>

        <div className="flex-1">
          <input
            {...register("lastName")}
            data-testid="txtUserLastName"
            className="input input-bordered w-full"
          />

          {errors.lastName && (
            <p className="mt-1 text-sm text-error">{errors.lastName.message}</p>
          )}
        </div>
      </div>

      {/* Date of Birth */}
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:gap-5">
        <label className={labelClass}>Date of Birth</label>

        <div className="flex-1">
          <DatePickerInput
            value={dateOfBirthValue}
            onChange={(date) => {
              setValue("dateOfBirth", date, {
                shouldValidate: true,
                shouldDirty: true,
                shouldTouch: true,
              });

              if (touchedFields.joinedDate) {
                void trigger(["dateOfBirth", "joinedDate"]);
              }
            }}
            placeholder=""
            width="w-full"
            txtInputTestId="dtpUserDateOfBirth"
            error={dateOfBirthError}
            showToast={false}
          />
        </div>
      </div>

      {/* Gender */}
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:gap-5">
        <label className={labelClass}>Gender</label>

        <RadioGroup
          items={genderItems}
          name="gender"
          data-testid="ddlUserType"
          value={genderValue}
          getKey={(item) => item}
          getLabel={(item) => item}
          getTestId={(item) =>
            item === Gender.Male ? "rdoUserMale" : "rdoUserFemale"
          }
          onChange={(value) => {
            setValue("gender", value as Gender, {
              shouldValidate: true,
              shouldDirty: true,
              shouldTouch: true,
            });
          }}
        />
        {errors.gender && (
          <p className="text-sm text-red-500">{errors.gender.message}</p>
        )}
      </div>

      {/* Joined Date */}
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:gap-5">
        <label className={labelClass}>Joined Date</label>

        <div className="flex-1">
          <DatePickerInput
            value={joinedDateValue}
            onChange={(date) => {
              setValue("joinedDate", date, {
                shouldValidate: true,
                shouldDirty: true,
                shouldTouch: true,
              });

              if (touchedFields.dateOfBirth) {
                void trigger(["dateOfBirth", "joinedDate"]);
              }
            }}
            error={joinedDateError}
            showToast={false}
            placeholder=""
            width="w-full"
            txtInputTestId="dtpUserJoinedDate"
          />
        </div>
      </div>

      {/* User Type */}
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:gap-5">
        <label className={labelClass}>Type</label>

        <div className="flex-1">
          <DropdownFilter
            items={[
              { value: UserRoles.Staff, label: "Staff" },
              { value: UserRoles.Admin, label: "Admin" },
            ]}
            getKey={(item) => item.value}
            getLabel={(item) => item.label}
            values={userTypeValue ? [userTypeValue] : []} // single value → array
            onChange={(vals) => {
              // pick the newly added value (last in array), ignore deselects
              const selected = vals.find((v) => v !== userTypeValue);
              if (selected) {
                setValue("userType", selected as UserRoles, {
                  shouldValidate: true,
                  shouldDirty: true,
                  shouldTouch: true,
                });
              }
            }}
            placeholder="Select type"
            width="w-full"
            showAll={false}
          />

          {errors.userType && (
            <p className="mt-1 text-sm text-error">{errors.userType.message}</p>
          )}
        </div>
      </div>

      {/* Actions */}
      <div className="flex flex-col-reverse gap-3 pt-6 sm:flex-row sm:justify-end">
        <button
          type="submit"
          data-testid="btnSaveUser"
          disabled={!isValid || isSubmitting}
          className="btn btn-primary w-full text-white sm:w-auto sm:min-w-24"
        >
          Save
        </button>
        <button
          type="button"
          data-testid="btnCancelUser"
          className="btn btn-outline btn-neutral w-full sm:w-auto sm:min-w-24"
          onClick={() =>
            router.push(searchParams.get("returnUrl") ?? "/admin/users")
          }
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
