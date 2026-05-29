"use client";

import {
  useState,
  useRef,
  useEffect,
  useMemo,
  KeyboardEvent,
  ChangeEvent,
} from "react";

interface DatePickerInputProps {
  value?: Date | null;
  onChange?: (date: Date | null) => void;
  placeholder?: string;
  label?: string;
  width?: string;
  txtInputTestId?: string;
}

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

const WEEKDAYS = ["S", "M", "T", "W", "T", "F", "S"];

type View = "day" | "month" | "year";

function formatDate(date: Date) {
  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0");

  return `${day}/${month}/${date.getFullYear()}`;
}

function parseDate(value: string) {
  const parts = value.split("/");

  if (parts.length !== 3) {
    return null;
  }

  const day = Number(parts[0]);
  const month = Number(parts[1]) - 1;
  const year = Number(parts[2]);

  const parsed = new Date(year, month, day);

  if (
    parsed.getFullYear() !== year ||
    parsed.getMonth() !== month ||
    parsed.getDate() !== day
  ) {
    return null;
  }

  return parsed;
}

function CalendarIcon() {
  return (
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
  );
}

export default function DatePickerInput({
  value,
  onChange,
  placeholder = "dd/MM/yyyy",
  label,
  width = "w-64",
  txtInputTestId = "dpReturned",
}: DatePickerInputProps) {
  const today = new Date();

  const [open, setOpen] = useState(false);
  const [view, setView] = useState<View>("day");
  const [cursor, setCursor] = useState<Date>(() => value ?? today);

  const [inputValue, setInputValue] = useState<string | null>(null);

  const displayValue = inputValue ?? (value ? formatDate(value) : "");

  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handler = (e: PointerEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };

    document.addEventListener("pointerdown", handler);

    return () => {
      document.removeEventListener("pointerdown", handler);
    };
  }, []);

  const year = cursor.getFullYear();
  const month = cursor.getMonth();

  const days = useMemo(() => {
    const first = new Date(year, month, 1);
    const startDay = first.getDay();

    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const prevMonthDays = new Date(year, month, 0).getDate();

    const cells: { date: Date; inMonth: boolean }[] = [];

    for (let i = startDay - 1; i >= 0; i--) {
      cells.push({
        date: new Date(year, month - 1, prevMonthDays - i),
        inMonth: false,
      });
    }

    for (let d = 1; d <= daysInMonth; d++) {
      cells.push({
        date: new Date(year, month, d),
        inMonth: true,
      });
    }

    while (cells.length < 42) {
      const last = cells[cells.length - 1].date;

      cells.push({
        date: new Date(
          last.getFullYear(),
          last.getMonth(),
          last.getDate() + 1,
        ),
        inMonth: false,
      });
    }

    return cells;
  }, [year, month]);

  const isSameDay = (a: Date, b?: Date | null) =>
    !!b &&
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate();

  const selectDate = (date: Date) => {
    setCursor(date);
    setInputValue(null);

    onChange?.(date);

    setOpen(false);
  };

  const commitInputValue = () => {
    if (!inputValue?.trim()) {
      setInputValue(null);
      onChange?.(null);
      return;
    }

    const parsed = parseDate(inputValue);

    if (!parsed) {
      setInputValue(null);
      return;
    }

    setCursor(parsed);
    setInputValue(null);

    onChange?.(parsed);
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    setInputValue(e.target.value);
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      commitInputValue();
      setOpen(false);
    }
  };

  const clearDate = () => {
    setCursor(today);
    setInputValue(null);

    onChange?.(null);

    setOpen(false);
  };

  const yearRangeStart = Math.floor(year / 12) * 12;

  return (
    <div ref={ref} className={`relative inline-block ${width}`}>
      {label && (
        <label className="mb-1 block text-sm font-medium text-gray-700">
          {label}
        </label>
      )}

      <div className="flex h-9 items-center gap-1 rounded border border-gray-400 bg-white px-2 shadow-sm">
        <input
          type="text"
          value={displayValue}
          onChange={handleInputChange}
          onBlur={commitInputValue}
          onKeyDown={handleKeyDown}
          placeholder={placeholder}
          className="min-w-0 flex-1 bg-transparent text-sm text-gray-700 outline-none"
          data-testid={txtInputTestId}
        />

        {value && (
          <button
            type="button"
            onClick={clearDate}
            className="flex h-5 w-5 items-center justify-center rounded text-xs text-gray-400 hover:bg-gray-100 hover:text-gray-600"
            aria-label="Clear date"
          >
            x
          </button>
        )}

        <button
          type="button"
          aria-expanded={open}
          aria-haspopup="dialog"
          onClick={() => {
            if (value) {
              setCursor(value);
            }

            setOpen((prev) => !prev);
          }}
          className="flex h-6 w-6 items-center justify-center rounded text-gray-500 hover:bg-gray-100"
          aria-label="Open calendar"
          data-testid="btnAssignedDateCalendar"
        >
          📅
        </button>
      </div>

      {open && (
        <div className="absolute z-50 mt-2 w-72 rounded-lg border border-gray-200 bg-white p-3 shadow-lg">
          <div className="mb-2 flex items-center justify-between">
            <div className="flex items-center gap-1">
              <button
                type="button"
                onClick={() => setCursor(new Date(year - 1, month, 1))}
                className="rounded p-1 text-gray-500 hover:bg-gray-100"
                data-testid="btnPreviousYear"
              >
                {"<<"}
              </button>

              {view === "day" && (
                <button
                  type="button"
                  onClick={() => setCursor(new Date(year, month - 1, 1))}
                  className="rounded p-1 text-gray-500 hover:bg-gray-100"
                  data-testid="btnPreviousMonth"
                >
                  {"<"}
                </button>
              )}
            </div>

            <button
              type="button"
              onClick={() =>
                setView(
                  view === "day"
                    ? "month"
                    : view === "month"
                      ? "year"
                      : "day",
                )
              }
              className="rounded px-2 py-1 text-sm font-medium text-gray-700 hover:bg-gray-100"
            >
              {view === "day" && `${MONTHS[month]} ${year}`}
              {view === "month" && `${year}`}
              {view === "year" &&
                `${yearRangeStart} - ${yearRangeStart + 11}`}
            </button>

            <div className="flex items-center gap-1">
              {view === "day" && (
                <button
                  type="button"
                  onClick={() => setCursor(new Date(year, month + 1, 1))}
                  className="rounded p-1 text-gray-500 hover:bg-gray-100"
                  data-testid="btnNextMonth"
                >
                  {">"}
                </button>
              )}

              <button
                type="button"
                onClick={() => setCursor(new Date(year + 1, month, 1))}
                className="rounded p-1 text-gray-500 hover:bg-gray-100"
                data-testid="btnNextYear"
              >
                {">>"}
              </button>
            </div>
          </div>

          {view === "day" && (
            <>
              <div className="mb-1 grid grid-cols-7 text-center text-xs font-medium text-gray-400">
                {WEEKDAYS.map((day, index) => (
                  <div key={index} className="py-1">
                    {day}
                  </div>
                ))}
              </div>

              <div className="grid grid-cols-7 gap-1">
                {days.map(({ date, inMonth }, index) => {
                  const selected = isSameDay(date, value);
                  const isToday = isSameDay(date, today);

                  return (
                    <button
                      key={index}
                      type="button"
                      onClick={() => selectDate(date)}
                      className={[
                        "h-8 w-8 rounded text-sm transition",
                        selected
                          ? "bg-orange-500 text-white"
                          : isToday
                            ? "text-orange-500 hover:bg-gray-100"
                            : inMonth
                              ? "text-gray-700 hover:bg-gray-100"
                              : "text-gray-300 hover:bg-gray-50",
                      ].join(" ")}
                    >
                      {date.getDate()}
                    </button>
                  );
                })}
              </div>
            </>
          )}

          {view === "month" && (
            <div className="grid grid-cols-3 gap-2">
              {MONTHS.map((m, index) => (
                <button
                  key={m}
                  type="button"
                  onClick={() => {
                    setCursor(new Date(year, index, 1));
                    setView("day");
                  }}
                  className={[
                    "rounded px-2 py-3 text-sm transition",
                    index === month
                      ? "bg-orange-500 text-white"
                      : "text-gray-700 hover:bg-gray-100",
                  ].join(" ")}
                >
                  {m.slice(0, 3)}
                </button>
              ))}
            </div>
          )}

          {view === "year" && (
            <div className="grid grid-cols-3 gap-2">
              {Array.from(
                { length: 12 },
                (_, index) => yearRangeStart + index,
              ).map((y) => (
                <button
                  key={y}
                  type="button"
                  onClick={() => {
                    setCursor(new Date(y, month, 1));
                    setView("month");
                  }}
                  className={[
                    "rounded px-2 py-3 text-sm transition",
                    y === year
                      ? "bg-orange-500 text-white"
                      : "text-gray-700 hover:bg-gray-100",
                  ].join(" ")}
                >
                  {y}
                </button>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
