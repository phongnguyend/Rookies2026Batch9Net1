"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import {
  useGetCategoriesQuery,
  useCreateAssetMutation,
} from "@/features/Assets/assets.api";
import { AssetState } from "@/features/Assets/assets.types";
import CategoryDropdown from "@/features/Assets/components/categoryDropdown";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import type { ApiErrorResponse } from "@/lib/api/base.types";

export default function CreateAssetPage() {
  const router = useRouter();

  // ─── Form State ────────────────────────────────────
  const [assetName, setAssetName] = useState("");
  const [specification, setSpecification] = useState("");
  const [installedDate, setInstalledDate] = useState<Date | null>(null); // ← Date | null
  const [state, setState] = useState<string>(AssetState.Available);
  const [serverError, setServerError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [categoryId, setCategoryId] = useState("");      // ← sent to API
  const [categoryName, setCategoryName] = useState("");   // ← shown in dropdown

  // ─── API ───────────────────────────────────────────
  const { data: categoriesData, isLoading: categoriesLoading } =
    useGetCategoriesQuery();
  const [createAsset, { isLoading: isCreating }] = useCreateAssetMutation();

  // ─── Save enabled only when all fields filled ──────
  const isFormValid =
    assetName.trim() !== "" &&
    categoryId !== "" &&
    specification.trim() !== "" &&
    installedDate !== null; // ← check null not empty string

  // ─── Handle category selection ─────────────────────
  const handleCategoryChange = (id: string, name: string) => {  // ← accept id + name
    setCategoryId(id);
    setCategoryName(name);
  };

  // ─── Convert Date → YYYY-MM-DD for backend ─────────
  const formatDate = (date: Date): string => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  };

  // ─── Handle Save ───────────────────────────────────
  const handleSave = async () => {
    setServerError(null);
    const isValid = validateForm();
    if (!isValid || !installedDate) return;

    try {
      await createAsset({
        assetName: assetName.trim(),
        specification: specification.trim(),
        installedDate: formatDate(installedDate), // ← convert Date to YYYY-MM-DD
        state,
        categoryId,
      }).unwrap();

      router.push("/admin/assets");
    } catch (err) {
      const apiError = err as ApiErrorResponse;
      if (apiError?.status === 409 || apiError?.status === 400) {
        setServerError(apiError.detail);
      } else {
        setServerError("Something went wrong. Please try again.");
      }
    }
  };

  // ─── Validation form ───────────────────────────────────────
  const validateForm = () => {
    const errors: Record<string, string> = {};

    if (!assetName.trim()) {
      errors.assetName = "Asset name is required.";
    } else if (assetName.length > 100) {
      errors.assetName = "Asset name must not exceed 100 characters.";
    }

    if (!categoryId.trim()) {
      errors.categoryName = "Category name is required.";
    }

    if (!specification.trim()) {
      errors.specification = "Specification is required.";
    } else if (specification.length > 500) {
      errors.specification = "Specification must not exceed 500 characters.";
    }

    if (!installedDate) {
      errors.installedDate = "Installed date is required.";
    } else {
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      if (installedDate > today) {
        errors.installedDate = "Installed date cannot be in the future.";
      }
    }

    setFieldErrors(errors);

    return Object.keys(errors).length === 0;
  };

  // ─── Render ───────────────────────────────────────
  return (
    <div className="p-6 max-w-lg">
      <h1 className="text-primary font-bold text-xl mb-6">Create New Asset</h1>

      {serverError && (
        <div className="mb-4 rounded border border-red-300 bg-red-50 px-4 py-3 text-sm text-red-600">
          {serverError}
        </div>
      )}

      <div className="space-y-4">
        {/* Name */}
        <div className="flex-1">
          Name
          <input
            maxLength={100}
            data-testid="txtName"
            type="text"
            value={assetName}
            onChange={(e) => setAssetName(e.target.value)}
            className="h-9 w-full rounded border border-gray-400 px-3 text-sm outline-none focus:border-primary"
          />
          {fieldErrors.assetName && (
            <p className="mt-1 text-sm text-red-500">{fieldErrors.assetName}</p>
          )}
        </div>

        {/* Category */}
        <div className="flex-1">
          Category
          <CategoryDropdown
            categories={categoriesData ?? []}
            isLoading={categoriesLoading}
            value={categoryName}
            onChange={handleCategoryChange}
          />
          {fieldErrors.categoryId && (
            <p className="mt-1 text-sm text-red-500">
              {fieldErrors.categoryId}
            </p>
          )}
        </div>

        {/* Specification */}
        <div className="flex-1">
          Specification
          <textarea
            data-testid="txaSpecification"
            maxLength={500}
            value={specification}
            onChange={(e) => setSpecification(e.target.value)}
            rows={4}
            className="w-full rounded border border-gray-400 px-3 py-2 text-sm outline-none focus:border-primary resize-none"
          />
          {fieldErrors.specification && (
            <p className="mt-1 text-sm text-red-500">
              {fieldErrors.specification}
            </p>
          )}
        </div>

        {/* Installed Date */}
        <div className="flex-1" data-testid="dtpInstalledDate">
          Installed Date
          <DatePickerInput
            value={installedDate}
            onChange={(date) => setInstalledDate(date)}
            placeholder="Select date"
            width="w-full"
          />
          {fieldErrors.installedDate && (
            <p className="mt-1 text-sm text-red-500">
              {fieldErrors.installedDate}
            </p>
          )}
        </div>

        {/* State */}
        <div className="flex items-start gap-4">
          <label className="w-36 pt-2 text-sm font-medium text-gray-700 shrink-0">
            State
          </label>
          <div className="flex flex-col gap-2 pt-2">
            <label className="flex items-center gap-2 text-sm cursor-pointer">
              <input
                data-testid="rdoAvailable"
                type="radio"
                name="state"
                value={AssetState.Available}
                checked={state === AssetState.Available}
                onChange={() => setState(AssetState.Available)}
                className="radio radio-primary radio-sm"
              />
              Available
            </label>
            <label className="flex items-center gap-2 text-sm cursor-pointer">
              <input
                data-testid="rdoNotAvailable"
                type="radio"
                name="state"
                value={AssetState.NotAvailable}
                checked={state === AssetState.NotAvailable}
                onChange={() => setState(AssetState.NotAvailable)}
                className="radio radio-primary radio-sm"
              />
              Not available
            </label>
          </div>
        </div>
      </div>

      {/* Actions */}
      <div className="mt-8 flex items-center justify-end gap-3">
        <button
          data-testid="btnSave"
          type="button"
          onClick={handleSave}
          disabled={!isFormValid || isCreating}
          className="btn btn-primary btn-sm disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isCreating ? "Saving..." : "Save"}
        </button>

        <button
          data-testid="btnCancel"
          type="button"
          onClick={() => router.push("/admin/assets")}
          className="btn btn-sm btn-outline"
        >
          Cancel
        </button>
      </div>
    </div>
  );
}
