"use client";

import { Month } from "@/lib/api/base.types";
import { useAppDispatch } from "@/lib/redux/hooks";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { useMemo, useRef, useState, useEffect } from "react";
import {
  Calendar,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  X,
} from "lucide-react";

interface DatePickerInputProps {
  value: Date | null | undefined;
  onChange: (date: Date | null) => void;
  onBlur?: () => void;
  placeholder?: string;
  width?: string;
  txtInputTestId?: string;
  canClearValue?: boolean;
  error?: string;
  showToast?: boolean;
}

const MONTHS = Object.values(Month);
const WEEKDAYS = ["S", "M", "T", "W", "T", "F", "S"];

const DATE_REGEX = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;

const parseDate = (input: string): Date | null => {
  if (!DATE_REGEX.test(input)) return null;

  const [day, month, year] = input.split("/").map(Number);
  const date = new Date(year, month - 1, day);

  const isValidDate =
    date.getFullYear() === year &&
    date.getMonth() === month - 1 &&
    date.getDate() === day;

  if (!isValidDate) return null;

  const currentYear = new Date().getFullYear();

  if (year < currentYear - 100 || year > currentYear + 100) {
    return null;
  }

  return date;
};

export default function DatePickerInput({
  value,
  onChange,
  onBlur,
  placeholder = "Assigned Date",
  width = "w-full sm:w-64",
  txtInputTestId = "txtDatePicker",
  canClearValue = true,
  showToast = true,
  error,
}: DatePickerInputProps) {
  const today = new Date();

  const [open, setOpen] = useState(false);
  const [cursor, setCursor] = useState(value ?? today);

  const [inputError, setInputError] = useState("");
  const dispatch = useAppDispatch();

  const ref = useRef<HTMLDivElement>(null);

  const year = cursor.getFullYear();
  const month = cursor.getMonth();

  // handle click outside for calendar modal
  useEffect(() => {
    const handleClickOutside = (e: PointerEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };

    document.addEventListener("pointerdown", handleClickOutside);
    return () =>
      document.removeEventListener("pointerdown", handleClickOutside);
  }, []);

  const formatDate = (date: Date | null) => {
    if (!date) return "";

    const day = String(date.getDate()).padStart(2, "0");
    const monthValue = String(date.getMonth() + 1).padStart(2, "0");

    return `${day}/${monthValue}/${date.getFullYear()}`;
  };

  const [inputValue, setInputValue] = useState(formatDate(value!));
  useEffect(() => {
    setInputValue(formatDate(value!));

    if (value) {
      setCursor(value);
    }
  }, [value]);

  const isSameDay = (a: Date, b: Date | null) =>
    !!b &&
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate();

  const days = useMemo(() => {
    const firstDay = new Date(year, month, 1).getDay();
    const totalDays = new Date(year, month + 1, 0).getDate();
    const prevMonthTotalDays = new Date(year, month, 0).getDate();

    const result: { date: Date; inMonth: boolean }[] = [];

    for (let i = firstDay - 1; i >= 0; i--) {
      result.push({
        date: new Date(year, month - 1, prevMonthTotalDays - i),
        inMonth: false,
      });
    }

    for (let day = 1; day <= totalDays; day++) {
      result.push({
        date: new Date(year, month, day),
        inMonth: true,
      });
    }

    while (result.length < 42) {
      const lastDate = result[result.length - 1].date;

      result.push({
        date: new Date(
          lastDate.getFullYear(),
          lastDate.getMonth(),
          lastDate.getDate() + 1,
        ),
        inMonth: false,
      });
    }

    return result;
  }, [year, month]);

  const selectDate = (date: Date) => {
    onChange(date);
    setInputValue(formatDate(date));
    setCursor(date);
    setInputError("");
    setOpen(false);
  };

  const handleDateValidation = () => {
    if (!inputValue.trim()) {
      onChange(null);
      setInputError("");
      return;
    }

    const parsedDate = parseDate(inputValue);

    if (!parsedDate) {
      setInputError("Date must be valid and follow dd/MM/yyyy format");
      if (showToast) {
        dispatch(
          enqueueToast({
            message: "Date must be valid and follow dd/MM/yyyy format.",
            type: ToastType.Error,
            testId: "txtDateValidationError",
          }),
        );
      }

      return;
    }

    onChange(parsedDate);
    setCursor(parsedDate);
    setInputError("");
  };

  return (
    <div ref={ref} className={`relative ${width}`}>
      <div
        className={`flex h-10 overflow-hidden rounded border bg-base-100 ${
          error || inputError ? "border-error" : "border-gray-400"
        }`}
      >
        {/* Date Picker Input */}
        <div className="relative flex-1">
          <input
            data-testid={txtInputTestId}
            value={inputValue}
            placeholder={placeholder}
            onChange={(e) => {
              setInputValue(e.target.value);
              setInputError("");
            }}
            onBlur={handleDateValidation}
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                e.preventDefault();
                handleDateValidation();
              }
            }}
            className={`h-full w-full px-3 pr-8 text-sm outline-none ${
              inputError ? "text-error" : ""
            }`}
          />

          {inputValue && canClearValue && (
            <button
              type="button"
              data-testid="btnClearAssignedDate"
              onMouseDown={(e) => e.preventDefault()}
              onClick={() => {
                setInputValue("");
                setInputError("");
                setCursor(new Date());
                onChange(null);
                onChange(null);
              }}
              className="absolute right-2 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
            >
              <X size={16} className="hover:cursor-pointer"/>
            </button>
          )}
        </div>

        {/* Button Calendar */}
        <button
          data-testid="btnAssignedDateCalendar"
          type="button"
          onClick={() => setOpen((prev) => !prev)}
          className="flex h-full w-11 shrink-0 items-center justify-center border-l border-gray-400 bg-base-200 text-gray-500 hover:bg-base-300 hover:cursor-pointer"
        >
          {<Calendar size={20} />}
        </button>
      </div>
      {(inputError || error) && !showToast && (
        <p className="mt-1 text-sm text-error">{inputError || error}</p>
      )}

      {open && (
        <div className="absolute left-0 top-9.5 z-50 w-[320px] overflow-hidden rounded-sm border border-gray-400 bg-base-100 shadow">
          {/* Year and Month Navigator */}
          <div className="grid h-12 grid-cols-[1fr_auto_1fr] items-center border-b border-base-200 px-4">
            <div className="flex gap-2">
              <button
                data-testid="btnPreviousYear"
                type="button"
                onClick={() => setCursor(new Date(year - 1, month, 1))}
                className="text-lg leading-none text-gray-300 transition hover:text-gray-500 hover:cursor-pointer"
              >
                {<ChevronsLeft size={20} />}
              </button>

              <button
                data-testid="btnPreviousMonth"
                type="button"
                onClick={() => setCursor(new Date(year, month - 1, 1))}
                className="text-lg leading-none text-gray-300 transition hover:text-gray-500 hover:cursor-pointer"
              >
                {<ChevronLeft size={20} />}
              </button>
            </div>

            <div className="text-base font-normal text-gray-700">
              {MONTHS[month]} {year}
            </div>

            <div className="flex justify-end gap-2">
              <button
                data-testid="btnNextMonth"
                type="button"
                onClick={() => setCursor(new Date(year, month + 1, 1))}
                className="text-lg leading-none text-gray-300 transition hover:text-gray-500 hover:cursor-pointer"
              >
                {<ChevronRight size={20} />}
              </button>

              <button
                data-testid="btnNextYear"
                type="button"
                onClick={() => setCursor(new Date(year + 1, month, 1))}
                className="text-lg leading-none text-gray-300 transition hover:text-gray-500 hover:cursor-pointer"
              >
                {<ChevronsRight size={20} />}
              </button>
            </div>
          </div>

          <div className="grid grid-cols-7 px-4 pb-2 pt-4 text-center text-sm font-medium text-gray-500">
            {WEEKDAYS.map((day, index) => (
              <div key={`${day}-${index}`}>{day}</div>
            ))}
          </div>

          <div className="grid grid-cols-7 px-4 pb-4 text-center">
            {days.map(({ date, inMonth }, index) => {
              const selected = isSameDay(date, value!);
              const currentDay = isSameDay(date, today);

              return (
                <button
                  key={index}
                  type="button"
                  onClick={() => selectDate(date)}
                  className={[
                    "mx-auto my-0.5 h-8 w-8 rounded text-sm leading-8 transition",
                    selected
                      ? "bg-primary text-primary-content"
                      : currentDay
                        ? "text-error"
                        : inMonth
                          ? "text-gray-600 hover:bg-base-200"
                          : "text-gray-300",
                  ].join(" ")}
                >
                  {date.getDate()}
                </button>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}

