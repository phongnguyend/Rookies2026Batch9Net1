"use client";

import { useState, useRef, useEffect } from "react";
import type { CategoryItem } from "../assets.types";

// ─── Helpers ──────────────────────────────────────────
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
  const [isOpen, setIsOpen] = useState(false);
  const [showAddForm, setShowAddForm] = useState(false);
  const [newName, setNewName] = useState("");
  const [newPrefix, setNewPrefix] = useState("");
  const [addErrors, setAddErrors] = useState<{
    name?: string;
    prefix?: string;
  }>({});
  const containerRef = useRef<HTMLDivElement>(null);

  // Close dropdown on outside click
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(e.target as Node)
      ) {
        setIsOpen(false);
        setShowAddForm(false);
        setNewName("");
        setNewPrefix("");
        setAddErrors({});
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleSelectCategory = (name: string) => {
    onChange(name, undefined); // existing category — no prefix
    setIsOpen(false);
    setShowAddForm(false);
  };

  const handleConfirmNewCategory = () => {
    const errors: { name?: string; prefix?: string } = {};

    const parsedName = toTitleCase(newName);
    const parsedPrefix = toUpperPrefix(newPrefix);

    // Name validation
    if (!parsedName.trim()) {
      errors.name = "Name is required.";
    } else if (parsedName.length > 20) {
      errors.name = "Category name must not exceed 20 characters.";
    }

    // Prefix validation
    if (!parsedPrefix.trim()) {
      errors.prefix = "Prefix is required.";
    } else if (parsedPrefix.length > 2) {
      errors.prefix = "Category prefix must not exceed 2 characters.";
    } else if (!/^[A-Z]+$/.test(parsedPrefix)) {
      errors.prefix = "Category prefix must contain uppercase letters only.";
    }

    if (Object.keys(errors).length > 0) {
      setAddErrors(errors);
      return;
    }

    onChange(parsedName, parsedPrefix);

    setIsOpen(false);
    setShowAddForm(false);
    setNewName("");
    setNewPrefix("");
    setAddErrors({});
  };

  const handleCancelNewCategory = () => {
    setShowAddForm(false);
    setNewName("");
    setNewPrefix("");
    setAddErrors({});
  };

  return (
    <div ref={containerRef} className="relative w-full">
      {/* Trigger */}
      <button
        data-testid="ddlCategory"
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
          {/* Category list */}
          {categories.map((cat) => (
            <div
              key={cat.id}
              onClick={() => handleSelectCategory(cat.name)}
              className={`cursor-pointer px-3 py-2 text-sm hover:bg-gray-100 ${
                value === cat.name ? "bg-gray-50 font-semibold" : ""
              }`}
            >
              {cat.name}
            </div>
          ))}

          {/* Divider */}
          <div className="border-t border-gray-200" />

          {/* Add new category */}
          {!showAddForm ? (
            <div
              data-testid="lnkAddNewCategory"
              onClick={() => setShowAddForm(true)}
              className="cursor-pointer px-3 py-2 text-sm text-primary underline italic hover:bg-gray-50"
            >
              Add new category
            </div>
          ) : (
            <div className="px-3 py-2">
              <div className="flex items-center gap-2">
                {/* Name input */}
                <div className="flex-1">
                  <input
                    data-testid="txtAddNewCategoryName"
                    type="text"
                    value={newName}
                    // maxLength={20}
                    onChange={(e) => {
                      setNewName(e.target.value);
                      setAddErrors((prev) => ({ ...prev, name: undefined }));
                    }}
                    onBlur={(e) => setNewName(toTitleCase(e.target.value))}
                    placeholder="Category name"
                    className={`h-8 w-full rounded border px-2 text-sm outline-none ${
                      addErrors.name ? "border-red-500" : "border-gray-400"
                    }`}
                  />
                </div>

                {/* Prefix input */}
                <div className="w-20">
                  <input
                    data-testid="txtAddNewCategoryPrefix"
                    type="text"
                    value={newPrefix}
                    onChange={(e) => {
                      const value = e.target.value
                        .toUpperCase()
                        .replace(/[^A-Z]/g, "");

                      setNewPrefix(value);
                      setAddErrors((prev) => ({ ...prev, prefix: undefined }));
                    }}
                    placeholder="Prefix"
                    // maxLength={2}
                    className={`h-8 w-full rounded border px-2 text-sm outline-none uppercase ${
                      addErrors.prefix ? "border-red-500" : "border-gray-400"
                    }`}
                  />
                </div>

                {/* Confirm */}
                <button
                  data-testid="btnAcceptCategory"
                  type="button"
                  onClick={handleConfirmNewCategory}
                  className="text-primary hover:text-primary/80 font-bold text-lg"
                  title="Confirm"
                >
                  ✓
                </button>

                {/* Cancel */}
                <button
                  data-testid="btnCancelCategory"
                  type="button"
                  onClick={handleCancelNewCategory}
                  className="text-gray-500 hover:text-gray-700 font-bold text-lg"
                  title="Cancel"
                >
                  ✕
                </button>
              </div>

              {/* Inline errors */}
              {addErrors.name && (
                <p className="mt-1 text-xs text-red-500">{addErrors.name}</p>
              )}
              {addErrors.prefix && (
                <p className="mt-1 text-xs text-red-500">{addErrors.prefix}</p>
              )}
            </div>
          )}
        </div>
      )}

      {/* Field error */}
      {error && <p className="mt-1 text-xs text-red-500">{error}</p>}
    </div>
  );
}
