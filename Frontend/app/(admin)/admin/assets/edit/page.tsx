"use client";

import { useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import {
  useGetAssetByIdQuery,
  useEditAssetMutation,
} from "@/features/Assets/assets.api";
import { AssetState } from "@/features/Assets/assets.types";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import type { ApiErrorResponse } from "@/lib/api/base.types";
import { setPinnedEditedAsset } from "@/features/Assets/editAssetStore";

const EDITABLE_STATES = [
  {
    value: AssetState.Available,
    label: "Available",
    testId: "rdoAvailable",
  },
  {
    value: AssetState.NotAvailable,
    label: "Not available",
    testId: "rdoNotAvailable",
  },
  {
    value: AssetState.WaitingForRecycling,
    label: "Waiting for recycling",
    testId: "rdoWaiting",
  },
  {
    value: AssetState.Recycled,
    label: "Recycled",
    testId: "rdoRecycled",
  },
];

export default function EditAssetPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const assetId = searchParams.get("id");

  const [serverError, setServerError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const { data: asset, isLoading } = useGetAssetByIdQuery(assetId ?? "", {
    skip: !assetId,
  });

  const [editAsset, { isLoading: isEditing }] = useEditAssetMutation();

  const [form, setForm] = useState({
    initialized: false,
    assetName: "",
    specification: "",
    installedDate: null as Date | null,
    state: AssetState.Available,
  });

  if (asset && !form.initialized) {
    setForm({
      initialized: true,
      assetName: asset.name,
      specification: asset.specification,
      installedDate: new Date(asset.installedAtUtc),
      state: asset.state,
    });
  }

  const isFormValid =
    form.assetName.trim() !== "" &&
    form.specification.trim() !== "" &&
    form.installedDate !== null;

  const formatDate = (date: Date) => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");

    return `${year}-${month}-${day}`;
  };

  const isChanged =
    asset &&
    (form.assetName !== asset.name ||
      form.specification !== asset.specification ||
      form.state !== asset.state ||
      formatDate(form.installedDate!) !== asset.installedAtUtc.split("T")[0]);

  const canSave = isFormValid && isChanged && !isEditing;

  const validateForm = () => {
    const errors: Record<string, string> = {};

    if (!form.assetName.trim()) {
      errors.assetName = "Asset name is required.";
    } else if (form.assetName.length > 100) {
      errors.assetName = "Asset name must not exceed 100 characters.";
    }

    if (!form.specification.trim()) {
      errors.specification = "Specification is required.";
    } else if (form.specification.length > 500) {
      errors.specification = "Specification must not exceed 500 characters.";
    }

    if (!form.installedDate) {
      errors.installedDate = "Installed date is required.";
    } else {
      const today = new Date();
      today.setHours(0, 0, 0, 0);
    }

    setFieldErrors(errors);

    return Object.keys(errors).length === 0;
  };

  const handleSave = async () => {
    setServerError(null);

    if (!validateForm() || !form.installedDate) {
      return;
    }

    try {
      const result = await editAsset({
        assetId: assetId!,
        assetName: form.assetName.trim(),
        specification: form.specification.trim(),
        installedDate: formatDate(form.installedDate),
        state: form.state,
      }).unwrap();

      setPinnedEditedAsset({
        id: result.id,
        assetCode: result.assetCode,
        name: result.assetName,
        category: result.category,
        state: form.state,
        location: result.location,
      });

      router.push("/admin/assets");
    } catch (err) {
      const apiError = err as ApiErrorResponse;

      if (apiError?.status === 400 || apiError?.status === 409) {
        setServerError(apiError.detail);
      } else {
        setServerError("Something went wrong. Please try again.");
      }
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-8">
        <span className="loading loading-spinner loading-md" />
      </div>
    );
  }

  return (
    <div className="max-w-lg p-6">
      <h1 className="mb-6 text-xl font-bold text-primary">Edit Asset</h1>

      {serverError && (
        <div className="mb-4 rounded border border-red-300 bg-red-50 px-4 py-3 text-sm text-red-600">
          {serverError}
        </div>
      )}

      <div className="space-y-4">
        <div>
          Name
          <input
            data-testid="txtName"
            type="text"
            maxLength={100}
            value={form.assetName}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                assetName: e.target.value,
              }))
            }
            className="h-9 w-full rounded border border-gray-400 px-3 text-sm outline-none focus:border-primary"
          />
          <div className="mt-1 flex items-center justify-between text-xs">
            {form.assetName.length === 0 ? (
              <span className="text-red-500">Asset Name is required.</span>
            ) : form.assetName.length === 100 ? (
              <span className="text-orange-500">
                Maximum characters is 100.
              </span>
            ) : (
              <span />
            )}

            <span
              className={
                form.assetName.length === 0 || form.assetName.length === 100
                  ? "text-red-500"
                  : "text-gray-500"
              }
            >
              {form.assetName.length}/100
            </span>
          </div>
          {fieldErrors.specification && (
            <p className="mt-1 text-sm text-red-500">
              {fieldErrors.specification}
            </p>
          )}
        </div>

        <div>
          Category
          <input
            data-testid="txtCategory"
            type="text"
            value={asset?.category ?? ""}
            disabled
            className="h-9 w-full cursor-not-allowed rounded border border-gray-200 bg-gray-100 px-3 text-sm text-gray-500"
          />
        </div>

        <div>
          Specification
          <textarea
            data-testid="txaSpecification"
            rows={4}
            maxLength={500}
            value={form.specification}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                specification: e.target.value,
              }))
            }
            className="w-full resize-none rounded border border-gray-400 px-3 py-2 text-sm outline-none focus:border-primary"
          />
          <div className="mt-1 flex items-center justify-between text-xs">
            {form.specification.length === 0 ? (
              <span className="text-red-500">Specification is required.</span>
            ) : form.specification.length === 500 ? (
              <span className="text-orange-500">
                Maximum characters is 500.
              </span>
            ) : (
              <span />
            )}

            <span
              className={
                form.specification.length === 0 || form.specification.length === 500
                  ? "text-red-500"
                  : "text-gray-500"
              }
            >
              {form.specification.length}/500
            </span>
          </div>
          {fieldErrors.specification && (
            <p className="mt-1 text-sm text-red-500">
              {fieldErrors.specification}
            </p>
          )}
        </div>

        <div data-testid="dtpInstalledDate">
          Installed Date
          <DatePickerInput
            value={form.installedDate}
            onChange={(date) => {
              console.log("Date changed:", date);
              setForm((prev) => ({
                ...prev,
                installedDate: date,
              }));
            }}
            placeholder="Select date"
            width="w-full"
          />
          {fieldErrors.installedDate && (
            <p className="mt-1 text-sm text-red-500">
              {fieldErrors.installedDate}
            </p>
          )}
        </div>

        <div className="flex items-start gap-4">
          <label className="w-36 shrink-0 pt-2 text-sm font-medium text-gray-700">
            State
          </label>

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
        </div>
      </div>

      <div className="mt-8 flex items-center justify-end gap-3">
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
