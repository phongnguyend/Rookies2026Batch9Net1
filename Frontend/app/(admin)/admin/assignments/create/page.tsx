"use client";

import { useState } from "react";
import { Resolver, useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import { useRouter } from "next/navigation";
import { useCreateAssignmentMutation } from "@/features/assignments/admin/assignments.api";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { useAppDispatch } from "@/lib/redux/hooks";
import { LookupUsers } from "@/features/users/users.types";
import { LookupAssetsSummary } from "@/features/Assets/assets.types";
import UsersLookupInput from "@/features/shared/components/LookupTable/UsersLookup/UsersLookupInput";
import AssetsLookupInput from "@/features/shared/components/LookupTable/AssetsLookup/AssetsLookupInput";

// ─── Zod Schema ───────────────────────────────────────────────────────────────
const createAssignmentSchema = z.object({
  userId: z.guid({ error: "User ID must be a valid GUID." }),
  assetId: z.guid({ error: "Asset ID must be a valid GUID." }),

  assignedDate: z
    .date({ error: "Assigned date is required." })
    .refine(
      (date) => {
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return date >= today;
      },
      { error: "Assigned date must be current date or in the future." }
    ),

  note: z
    .string()
    .max(1000, { error: "Note cannot exceed 1000 characters." })
    .optional()
    .or(z.literal("")),
});

type CreateAssignmentFormValues = z.infer<typeof createAssignmentSchema>;

// ─── Props ────────────────────────────────────────────────────────────────────
interface AssignmentFormProps {
  title?: string;
  initialUserId?: string;
  initialAssetId?: string;
  initialAssignedDate?: Date | null;
  initialNote?: string;
}

// ─── Component ────────────────────────────────────────────────────────────────
export default function CreateAssignment({
  title = "Create New Assignment",
  initialUserId = "",
  initialAssetId = "",
  initialAssignedDate = new Date(),
  initialNote = "",
}: AssignmentFormProps) {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const [createAssignment, { isLoading }] = useCreateAssignmentMutation();
  const [user, setUser] = useState<LookupUsers.LookupUsersSummary | null>(null);
  const [asset, setAsset] = useState<LookupAssetsSummary | null>(null);

  const {
    control,
    handleSubmit,
    setValue,
    formState: { errors, isValid },
  } = useForm<CreateAssignmentFormValues>({
    resolver: zodResolver(
      createAssignmentSchema
    ) as Resolver<CreateAssignmentFormValues>,
    mode: "onTouched",
    reValidateMode: "onChange",
    defaultValues: {
      userId: initialUserId,
      assetId: initialAssetId,
      assignedDate: initialAssignedDate ?? new Date(),
      note: initialNote,
    },
  });

  // ─── Handle Submit ─────────────────────────────────────────────────────────
  const onSubmit = async (values: CreateAssignmentFormValues) => {
    try {
      await createAssignment({
        userId: values.userId,
        assetId: values.assetId,
        assignedDate: `${values.assignedDate.getFullYear()}-${String(values.assignedDate.getMonth() + 1).padStart(2, "0")}-${String(values.assignedDate.getDate()).padStart(2, "0")}`,
        note: values.note?.trim() || undefined,
      }).unwrap();

      dispatch(
        enqueueToast({
          message: "Assignment created successfully.",
          type: ToastType.Success,
        })
      );

      router.push("/admin/assignments");
    } catch (err: unknown) {
      const error = err as { data?: { detail?: string; title?: string }; detail?: string; title?: string };
      dispatch(
        enqueueToast({
          message:
            error?.data?.detail ||
            error?.data?.title ||
            error?.detail ||
            error?.title ||
            "Failed to create assignment.",
          type: ToastType.Error,
        })
      );
    }
  };

  // ─── Render ────────────────────────────────────────────────────────────────
  return (
    <div className="w-full max-w-xl rounded-lg bg-white p-4 sm:p-6 md:p-8">
      <h2 className="mb-8 text-xl font-bold text-red-600">{title}</h2>

      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className="space-y-5">

          {/* User */}
          <UsersLookupInput
            value={user}
            onChange={(selected) => {
              setUser(selected);
              setValue("userId", selected?.id ?? "", {
                shouldValidate: true,
                shouldTouch: true,
              });
            }} />

          {/* Asset */}
          <AssetsLookupInput
            value={asset}
            onChange={(selected) => {
              setAsset(selected);
              setValue("assetId", selected?.id ?? "", {
                shouldValidate: true,
                shouldTouch: true,
              });
            }} />

          {/* Assigned Date */}
          <div>
            <div className="flex flex-col gap-2 md:flex-row md:items-center md:gap-4">
              <label className="text-sm font-medium text-gray-700 md:w-32 md:shrink-0">
                Assigned Date
              </label>
              <div className="flex-1" data-testid="dtpAssignedDate">
                <Controller
                  control={control}
                  name="assignedDate"
                  render={({ field }) => (
                    <DatePickerInput
                      value={field.value}
                      onChange={(date) => {
                        field.onChange(date);
                        field.onBlur(); // ← trigger touch
                      }}
                      placeholder="Select date"
                      width="w-full"
                    />
                  )}
                />
              </div>
            </div>
            {errors.assignedDate && (
              <div className="md:pl-36">
                <p className="mt-1 text-sm text-red-500">
                  {errors.assignedDate.message}
                </p>
              </div>
            )}
          </div>

          {/* Note */}
          <div>
            <div className="flex flex-col gap-2 md:flex-row md:items-start md:gap-4">
              <label className="pt-0 text-sm font-medium text-gray-700 md:w-32 md:shrink-0 md:pt-2">
                Note
              </label>
              <Controller
                control={control}
                name="note"
                render={({ field }) => (
                  <textarea
                    {...field}
                    rows={4}
                    maxLength={2000}
                    data-testid="txtNote"
                    className="w-full flex-1 resize-none rounded-md border border-gray-300 bg-white px-3 py-2 text-sm text-gray-800 outline-none transition-colors focus:border-gray-400"
                  />
                )}
              />
            </div>
            {errors.note && (
              <div className="md:pl-36">
                <p className="mt-1 text-sm text-red-500">
                  {errors.note.message}
                </p>
              </div>
            )}
          </div>

        </div>

        {/* Actions */}
        <div className="mt-8 flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
          <button
            type="button"
            onClick={() => router.push("/admin/assignments")}
            className="h-9 rounded-md border border-gray-300 bg-white px-6 text-sm font-medium text-gray-700 hover:bg-gray-50 cursor-pointer"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={!isValid || isLoading}
            className="h-9 rounded-md bg-red-500 px-6 text-sm font-medium text-white hover:bg-red-600 disabled:cursor-not-allowed disabled:opacity-50 cursor-pointer"
          >
            {isLoading ? "Saving..." : "Save"}
          </button>
        </div>
      </form>
    </div>
  );
}
