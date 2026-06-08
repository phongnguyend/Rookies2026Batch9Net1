"use client";

import { ChevronDown } from "lucide-react";
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

  // Ignore unknown values from the URL or parent state so the dropdown only
  // renders selections that exist in the provided item list.
  const selectedKeys = values.filter((value) =>
    items.some((item) => getKey(item) === value),
  );

  // In this filter, an empty selection means "All" is selected.
  const isAllSelected = selectedKeys.length === 0;

  const selectedLabel = isAllSelected
    ? placeholder
    : selectedKeys.length === 1
      ? getLabel(items.find((item) => getKey(item) === selectedKeys[0])!)
      : `${selectedKeys.length} selected`;

  const handleToggleItem = (itemKey: string) => {
    const nextValues = selectedKeys.includes(itemKey)
      ? selectedKeys.filter((value) => value !== itemKey)
      : [...selectedKeys, itemKey];

    // If every specific option is selected, normalize back to "All" by
    // clearing the selected keys. This keeps the UI from showing redundant
    // Staff + Admin selections when they mean the same thing as All.
    onChange(nextValues.length === items.length ? [] : nextValues);
  };

  const handleSelectAll = () => {
    // Parent components read an empty array as the "All" filter.
    onChange([]);
  };

  return (
    <div className="relative">
      <button
        type="button"
        onClick={() => setIsOpen((prev) => !prev)}
        className={`hover:cursor-pointer flex h-9 items-center justify-between rounded border border-gray-400 px-3 ${width}`}
      >
        <span className="truncate">{selectedLabel}</span>
        <span><ChevronDown size={16} className={`shrink-0 transition-transform duration-200 ${isOpen ? "rotate-180" : ""}`}/></span>
      </button>

      {isOpen && (
        <div
          className={`absolute top-9 z-20 rounded border border-gray-300 bg-white py-1 shadow ${width}`}
        >
          <label className="flex cursor-pointer items-center gap-2 px-2 py-1 text-sm hover:bg-gray-100">
            <input
              type="checkbox"
              checked={isAllSelected}
              onChange={handleSelectAll}
              className="checkbox checkbox-xs"
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
                className="flex cursor-pointer items-center gap-2 px-2 py-1 text-sm hover:bg-gray-100"
              >
                <input
                  type="checkbox"
                  checked={selectedKeys.includes(itemKey)}
                  onChange={() => handleToggleItem(itemKey)}
                  className="checkbox checkbox-xs"
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
