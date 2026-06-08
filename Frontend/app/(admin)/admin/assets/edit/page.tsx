"use client";

import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import {
  useGetAssetByIdQuery,
  useEditAssetMutation,
} from "@/features/Assets/assets.api";
import { AssetState } from "@/features/Assets/assets.types";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import type { ApiErrorResponse } from "@/lib/api/base.types";
import { setPinnedEditedAsset } from "@/features/Assets/editAssetStore";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { useAppDispatch } from "@/lib/redux/hooks";
import {
  ALLOWED_REGEX,
  normalizeText,
  stripEmoji,
  formatDateToISO,
  EDITABLE_STATES,
} from "@/features/Assets/components/assetConstant";
import {
  FormField,
  CharacterCounter,
  FieldError,
} from "@/features/Assets/components/assetSharedForm";
import EntityNotFound from "@/features/shared/components/EntityNotFound";
import LoadingSpinner from "@/features/shared/components/LoadingSpinner";

export default function EditAssetPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const assetId = searchParams.get("id");
  const dispatch = useAppDispatch();
  const [serverError, setServerError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const { data: asset, isLoading, error } = useGetAssetByIdQuery(assetId ?? "", {
    skip: !assetId,
  });

  const [editAsset, { isLoading: isEditing }] = useEditAssetMutation();

  const [form, setForm] = useState({
    assetName: "",
    specification: "",
    installedDate: null as Date | null,
    state: AssetState.Available,
  });

  //-- Set data into Fields -----------------------------------------------
  useEffect(() => {
    if (!asset) return;
    setForm({
      assetName: stripEmoji(asset.name ?? ""),
      specification: stripEmoji(asset.specification ?? ""),
      installedDate: asset.installedAtUtc
        ? new Date(asset.installedAtUtc)
        : null,
      state: asset.state,
    });
  }, [asset]);

  const validateForm = () => {
    const errors: Record<string, string> = {};

    if (!form.assetName.trim()) {
      errors.assetName = "Asset name is required.";
    } else if (form.assetName.length > 100) {
      errors.assetName = "Asset name must not exceed 100 characters.";
    } else if (!ALLOWED_REGEX.test(form.assetName)) {
      errors.assetName =
        'Asset name must contain at least one letter and only allow letters, numbers and these special characters: " / - | ( ) + . ,';
    }

    if (!form.specification.trim()) {
      errors.specification = "Specification is required.";
    } else if (form.specification.length > 500) {
      errors.specification = "Specification must not exceed 500 characters.";
    } else if (!ALLOWED_REGEX.test(form.specification)) {
      errors.specification =
        'Specification must contain at least one letter and only allow letters, numbers and these special characters: " / - | ( ) + . ,';
    }

    if (!form.installedDate) {
      errors.installedDate = "Installed date is required.";
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const isFormValid =
    form.assetName.trim() !== "" &&
    form.specification.trim() !== "" &&
    form.installedDate instanceof Date &&
    !isNaN(form.installedDate.getTime());

  const isChanged =
    asset &&
    (form.assetName !== asset.name ||
      form.specification !== asset.specification ||
      form.state !== asset.state ||
      formatDateToISO(form.installedDate) !==
        asset.installedAtUtc.split("T")[0]);

  //-- Enable Save Button when something is edited ----------------------
  const canSave = isFormValid && isChanged && !isEditing;

  //-- Handle Save -----------------------------------------------
  const handleSave = async () => {
    setServerError(null);

    if (
      !validateForm() ||
      !(form.installedDate instanceof Date) ||
      isNaN(form.installedDate.getTime())
    ) {
      return;
    }

    try {
      const result = await editAsset({
        assetId: assetId!,
        assetName: normalizeText(form.assetName).trim(),
        specification: normalizeText(form.specification).trim(),
        installedDate: formatDateToISO(form.installedDate),
        state: form.state,
      }).unwrap();

      setPinnedEditedAsset({
        id: result.id,
        assetCode: result.assetCode,
        name: result.assetName,
        category: result.category,
        state: result.state as AssetState,
        location: result.location,
        hasHistory: result.hasHistory,
      });

      router.push("/admin/assets");
    } catch (err) {
      const apiError = err as ApiErrorResponse;

      if (apiError?.status === 400) {
        setServerError(apiError.detail);
      } else if (apiError?.status === 404) {
        dispatch(
          enqueueToast({
            message: `Something went wrong. Asset is not found or be deleted.`,
            type: ToastType.Error,
          }),
        );
        router.push("/admin/assets");
      } else if (apiError?.status === 409) {
        dispatch(
          enqueueToast({
            message: `Something went wrong. This asset has been assigned`,
            type: ToastType.Error,
          }),
        );
        router.push("/admin/assets");
      }
    }
  };

  //-- Check Url (Scenario : user parse id of assigned or deleted on URL) -------
  if (isLoading) {
    return <LoadingSpinner />;
  }

  if (!assetId || !asset || error){
    return (
      <EntityNotFound
        pageTitle="Edit Asset"
        entityName="Asset"
        action="edit"
        redirectPath="/admin/assets"
        redirectText="Back to Manage Assets"
      />
    );
  }

  //-- Render -----------------------------------------------
  return (
    <div className="w-full max-w-lg px-4 sm:px-0 mb-10">
      <h1 className="mb-6 text-xl font-bold text-primary">Edit Asset</h1>

      {serverError && (
        <div className="mb-4 rounded border border-red-300 bg-red-50 px-4 py-3 text-sm text-red-600">
          {serverError}
        </div>
      )}
      {/* Name */}
      <div className="space-y-4">
        <FormField label="Name">
          <input
            data-testid="txtName"
            type="text"
            maxLength={100}
            value={form.assetName}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                assetName: stripEmoji(e.target.value),
              }))
            }
            className="h-9 w-full rounded border border-gray-400 px-3 text-sm outline-none focus:border-gray-600"
          />
          <CharacterCounter value={form.assetName} max={100} />
          <FieldError message={fieldErrors.assetName} />
        </FormField>

        {/* Category */}
        <FormField label="Category">
          <input
            data-testid="txtCategory"
            type="text"
            value={asset?.category ?? ""}
            disabled
            className="h-9 w-full cursor-not-allowed rounded border border-gray-200 bg-gray-100 px-3 text-sm text-gray-500"
          />
        </FormField>
        {/* Specification */}
        <FormField label="Specification">
          <textarea
            data-testid="txaSpecification"
            rows={4}
            maxLength={500}
            value={form.specification}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                specification: stripEmoji(e.target.value),
              }))
            }
            className="w-full resize-none rounded border border-gray-400 px-3 py-2 text-sm outline-none focus:border-gray-600"
          />

          <CharacterCounter value={form.specification} max={500} />

          <FieldError message={fieldErrors.specification} />
        </FormField>

        {/* Installed Date */}
        <FormField label="Installed Date">
          <DatePickerInput
            key={form.installedDate?.toISOString() ?? "empty"}
            value={form.installedDate}
            onChange={(date) => {
              setForm((prev) => ({
                ...prev,
                installedDate:
                  date instanceof Date && !isNaN(date.getTime()) ? date : null,
              }));
            }}
            placeholder="Select date"
            width="w-full"
          />

          <FieldError message={fieldErrors.installedDate} />
        </FormField>
        {/* State */}
        <FormField label="State">
          <div className="flex flex-col gap-2 pt-2">
            {EDITABLE_STATES.map((s) => (
              <label
                key={s.value}
                className="flex cursor-pointer items-center gap-2 text-sm"
              >
                <input
                  data-testid={s.testId}
                  type="radio"
                  name="state"
                  checked={form.state === s.value}
                  onChange={() =>
                    setForm((prev) => ({
                      ...prev,
                      state: s.value,
                    }))
                  }
                  className="radio radio-primary radio-sm"
                />

                {s.label}
              </label>
            ))}
          </div>
        </FormField>
      </div>

      <div className="mt-8 flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
        <button
          data-testid="btnSave"
          type="button"
          onClick={handleSave}
          disabled={!canSave}
          className="btn btn-primary btn-sm disabled:cursor-not-allowed disabled:opacity-50"
        >
          {isEditing ? "Saving..." : "Save"}
        </button>
        <button
          data-testid="btnCancel"
          type="button"
          onClick={() => router.push("/admin/assets")}
          className="btn btn-outline btn-sm"
        >
          Cancel
        </button>
      </div>
    </div>
  );
}
