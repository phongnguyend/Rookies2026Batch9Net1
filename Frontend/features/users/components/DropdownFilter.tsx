"use client";

import { useState } from "react";

interface DropdownFilterProps<T> {
  items: T[];
  values: string[];
  placeholder?: string;
  width?: string;
  getKey: (item: T) => string;
  getLabel: (item: T) => string;
  onChange: (values: string[]) => void;
  allLabel?: string;
  getTestId?: (item: T) => string;
  getTestIdAll?: () => string;
}

export default function DropdownFilter<T>({
  items,
  values,
  placeholder = "Select",
  width = "w-52",
  getKey,
  getLabel,
  onChange,
  allLabel = "All",
  getTestId,
  getTestIdAll,
}: DropdownFilterProps<T>) {
  const [isOpen, setIsOpen] = useState(false);
  const selectedValue = values[0] ?? "";

  const selectedLabel = selectedValue
    ? getLabel(items.find((item) => getKey(item) === selectedValue)!)
    : placeholder;

  const handleSelectItem = (itemKey: string) => {
    onChange([itemKey]);
    setIsOpen(false);
  };

  const handleSelectAll = () => {
    onChange([]);
    setIsOpen(false);
  };

  return (
    <div className="relative">
      <button
        type="button"
        onClick={() => setIsOpen((prev) => !prev)}
        className={`flex h-9 items-center justify-between rounded border border-gray-400 px-3 ${width}`}
      >
        <span className="truncate">{selectedLabel}</span>
        <span>{"\u25bc"}</span>
      </button>

      {isOpen && (
        <div
          className={`absolute top-9 z-20 rounded border border-gray-300 bg-white py-1 shadow ${width}`}
        >
          <label
            onClick={handleSelectAll}
            className="flex cursor-pointer items-center gap-2 px-2 py-1 text-sm hover:bg-gray-100"
          >
            <input
              type="radio"
              checked={!selectedValue}
              readOnly
              className="radio radio-xs"
              {...(getTestIdAll ? { "data-testid": getTestIdAll() } : {})}
            />
            <span>{allLabel}</span>
          </label>

          {items.map((item) => {
            const itemKey = getKey(item);
            const itemLabel = getLabel(item);

            return (
              <label
                key={itemKey}
                onClick={() => handleSelectItem(itemKey)}
                className="flex cursor-pointer items-center gap-2 px-2 py-1 text-sm hover:bg-gray-100"
              >
                <input
                  type="radio"
                  checked={selectedValue === itemKey}
                  readOnly
                  className="radio radio-xs"
                  {...(getTestId ? { "data-testid": getTestId(item) } : {})}
                />
                <span>{itemLabel}</span>
              </label>
            );
          })}
        </div>
      )}
    </div>
  );
}
