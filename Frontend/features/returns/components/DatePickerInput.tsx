"use client";

import { useRef } from "react";
import DatePicker from "react-datepicker";

interface DatePickerInputProps {
  value: Date | null;
  onChange: (date: Date | null) => void;
  placeholder?: string;
  width?: string;
  testId?: string;
}

export default function DatePickerInput({
  value,
  onChange,
  placeholder = "Select Date",
  width = "w-56",
  testId,
}: DatePickerInputProps) {
  const datePickerRef = useRef<DatePicker>(null);

  return (
    <div className={width}>
      <div className="relative">
        <DatePicker
          ref={datePickerRef}
          selected={value}
          onChange={onChange}
          maxDate={new Date()}
          placeholderText={placeholder}
          dateFormat="dd/MM/yyyy"
          showMonthDropdown
          showYearDropdown
          scrollableYearDropdown
          yearDropdownItemNumber={80}
          dropdownMode="select"
          className="h-9 w-full rounded border border-gray-400 px-3 pr-10"
          data-testid={testId}
        />
        <button
          type="button"
          aria-label="Open calendar"
          onClick={() => datePickerRef.current?.setOpen(true)}
          className="absolute right-2 top-1/2 flex h-6 w-6 -translate-y-1/2 items-center justify-center text-gray-500 hover:text-primary"
        >
          <svg
            aria-hidden="true"
            viewBox="0 0 24 24"
            className="h-4 w-4"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <path d="M8 2v4" />
            <path d="M16 2v4" />
            <rect width="18" height="18" x="3" y="4" rx="2" />
            <path d="M3 10h18" />
          </svg>
        </button>
      </div>
    </div>
  );
}
