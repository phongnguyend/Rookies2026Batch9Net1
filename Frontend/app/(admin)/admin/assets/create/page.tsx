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
import { setCreatedNewAsset } from "@/features/Assets/assets.slice";
import { useAppDispatch } from "@/lib/redux/hooks";

const EMOJI_REGEX = /\p{Extended_Pictographic}/gu
const normalizeText = (value: string) =>
  stripEmoji(value)
    .replace(/\s+/g, " ")
    .trim();
const stripEmoji = (value: string) => value.replace(EMOJI_REGEX, "")

export default function CreateAssetPage() {
  const router = useRouter();
  const dispatch = useAppDispatch();
  // ─── Form State ────────────────────────────────────
  const [assetName, setAssetName] = useState("");
  const [specification, setSpecification] = useState("");
  const [installedDate, setInstalledDate] = useState<Date | null>(null); // ← Date | null
  const [state, setState] = useState<string>(AssetState.Available);
  const [serverError, setServerError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [categoryId, setCategoryId] = useState(""); // ← sent to API
  const [categoryName, setCategoryName] = useState(""); // ← shown in dropdown

  // ─── API ───────────────────────────────────────────
  const { data: categoriesData, isLoading: categoriesLoading } =
    useGetCategoriesQuery();
  const [createAsset, { isLoading: isCreating }] = useCreateAssetMutation();

  // ─── Save enabled only when all fields filled ──────
  const isFormValid =
    assetName.trim() !== "" &&
    categoryId !== "" &&
    specification.trim() !== "" &&
    installedDate !== null;

  // ─── Handle category selection ─────────────────────
  const handleCategoryChange = (id: string, name: string) => {
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
        assetName: normalizeText(assetName),
        specification: normalizeText(specification),
        installedDate: formatDate(installedDate!),
        state,
        categoryId,
      }).unwrap();
      //Set state isNewCreatedAsset to true here
      dispatch(setCreatedNewAsset(true));

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

    const normalizedName = normalizeText(assetName);
    const normalizedSpec = normalizeText(specification);

    if (!normalizedName) {
      errors.assetName = "Asset name is required.";
    } else if (normalizedName.length > 100) {
      errors.assetName = "Asset name must not exceed 100 characters.";
    }

    if (!normalizedSpec) {
      errors.specification = "Specification is required.";
    } else if (normalizedSpec.length > 500) {
      errors.specification = "Specification must not exceed 500 characters.";
    }

    if (!categoryId.trim()) {
      errors.categoryId = "Category is required.";
    }

    if (!installedDate) {
      errors.installedDate = "Installed date is required.";
    }

    setFieldErrors(errors);

    return Object.keys(errors).length === 0;
  };

  // ─── Render ───────────────────────────────────────
  return (
    <div className="max-w-lg">
      <h1 className="text-primary font-bold text-xl mb-6">Create New Asset</h1>

      {serverError && (
        <div className="mb-4 rounded border border-red-300 bg-red-50 px-4 py-3 text-sm text-red-600">
          {serverError}
        </div>
      )}

      <div className="space-y-4">
        {/* Name */}
        <div className="flex flex-col gap-2 md:flex-row md:items-start md:gap-4">
          <label className="w-full md:w-36 md:shrink-0 pt-2 text-sm font-medium text-gray-700">
            Name
          </label>
          <div className="flex-1">
            <input
              maxLength={100}
              data-testid="txtName"
              type="text"
              value={assetName}
              onChange={(e) => {
                setAssetName(stripEmoji(e.target.value));
              }}
              className="
            h-9
            w-full
            rounded
            border
            border-gray-400
            px-3
            text-sm
            outline-none
            focus:border-primary
          "
            />
            <div className="mt-1 flex justify-between text-xs">
              {assetName.length === 100 ? (
                <span className="text-orange-500">
                  Maximum characters is 100.
                </span>
              ) : (
                <span />
              )}
              <span
                className={
                  assetName.length === 100 ? "text-red-500" : "text-gray-500"
                }
              >
                {assetName.length}/100
              </span>
            </div>
            {fieldErrors.assetName && (
              <p className="mt-1 text-sm text-red-500">
                {fieldErrors.assetName}
              </p>
            )}
          </div>
        </div>

        {/* Category */}
        <div className="flex flex-col gap-2 md:flex-row md:items-start md:gap-4">
          <label className="w-full md:w-36 md:shrink-0 pt-2 text-sm font-medium text-gray-700">
            Category
          </label>
          <div className="flex-1">
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
        </div>
        {/* Specification */}
        <div className="flex flex-col gap-2 md:flex-row md:items-start md:gap-4">
          <label className="w-full md:w-36 md:shrink-0 pt-2 text-sm font-medium text-gray-700">
            Specification
          </label>
          <div className="flex-1">
            <textarea
              data-testid="txaSpecification"
              maxLength={500}
              value={specification}
              onChange={(e) => {
                setSpecification(stripEmoji(e.target.value));
              }}
              rows={4}
              className="
            w-full
            resize-none
            rounded
            border
            border-gray-400
            px-3
            py-2
            text-sm
            outline-none
            focus:border-primary
          "
            />
            <div className="mt-1 flex justify-between text-xs">
              {specification.length === 500 ? (
                <span className="text-orange-500">
                  Maximum characters is 500.
                </span>
              ) : (
                <span />
              )}
              <span
                className={
                  specification.length === 500
                    ? "text-red-500"
                    : "text-gray-500"
                }
              >
                {specification.length}/500
              </span>
            </div>
            {fieldErrors.specification && (
              <p className="mt-1 text-sm text-red-500">
                {fieldErrors.specification}
              </p>
            )}
          </div>
        </div>
        {/* Installed Date */}
        <div className="flex flex-col gap-2 md:flex-row md:items-start md:gap-4">
          <label className="w-full md:w-36 md:shrink-0 pt-2 text-sm font-medium text-gray-700">
            Installed Date
          </label>
          <div className="flex-1">
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
        </div>

        {/* State */}
        <div className="flex flex-col gap-2 md:flex-row md:items-start md:gap-4">
          <label className="w-full md:w-36 md:shrink-0 pt-2 text-sm font-medium text-gray-700">
            State
          </label>
          <div className="flex flex-col gap-2 pt-2">
            <label className="flex items-center gap-2 text-sm cursor-pointer">
              <input
                data-testid="rdoAvailable"
                type="radio"
                name="state"
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
                checked={state === AssetState.NotAvailable}
                onChange={() => setState(AssetState.NotAvailable)}
                className="radio radio-primary radio-sm"
              />
              Not available
            </label>
          </div>
        </div>
      </div>
      <div className="mt-8 flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
        <button
          data-testid="btnSave"
          type="button"
          onClick={handleSave}
          disabled={!isFormValid || isCreating}
          className="
        btn
        btn-primary
        btn-sm
        disabled:cursor-not-allowed
        disabled:opacity-50
      "
        >
          {isCreating ? "Saving..." : "Save"}
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
