"use client";

import { useState, useRef, useEffect } from "react";
import { useAppDispatch } from "@/lib/redux/hooks";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import type { CategoryItem } from "../assets.types";
import { useCreateCategoryMutation } from "../assets.api";
import { Check, ChevronDown, X } from "lucide-react";
import { FocusTrap } from "focus-trap-react";

const toTitleCase = (str: string): string =>
  str.trim().replace(/\b\w/g, (char) => char.toUpperCase());

const toUpperPrefix = (str: string): string => str.trim().toUpperCase();

interface CategoryDropdownProps {
  categories: CategoryItem[];
  isLoading?: boolean;
  value: string; // ← display value (category name)
  onChange: (id: string, name: string) => void; // ← passes id + name
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
  const [createCategory] = useCreateCategoryMutation();

  const [isOpen, setIsOpen] = useState(false);
  const [showAddForm, setShowAddForm] = useState(false);
  const [newName, setNewName] = useState("");
  const [newPrefix, setNewPrefix] = useState("");

  const categoryNameRegex = /^(?=.*\p{L})[\p{L}\p{N}\s]+$/u;
  const prefixRegex = /^[A-Z]{2}$/;
  const emojiRegex = /(\p{Extended_Pictographic}|\p{Emoji_Component})/gu;

  const isValidCategoryName = (value: string) => {
    const withoutEmoji = value.replace(emojiRegex, "");
    return withoutEmoji === value && categoryNameRegex.test(value);
  };

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

  // ─── Select existing category ──────────────────────
  const handleSelectCategory = (cat: CategoryItem) => {
    onChange(cat.id, cat.name); // ← pass id + name to parent
    setIsOpen(false);
    resetAddForm();
  };

  // ─── Confirm new category ──────────────────────────
  const handleConfirm = async () => {
    const parsedName = toTitleCase(newName);
    const parsedPrefix = toUpperPrefix(newPrefix);

    // Client-side duplicate check
    const nameExists = categories.some(
      (c) => c.name.toLowerCase() === parsedName.toLowerCase(),
    );
    const prefixExists = categories.some(
      (c) => c.prefix.toLowerCase() === parsedPrefix.toLowerCase(),
    );

    const toastErrors: string[] = [];

    if (!parsedName) {
      toastErrors.push("Category name is required.");
    } else if (!isValidCategoryName(parsedName)) {
      toastErrors.push("Category name must contain letters only.");
    } else if (nameExists) {
      toastErrors.push(
        "Category is already existed. Please enter a different category",
      );
    }

    if (!parsedPrefix) {
      toastErrors.push("Prefix is required.");
    } else if (!prefixRegex.test(parsedPrefix)) {
      toastErrors.push("Prefix must contain 2 letters.");
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

      onChange(result.id, result.name); // ← pass id + name from API response
      setIsOpen(false);
      dispatch(
        enqueueToast({
          message: `Category "${result.name}" created successfully.`,
          type: ToastType.Success,
        }),
      );

      resetAddForm();
    } catch {
      dispatch(
        enqueueToast({
          message: "Somethings is wrong please try again.",
          type: ToastType.Error,
        }),
      );
    }
  };

  return (
    <FocusTrap
      active={isOpen}
      focusTrapOptions={{
        escapeDeactivates: false,
        allowOutsideClick: true,
        returnFocusOnDeactivate: true,
      }}
    >
      <div ref={containerRef} className="relative w-full">
        {/* Trigger */}
        <button
          data-testid="ddlCategory"
          type="button"
          onClick={() => setIsOpen((prev) => !prev)}
          className="hover:cursor-pointer flex h-9 items-center justify-between rounded border border-gray-400 px-3 w-full"
        >
          <div className="flex w-full items-center justify-between">
            <span className="truncate">
              {isLoading ? "Loading..." : value || "Select category"}
            </span>
            <ChevronDown
              size={16}
              className={`shrink-0 transition-transform duration-200 ${isOpen ? "rotate-180" : ""}`}
            />
          </div>
        </button>

        {/* Dropdown */}
        {isOpen && (
          <div className="absolute left-0 top-12 z-30 max-h-60 w-full overflow-y-auto rounded-box border border-base-300 bg-base-100 shadow-lg">
            {categories.map((cat) => (
              <button
                key={cat.id}
                type="button"
                onClick={() => handleSelectCategory(cat)}
                className={`w-full text-left cursor-pointer px-4 py-2 text-sm hover:bg-base-200 ${value === cat.name ? "bg-base-200" : ""
                  }`}
              >
                {cat.name}
              </button>
            ))}

            <div className="divider my-0" />

            {!showAddForm ? (
              <button
                data-testid="lnkAddNewCategory"
                onClick={() => setShowAddForm(true)}
                className="cursor-pointer px-4 py-2 text-sm italic text-primary hover:bg-base-200"
              >
                Add new category
              </button>
            ) : (
              <div className="p-3">
                <div className="flex flex-col gap-2 sm:flex-row">
                  <input
                    maxLength={20}
                    data-testid="txtAddNewCategoryName"
                    type="text"
                    value={newName}
                    onChange={(e) =>
                      setNewName(
                        e.target.value.replace(
                          /(\p{Extended_Pictographic}|\p{Emoji_Component})/gu,
                          "",
                        ),
                      )
                    }
                    onBlur={(e) => setNewName(toTitleCase(e.target.value))}
                    placeholder="Category name"
                    className="
                    input
                    input-bordered
                    input-sm
                    w-full
                    focus:outline-none
                    focus:ring-0
                    focus:border-gray-400
                  "
                    autoFocus
                  />

                  <input
                    data-testid="txtAddNewCategoryPrefix"
                    maxLength={2}
                    type="text"
                    value={newPrefix}
                    onChange={(e) =>
                      setNewPrefix(
                        e.target.value.replace(/[^a-zA-Z]/g, "").toUpperCase(),
                      )
                    }
                    placeholder="Prefix"
                    className="
                    input
                    input-bordered
                    input-sm
                    w-full
                    sm:w-20
                    uppercase
                    focus:outline-none
                    focus:ring-0
                    focus:border-gray-400
                  "
                  />

                  <div className="flex gap-2 sm:self-center">
                    <button
                      data-testid="btnAcceptCategory"
                      type="button"
                      onClick={handleConfirm}
                      className="btn btn-primary btn-sm"
                    >
                      <Check size={16} />
                    </button>

                    <button
                      data-testid="btnCancelCategory"
                      type="button"
                      onClick={resetAddForm}
                      className="btn btn-ghost btn-sm"
                    >
                      <X size={16} />
                    </button>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}

        {error && <p className="mt-1 text-xs text-error">{error}</p>}
      </div>
    </FocusTrap>
  );
}
