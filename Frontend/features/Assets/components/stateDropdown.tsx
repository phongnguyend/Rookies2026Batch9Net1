"use client";

import { ChevronDown } from "lucide-react";
import { useState } from "react";

interface DropdownStateFilterProps<T> {
  items: T[]; // This one to show chosen Item on UI
  values: string[];
  placeholder?: string;
  width?: string;
  getKey: (item: T) => string;
  getLabel: (item: T) => string;
  onChange: (values: string[]) => void;
  allLabel?: string;
  customLabel?: string;
  defaultValue?: string[];
}

export default function DropdownStateFilter<T>({
  items,
  values,
  placeholder = "Select",
  width = "w-52",
  getKey,
  getLabel,
  onChange,
  allLabel = "All",
  customLabel,
}: DropdownStateFilterProps<T>) {
  const [isOpen, setIsOpen] = useState(false);

  const isAllSelected =
    values.length === items.length &&
    items.every((item) => values.includes(getKey(item)));

  const selectedLabel =
    customLabel ??
    (values.length === 0
      ? placeholder
      : values.length === 1
        ? getLabel(items.find((item) => getKey(item) === values[0])!)
        : `${values.length} selected`);

  const handleToggleItem = (itemKey: string) => {
    if (values.includes(itemKey)) {
      const filteredValues = values.filter(
        (value) => value !== itemKey
      );

      if (filteredValues.length === 0) {
        onChange(items.map((item) => getKey(item)));
      } else {
        onChange(filteredValues);
      }
    } else {
      onChange([...values, itemKey]);
    }
  };

  const handleToggleAll = () => {
    if (isAllSelected) {
      return;
    } else {
      onChange(items.map((item) => getKey(item)));
    }
  };

  return (
    <div className="relative">
      <button
        data-testid="ddlState"
        type="button"
        onClick={() => setIsOpen((prev) => !prev)}
        className={`hover:cursor-pointer flex h-9 items-center justify-between rounded border border-gray-400 px-3 ${width}`}
      >
        <span className="truncate">{selectedLabel}</span>
        <span><ChevronDown size={16} className={`shrink-0 transition-transform duration-200 ${isOpen ? "rotate-180" : ""}`} /></span>
      </button>

      {isOpen && (
        <div
          className={`absolute top-9 z-20 rounded border border-gray-300 bg-white py-1 shadow ${width}`}
        >
          <label className="flex cursor-pointer items-center gap-2 px-2 py-1 text-sm hover:bg-gray-100">
            <input
              type="checkbox"
              checked={isAllSelected}
              onChange={handleToggleAll}
              className="checkbox checkbox-xs"
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
                  checked={values.includes(itemKey)}
                  onChange={() => handleToggleItem(itemKey)}
                  className="checkbox checkbox-xs"
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
