"use client";

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
}

export default function UserTypeDropdown({
  value,
  onChange,
  disabled = false,
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
    <div ref={ref} className="relative w-[265px]">
      <button
        type="button"
        disabled={disabled}
        onClick={() => setIsOpen((prev) => !prev)}
        className="flex h-[33px] w-full items-center justify-between rounded border border-gray-400 bg-white px-3 text-left text-sm text-gray-700 outline-none transition hover:border-gray-500 disabled:cursor-not-allowed disabled:bg-gray-100 disabled:text-gray-500"
      >
        <span>{selectedLabel}</span>
        <span className="text-gray-500">v</span>
      </button>

      {isOpen && !disabled && (
        <div className="absolute left-0 top-[36px] z-30 w-full overflow-hidden rounded border border-gray-300 bg-white shadow">
          {roleOptions.map((option) => (
            <button
              key={option.value}
              type="button"
              onClick={() => {
                onChange(option.value);
                setIsOpen(false);
              }}
              className={`block w-full px-3 py-2 text-left text-sm hover:bg-gray-100 ${
                value === option.value ? "bg-gray-50 font-medium" : ""
              }`}
            >
              {option.label}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
