"use client";

import { useEffect } from "react";
import { Resolver, useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import { Search } from "lucide-react";
import { useRouter } from "next/navigation";

// ─── Zod Schema ───────────────────────────────────────────────────────────────
const createAssignmentSchema = z.object({
  userId: z.string().min(1, "User is required."),
  assetId: z.string().min(1, "Asset is required."),

  assignedDate: z
    .date()
    .nullable()
    .refine((date) => date != null, {
      message: "Assigned date is required.",
    })
    .refine(
      (date) => {
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return date! >= today;
      },
      { message: "Assigned date must be current date or in the future." }
    ),

  note: z
    .string()
    .max(1000, "Note cannot exceed 1000 characters.")
    .optional()
    .or(z.literal("")),
});

type CreateAssignmentFormValues = z.infer<typeof createAssignmentSchema>;

// ─── Props ────────────────────────────────────────────────────────────────────
interface AssignmentFormProps {
  title?: string;
  initialUserId?: string;
  initialUserName?: string;
  initialAssetId?: string;
  initialAssetName?: string;
  initialAssignedDate?: Date | null;
  initialNote?: string;
  onSave?: (data: { assignedDate: Date; note: string }) => void;
  onPickUser?: () => void;
  onPickAsset?: () => void;
}

// ─── Component ────────────────────────────────────────────────────────────────
export default function CreateAssignment({
  title = "Create New Assignment",
  initialUserId = "",
  initialUserName = "",
  initialAssetId = "",
  initialAssetName = "",
  initialAssignedDate = new Date(),
  initialNote = "",
  onSave,
  onPickUser,
  onPickAsset,
}: AssignmentFormProps) {
  const {
    control,
    handleSubmit,
    setValue,
    trigger,
    formState: { errors, isValid },
  } = useForm<CreateAssignmentFormValues>({
    resolver: zodResolver(
      createAssignmentSchema
    ) as Resolver<CreateAssignmentFormValues>,
    mode: "onTouched",  //Form sẽ validate khi field bị touch
    reValidateMode: "onChange", //Form sẽ re-validate khi value thay đổi sau khi đã bị touch
    defaultValues: {
      userId: initialUserId,
      assetId: initialAssetId,
      assignedDate: initialAssignedDate ?? undefined,
      note: initialNote,
    },
  });

  // ─── Sync Id từ parent khi pick xong ──────────────────────────────────────
  useEffect(() => {
    setValue("userId", initialUserId, {
      shouldValidate: initialUserId !== "", // chỉ validate khi đã có giá trị
    });
  }, [initialUserId, setValue]);

  useEffect(() => {
    setValue("assetId", initialAssetId, {
      shouldValidate: initialAssetId !== "", // chỉ validate khi đã có giá trị
    });
  }, [initialAssetId, setValue]);

  // ─── Handle Submit ─────────────────────────────────────────────────────────
  const onSubmit = (values: CreateAssignmentFormValues) => {
    onSave?.({
      assignedDate: values.assignedDate!,
      note: values.note?.trim() ?? "",
    });
  };

  const router = useRouter();

  // ─── Render ────────────────────────────────────────────────────────────────
  return (
    <div className="w-full max-w-xl rounded-lg bg-white p-4 sm:p-6 md:p-8">
      <h2 className="mb-8 text-xl font-bold text-red-600">{title}</h2>

      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className="space-y-5">

          {/* User */}
          <div>
            <div className="flex flex-col gap-2 md:flex-row md:items-center md:gap-4">
              <label className="text-sm font-medium text-gray-700 md:w-32 md:shrink-0">
                User
              </label>
              <div className="relative flex-1">
                <input
                  type="text"
                  readOnly
                  value={initialUserName}
                  onClick={onPickUser}
                  onBlur={() => trigger("userId")}
                  className="h-9 w-full cursor-pointer rounded-md border border-gray-300 bg-white px-3 pr-9 text-sm text-gray-800 outline-none transition-colors focus:border-gray-400"
                />
                <button
                  type="button"
                  onClick={onPickUser}
                  className="absolute right-0 top-0 flex h-9 w-9 items-center justify-center text-gray-400 hover:text-gray-600"
                  onBlur={() => trigger("userId")}
                >
                  <Search size={16} />
                </button>
              </div>
            </div>
            {errors.userId && (
              <div className="md:pl-36">
                <p className="mt-1 text-sm text-red-500">
                  {errors.userId.message}
                </p>
              </div>
            )}
          </div>

          {/* Asset */}
          <div>
            <div className="flex flex-col gap-2 md:flex-row md:items-center md:gap-4">
              <label className="text-sm font-medium text-gray-700 md:w-32 md:shrink-0">
                Asset
              </label>
              <div className="relative flex-1">
                <input
                  type="text"
                  readOnly
                  value={initialAssetName}
                  onClick={onPickAsset}
                  onBlur={() => trigger("assetId")}
                  className="h-9 w-full cursor-pointer rounded-md border border-gray-300 bg-white px-3 pr-9 text-sm text-gray-800 outline-none transition-colors focus:border-gray-400"
                />
                <button
                  type="button"
                  onClick={onPickAsset}
                  className="absolute right-0 top-0 flex h-9 w-9 items-center justify-center text-gray-400 hover:text-gray-600"
                >
                  <Search size={16} />
                </button>
              </div>
            </div>
            {errors.assetId && (
              <div className="md:pl-36">
                <p className="mt-1 text-sm text-red-500">
                  {errors.assetId.message}
                </p>
              </div>
            )}
          </div>

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
                      onChange={field.onChange}
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
                    maxLength={1000}
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
            className="h-9 rounded-md border border-gray-300 bg-white px-6 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={!isValid}
            className="h-9 rounded-md bg-red-500 px-6 text-sm font-medium text-white hover:bg-red-600 disabled:cursor-not-allowed disabled:opacity-50"
          >
            Save
          </button>
        </div>
      </form>
    </div>
  );
}
