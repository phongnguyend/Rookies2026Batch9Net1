"use client";

import { ChevronDown } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { UserRoles } from "@/features/users/users.types";

const roleOptions = [
  { label: "Staff", value: UserRoles.Staff },
  { label: "Admin", value: UserRoles.Admin },
];

interface UserTypeDropdownProps {
  value: UserRoles;
  onChange: (value: UserRoles) => void;
  disabled?: boolean;
  width?: string;
  testId?: string;
}

export default function UserTypeDropdown({
  value,
  onChange,
  disabled = false,
  width = "w-[265px]",
  testId,
}: UserTypeDropdownProps) {
  const [isOpen, setIsOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);
  const selectedLabel =
    roleOptions.find((option) => option.value === value)?.label ?? "Select";

  useEffect(() => {
    const handleClickOutside = (event: PointerEvent) => {
      if (ref.current && !ref.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener("pointerdown", handleClickOutside);
    return () => document.removeEventListener("pointerdown", handleClickOutside);
  }, []);

  return (
    <div ref={ref} className="relative">
      <button
        type="button"
        data-testid={testId}
        disabled={disabled}
        onClick={() => setIsOpen((prev) => !prev)}
        className={`hover:cursor-pointer flex h-9 items-center justify-between rounded border border-gray-400 bg-white px-3 text-left text-sm text-gray-700 outline-none disabled:cursor-not-allowed disabled:bg-gray-100 disabled:text-gray-500 ${width}`}
      >
        <span className="truncate">{selectedLabel}</span>
        <span>
          <ChevronDown
            size={16}
            className={`shrink-0 transition-transform duration-200 ${
              isOpen ? "rotate-180" : ""
            }`}
          />
        </span>
      </button>

      {isOpen && !disabled && (
        <div
          className={`absolute top-9 z-30 rounded border border-gray-300 bg-white py-1 shadow ${width}`}
        >
          {roleOptions.map((option) => (
            <label
              key={option.value}
              className="flex cursor-pointer items-center gap-2 px-2 py-1 text-sm hover:bg-gray-100"
            >
              <input
                type="checkbox"
                checked={value === option.value}
                onChange={() => {
                  onChange(option.value);
                  setIsOpen(false);
                }}
                className="checkbox checkbox-xs"
              />
              {option.label}
            </label>
          ))}
        </div>
      )}
    </div>
  );
}
