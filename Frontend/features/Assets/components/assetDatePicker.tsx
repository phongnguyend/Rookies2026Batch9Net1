"use client";

import { useEffect, useMemo, useRef, useState } from "react";

interface DatePickerInputProps {
  value: Date | null;
  onChange: (date: Date | null) => void;
  placeholder?: string;
  width?: string;
}

const WEEKDAYS = ["S", "M", "T", "W", "T", "F", "S"];

const MONTHS = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
];

const formatDate = (date: Date | null) => {
  if (!date || isNaN(date.getTime())) return "";

  return `${String(date.getDate()).padStart(2, "0")}/${String(
    date.getMonth() + 1,
  ).padStart(2, "0")}/${date.getFullYear()}`;
};

const isSameDay = (a: Date, b: Date | null) =>
  !!b &&
  a.getDate() === b.getDate() &&
  a.getMonth() === b.getMonth() &&
  a.getFullYear() === b.getFullYear();

export default function DatePickerInput({
  value,
  onChange,
  placeholder = "dd/MM/yyyy",
  width = "w-full",
}: DatePickerInputProps) {
  const today = new Date();

  const [open, setOpen] = useState(false);
  const [cursor, setCursor] = useState(value ?? today);
  const [inputValue, setInputValue] = useState(formatDate(value));

  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setInputValue(formatDate(value));
    if (value) {
      setCursor(value);
    }
  }, [value]);

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };

    document.addEventListener("mousedown", handler);

    return () => document.removeEventListener("mousedown", handler);
  }, []);

  const days = useMemo(() => {
    const year = cursor.getFullYear();
    const month = cursor.getMonth();

    const firstDay = new Date(year, month, 1).getDay();
    const totalDays = new Date(year, month + 1, 0).getDate();

    const result: Date[] = [];

    for (let i = firstDay - 1; i >= 0; i--) {
      result.push(new Date(year, month - 1, i + 1));
    }

    for (let i = 1; i <= totalDays; i++) {
      result.push(new Date(year, month, i));
    }

    while (result.length < 35) {
      const last = result[result.length - 1];

      result.push(
        new Date(last.getFullYear(), last.getMonth(), last.getDate() + 1),
      );
    }

    return result;
  }, [cursor]);

  const selectDate = (date: Date) => {
    setInputValue(formatDate(date));
    setCursor(date);
    onChange(date);
    setOpen(false);
  };

  return (
    <div ref={ref} className={`relative ${width}`}>
      <div
        className="
          flex
          h-10
          rounded
          border
          border-gray-400
          overflow-hidden
          bg-white
        "
      >
        <input
          value={inputValue}
          placeholder={placeholder}
          onChange={(e) => {
            setInputValue(e.target.value);
          }}
          className="
            flex-1
            px-3
            text-sm
            outline-none
          "
        />

        <button
          type="button"
          onClick={() => setOpen(!open)}
          className="
            w-10
            flex
            items-center
            justify-center
            border-l
            border-gray-300
            bg-gray-100
          "
        >
          {/* same style as your picture */}
          <svg
            className="h-4 w-4 text-gray-500"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <rect x="3" y="4" width="18" height="18" rx="2" />

            <line x1="8" y1="2" x2="8" y2="6" />

            <line x1="16" y1="2" x2="16" y2="6" />

            <line x1="3" y1="10" x2="21" y2="10" />
          </svg>
        </button>
      </div>
      {open && (
        <div
          className="
            absolute
            z-50
            mt-1
            w-[300px]
            rounded
            border
            bg-white
            shadow
            p-3
          "
        >
          <div
            className="
              flex
              justify-between
              items-center
              mb-3
            "
          >
            <button
              onClick={() =>
                setCursor(new Date(cursor.getFullYear(), cursor.getMonth() - 1))
              }
            >
              ‹
            </button>
            <span className="text-sm font-medium">
              {MONTHS[cursor.getMonth()]} {cursor.getFullYear()}
            </span>
            <button
              onClick={() =>
                setCursor(new Date(cursor.getFullYear(), cursor.getMonth() + 1))
              }
            >
              ›
            </button>
          </div>
          <div className="grid grid-cols-7 text-center text-xs mb-2">
            {WEEKDAYS.map((d) => (
              <div key={d}>{d}</div>
            ))}
          </div>
          <div className="grid grid-cols-7 gap-1">
            {days.map((date) => (
              <button
                key={date.toISOString()}
                onClick={() => selectDate(date)}
                className={`
                  h-8
                  rounded
                  text-sm
                  ${
                    isSameDay(date, value)
                      ? "bg-primary text-white"
                      : "hover:bg-gray-100"
                  }
                `}
              >
                {date.getDate()}
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
