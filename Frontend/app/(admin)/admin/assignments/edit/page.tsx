"use client";

import { LookupAssetsSummary } from "@/features/Assets/assets.types";
import {
  useAdminEditAssignmentMutation,
  useGetEditingAssignmentQuery,
} from "@/features/assignments/admin/assignments.api";
import { setPromotedAssignment } from "@/features/assignments/admin/edit/admin-assignment-list-ui.slice";
import {
  EditAssignmentFormValues,
  editAssignmentSchema,
} from "@/features/assignments/admin/edit/editAssignmentSchema";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import EntityNotFound from "@/features/shared/components/EntityNotFound";
import LoadingSpinner from "@/features/shared/components/LoadingSpinner";
import { AssetsWithAssignedLookupInput } from "@/features/shared/components/LookupTable/AssetsWithAssignedLookup/AssetsWithAssignedLookupInput";
import UsersLookupInput from "@/features/shared/components/LookupTable/UsersLookup/UsersLookupInput";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { LookupUsers } from "@/features/users/users.types";
import { useAppDispatch } from "@/lib/redux/hooks";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";

const EditAssignmentPage = () => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const searchParams = useSearchParams();
  const id = searchParams.get("id");
  const { data, isLoading, isError } = useGetEditingAssignmentQuery(
    { assignmentId: id ?? "" },
    { skip: !id },
  );
  const [user, setUser] = useState<LookupUsers.LookupUsersSummary | null>(null);
  const [asset, setAsset] = useState<LookupAssetsSummary | null>(null);
  const [editAssignment, { isLoading: isEditLoading }] =
    useAdminEditAssignmentMutation();

  const form = useForm<EditAssignmentFormValues>({
    resolver: zodResolver(editAssignmentSchema),
    defaultValues: {
      userId: "",
      assetId: "",
      assignedDate: null,
      note: "",
    },
    mode: "onTouched",
    reValidateMode: "onChange",
  });

  useEffect(() => {
    if (data) {
      form.reset({
        userId: data.user.id,
        assetId: data.asset.id,
        assignedDate: new Date(data.assignedDate),
        note: data.note ?? "",
      });
      form.trigger("assignedDate");
      setUser(data.user);
      setAsset(data.asset);
    }
  }, [data]);

  const {
    control,
    handleSubmit,
    setValue,
    formState: { errors, isValid },
    watch,
  } = form;

  const onSubmit = async (data: EditAssignmentFormValues) => {
    try {
      const assignedDateUtc = new Date(
        Date.UTC(
          data.assignedDate!.getFullYear(),
          data.assignedDate!.getMonth(),
          data.assignedDate!.getDate(),
        ),
      );
      const updatedAssignment = await editAssignment({
        assignmentId: id ?? "",
        payload: {
          userId: data.userId,
          assetId: data.assetId,
          assignedDate: assignedDateUtc.toISOString(),
          note: data.note,
        },
      }).unwrap();

      dispatch(
        enqueueToast({
          message: "Assignment edited successfully.",
          type: ToastType.Success,
        }),
      );

      dispatch(setPromotedAssignment(updatedAssignment));

      router.push("/admin/assignments");
    } catch (err: unknown) {
      const error = err as {
        data?: { detail?: string; title?: string };
        detail?: string;
        title?: string;
      };
      dispatch(
        enqueueToast({
          message:
            error?.data?.detail ||
            error?.data?.title ||
            error?.detail ||
            error?.title ||
            "Failed to edit assignment.",
          type: ToastType.Error,
        }),
      );
    }
  };

  if (isLoading) {
    return <LoadingSpinner />;
  }

  if (!id || isError || !data) {
    return (
      <EntityNotFound
        pageTitle="Edit Assignment"
        entityName="Assignment"
        action="edit"
        redirectPath="/admin/assignments"
        redirectText="Back to Manage Assignments"
      />
    );
  }

  return (
    <div className="w-full max-w-xl">
      <h2 className="mb-8 text-xl font-bold text-red-600">Edit Assignment</h2>
      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <div className="space-y-5">
          <UsersLookupInput
            value={user}
            onChange={(selectedUser) => {
              setUser(selectedUser);
              setValue("userId", selectedUser?.id ?? "", {
                shouldValidate: true,
              });
            }}
          />

          <AssetsWithAssignedLookupInput
            value={asset}
            onChange={(selectedAsset) => {
              setAsset(selectedAsset);
              setValue("assetId", selectedAsset?.id ?? "", {
                shouldValidate: true,
              });
            }}
            assignedAssetId={asset?.id}
          />

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
                  render={({ field, fieldState }) => (
                    <DatePickerInput
                      value={field.value}
                      onChange={(date) => {
                        field.onChange(date);
                        field.onBlur();
                      }}
                      placeholder="Select date"
                      width="w-full"
                      showToast={false}
                      error={fieldState.error?.message}
                    />
                  )}
                />
              </div>
            </div>
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
                    className="w-full flex-1 rounded-md border border-gray-300 bg-white px-3 py-2 text-sm text-gray-800 outline-none transition-colors focus:border-gray-400"
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

          {/* Actions */}
          <div className="mt-8 flex flex-col gap-3 sm:flex-row sm:justify-end">
            <button
              type="submit"
              disabled={!isValid || isLoading || isEditLoading}
              className="h-9 rounded-md bg-red-500 px-6 text-sm font-medium text-white hover:bg-red-600 disabled:cursor-not-allowed disabled:opacity-50 hover:cursor-pointer"
            >
              {isLoading ? "Saving..." : "Save"}
            </button>
            <button
              type="button"
              onClick={() => router.push("/admin/assignments")}
              className="hover:cursor-pointer h-9 rounded-md border border-gray-300 bg-white px-6 text-sm font-medium text-gray-700 hover:bg-gray-50"
            >
              Cancel
            </button>
          </div>
        </div>
      </form>
    </div>
  );
};

export default EditAssignmentPage;
