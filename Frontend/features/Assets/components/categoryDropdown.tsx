"use client";

import { useState, useRef, useEffect } from "react";
import { useAppDispatch } from "@/lib/redux/hooks";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import type { CategoryItem } from "../assets.types";
import { useCreateCategoryMutation } from "../assets.api";

const toTitleCase = (str: string): string =>
  str.trim().replace(/\b\w/g, (char) => char.toUpperCase());

const toUpperPrefix = (str: string): string => str.trim().toUpperCase();

interface CategoryDropdownProps {
  categories: CategoryItem[];
  isLoading?: boolean;
  value: string;
  onChange: (name: string, prefix?: string) => void;
  error?: string;
}

export default function CategoryDropdown({
  categories,
  isLoading = false,
  value,
  onChange,
  error,
}: CategoryDropdownProps) {
  const dispatch = useAppDispatch();
  const containerRef = useRef<HTMLDivElement>(null);

  const [isOpen, setIsOpen] = useState(false);
  const [showAddForm, setShowAddForm] = useState(false);
  const [newName, setNewName] = useState("");
  const [newPrefix, setNewPrefix] = useState("");
  const [createCategory] = useCreateCategoryMutation();
  // All categories including newly added ones
  const allCategories = categories;

  // Close on outside click
  const resetAddForm = () => {
    setShowAddForm(false);
    setNewName("");
    setNewPrefix("");
  };
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(e.target as Node)
      ) {
        setIsOpen(false);
        resetAddForm();
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleSelectCategory = (cat: { name: string; prefix: string }) => {
    onChange(cat.name, cat.prefix);
    setIsOpen(false);
    resetAddForm();
  };

  const handleConfirm = async () => {
    const parsedName = toTitleCase(newName);
    const parsedPrefix = toUpperPrefix(newPrefix);

    const nameExists = allCategories.some(
      (c) => c.name.toLowerCase() === parsedName.toLowerCase(),
    );

    const prefixExists = allCategories.some(
      (c) => c.prefix.toLowerCase() === parsedPrefix.toLowerCase(),
    );

    const toastErrors: string[] = [];

    if (!parsedName) {
      toastErrors.push("Category name is required.");
    } else if (nameExists) {
      toastErrors.push(
        "Category is already existed. Please enter a different category",
      );
    }

    if (!parsedPrefix) {
      toastErrors.push("Prefix is required.");
    } else if (prefixExists) {
      toastErrors.push(
        "Prefix is already existed. Please enter a different prefix",
      );
    }

    if (toastErrors.length > 0) {
      toastErrors.forEach((msg) =>
        dispatch(enqueueToast({ message: msg, type: ToastType.Error })),
      );
      return;
    }

    try {
      const result = await createCategory({
        categoryName: parsedName,
        categoryPrefix: parsedPrefix,
      }).unwrap();

      onChange(result.name, result.prefix);

      dispatch(
        enqueueToast({
          message: `Category "${result.name}" created successfully.`,
          type: ToastType.Success,
        }),
      );
    } catch {
      dispatch(
        enqueueToast({
          message:
            "Category is already existed. Please enter a different category",
          type: ToastType.Error,
        }),
      );

      dispatch(
        enqueueToast({
          message: "Prefix is already existed. Please enter a different prefix",
          type: ToastType.Error,
        }),
      );
    }
  };

  const handleCancel = () => {
    resetAddForm();
    // ← dropdown stays open, form disappears, fields cleared
  };

  return (
    <div ref={containerRef} className="relative w-full">
      {/* Trigger */}
      <button
        type="button"
        onClick={() => setIsOpen((prev) => !prev)}
        className={`flex h-9 w-full items-center justify-between rounded border px-3 text-sm bg-white ${
          error ? "border-red-500" : "border-gray-400"
        }`}
      >
        <span className={value ? "text-gray-800" : "text-gray-400"}>
          {isLoading ? "Loading..." : value || "Select category"}
        </span>
        <span>▼</span>
      </button>

      {/* Dropdown */}
      {isOpen && (
        <div className="absolute top-10 z-30 w-full rounded border border-gray-300 bg-white shadow-lg">
          {/* Existing + pending categories */}
          {allCategories.map((cat, index) => (
            <div
              key={cat.id || `pending-${index}`}
              onClick={() => handleSelectCategory(cat)}
              className={`cursor-pointer px-3 py-2 text-sm hover:bg-gray-100 ${
                value === cat.name ? "bg-gray-50 font-semibold" : ""
              }`}
            >
              {cat.name}
            </div>
          ))}

          {/* Divider */}
          <div className="border-t border-gray-200" />

          {/* Add new category trigger */}
          {!showAddForm ? (
            <div
              data-testid="lnkAddNewCategory"
              onClick={() => setShowAddForm(true)}
              className="cursor-pointer px-3 py-2 text-sm text-primary underline italic hover:bg-gray-50"
            >
              Add new category
            </div>
          ) : (
            // ─── Inline add form ──────────────────
            <div className="px-3 py-2">
              <div className="flex items-center gap-2">
                <input
                  data-testid="txtAddNewCategoryName"
                  type="text"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  onBlur={(e) => setNewName(toTitleCase(e.target.value))}
                  placeholder="Category name"
                  className="h-8 flex-1 rounded border border-gray-400 px-2 text-sm outline-none"
                  autoFocus
                />
                <input
                  data-testid="txtAddNewCategoryPrefix"
                  type="text"
                  value={newPrefix}
                  onChange={(e) => setNewPrefix(e.target.value.toUpperCase())}
                  placeholder="Prefix"
                  maxLength={10}
                  className="h-8 w-20 rounded border border-gray-400 px-2 text-sm outline-none uppercase"
                />
                <button
                  data-testid="btnAcceptCategory"
                  type="button"
                  onClick={handleConfirm}
                  className="text-primary hover:text-primary/80 font-bold text-lg"
                  title="Confirm"
                >
                  ✓
                </button>
                <button
                  data-testid="btnCancelCategory"
                  type="button"
                  onClick={handleCancel}
                  className="text-gray-500 hover:text-gray-700 font-bold text-lg"
                  title="Cancel"
                >
                  ✕
                </button>
              </div>
            </div>
          )}
        </div>
      )}

      {error && <p className="mt-1 text-xs text-red-500">{error}</p>}
    </div>
  );
}
