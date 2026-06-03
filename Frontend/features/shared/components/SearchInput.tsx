"use client";

import { ReactNode } from "react";

interface SearchInputProps {
  value: string;
  onChange: (value: string) => void;
  onSearch?: (value: string) => void;
  placeholder?: string;
  width?: string;
  txtInputTestId?: string;
  btnSearchTestId?: string;
  searchIcon?: ReactNode;
}

export default function SearchInput({
  value,
  onChange,
  onSearch,
  placeholder = "Search...",
  width = "w-full sm:w-60",
  txtInputTestId = "txtSearch",
  btnSearchTestId = "btnSearch",
  searchIcon = "🔍"
}: SearchInputProps) {
  return (
    <div
      className={`flex h-9 w-full items-center rounded border border-gray-400 bg-white sm:max-w-xs ${width}`}
    >
      <input
        type="text"
        maxLength={100}
        value={value}
        placeholder={placeholder}
        onChange={(e) => onChange(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === "Enter") {
            onSearch?.(value);
          }
        }}
        className="h-full flex-1 text-sm outline-none pl-3"
        data-testid={txtInputTestId}
      />

      <button
        type="button"
        onClick={() => onSearch?.(value)}
        className="flex h-full aspect-square shrink-0 items-center justify-center rounded border-l border-gray-400 text-gray-600 hover:bg-gray-50 hover:cursor-pointer"
        data-testid={btnSearchTestId}
      >
        {searchIcon}
      </button>
    </div>
  );
}
