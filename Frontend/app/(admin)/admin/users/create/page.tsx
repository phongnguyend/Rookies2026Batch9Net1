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

const genderItems = [Gender.Female, Gender.Male];

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

const isJoinedDateOverOneWeek = (date: Date) => {
  const today = getToday();

  const maxJoinedDate = new Date(today);
  maxJoinedDate.setDate(maxJoinedDate.getDate() + 7);

  return normalizeDate(date) > maxJoinedDate;
};

const isWeekend = (date: Date) => {
  const day = normalizeDate(date).getDay();
  return day === 0 || day === 6;
};

const createUserSchema = z
  .object({
    firstName: z.string().trim().min(1, "First Name is required."),

    lastName: z.string().trim().min(1, "Last Name is required."),

    dateOfBirth: z
      .date()
      .nullable()
      .refine((value): value is Date => value !== null, {
        message: "Date of Birth is required.",
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
      .refine((value) => !isJoinedDateOverOneWeek(value), {
        message:
          "Joined date must not exceed a week. Please select a different date",
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

    const minJoinedDateByAge = new Date(dob);
    minJoinedDateByAge.setFullYear(dob.getFullYear() + 18);

    if (joinedDate < minJoinedDateByAge) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["joinedDate"],
        message:
          "User is under 18 at joined date. Please select a different date",
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
    formState: { errors, isValid, isSubmitting },
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

  const [createUser] = useCreateUserMutation();
  const dispatch = useAppDispatch();

  const onSubmit = async (data: CreateUserFormOutput) => {
    try {
      await createUser({
        firstName: data.firstName,
        lastName: data.lastName,
        dayOfBirth: data.dateOfBirth.toISOString(),
        joinedDate: data.joinedDate.toISOString(),
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
    <form onSubmit={handleSubmit(onSubmit)} className="w-[420px] space-y-4">
      <h1 className="text-xl font-bold text-primary">Create New User</h1>

      {/* First Name */}
      <div className="flex items-start gap-5">
        <label className={labelClass}>First Name</label>

        <div className="flex-1">
          <input
            {...register("firstName")}
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
      <div className="flex items-start gap-5">
        <label className={labelClass}>Last Name</label>

        <div className="flex-1">
          <input
            {...register("lastName")}
            className="input input-bordered w-full"
          />

          {errors.lastName && (
            <p className="mt-1 text-sm text-error">
              {errors.lastName.message}
            </p>
          )}
        </div>
      </div>

      {/* Date of Birth */}
      <div className="flex items-start gap-5">
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

              void trigger(["dateOfBirth", "joinedDate"]);
            }}
            placeholder=""
            width="w-full"
            txtInputTestId="txtDateOfBirth"
          />

          {errors.dateOfBirth && (
            <p className="mt-1 text-sm text-error">
              {errors.dateOfBirth.message}
            </p>
          )}
        </div>
      </div>

      {/* Gender */}
      <div className="flex items-center gap-5">
        <label className={labelClass}>Gender</label>

        <RadioGroup
          items={genderItems}
          name="gender"
          value={genderValue}
          getKey={(item) => item}
          getLabel={(item) => item}
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
      <div className="flex items-start gap-5">
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

              void trigger(["dateOfBirth", "joinedDate"]);
            }}
            placeholder=""
            width="w-full"
            txtInputTestId="txtJoinedDate"
          />

          {errors.joinedDate && (
            <p className="mt-1 text-sm text-error">
              {errors.joinedDate.message}
            </p>
          )}
        </div>
      </div>

      {/* User Type */}
      <div className="flex items-start gap-5">
        <label className={labelClass}>Type</label>

        <div className="flex-1">
          <select
            value={userTypeValue ?? ""}
            onChange={(e) =>
              setValue("userType", e.target.value as UserRoles, {
                shouldValidate: true,
                shouldDirty: true,
                shouldTouch: true,
              })
            }
            className="select select-bordered w-full"
          >
            <option value="" disabled></option>
            <option value={UserRoles.Admin}>Admin</option>
            <option value={UserRoles.Staff}>Staff</option>
          </select>

          {errors.userType && (
            <p className="mt-1 text-sm text-error">
              {errors.userType.message}
            </p>
          )}
        </div>
      </div>

      {/* Actions */}
      <div className="flex justify-end gap-4 pt-4">
        <button type="submit" disabled={!isValid || isSubmitting}>
          Save
        </button>

        <button type="button" className="btn btn-outline">
          Cancel
        </button>
      </div>
    </form>
  );
}