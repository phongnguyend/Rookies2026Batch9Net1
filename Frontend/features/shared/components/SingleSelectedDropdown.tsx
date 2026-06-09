"use client";

import { ChevronDown } from "lucide-react";
import { useState } from "react";

interface SingleSelectDropdownProps<T> {
    items: T[];
    value?: string;
    placeholder?: string;
    width?: string;
    getKey: (item: T) => string;
    getLabel: (item: T) => string;
    onChange: (value?: string) => void;
    allLabel?: string;
    getTestId?: (item: T) => string;
    getTestIdAll?: string;
    showAll?: boolean;
}

export default function SingleSelectDropdown<T>({
    items,
    value,
    placeholder = "Select",
    width = "w-52",
    getKey,
    getLabel,
    onChange,
    allLabel = "All",
    getTestId,
    getTestIdAll,
    showAll = true,
}: SingleSelectDropdownProps<T>) {
    const [isOpen, setIsOpen] = useState(false);

    const selectedItem = items.find((item) => getKey(item) === value);

    const selectedLabel = selectedItem ? getLabel(selectedItem) : placeholder;

    const handleSelectItem = (itemKey: string) => {
        onChange(itemKey);
        setIsOpen(false);
    };

    const handleSelectAll = () => {
        onChange(undefined);
        setIsOpen(false);
    };

    return (
        <div className="relative">
            <button
                type="button"
                onClick={() => setIsOpen((prev) => !prev)}
                className={`hover:cursor-pointer flex h-9 items-center justify-between rounded border border-gray-400 px-3 ${width}`}
            >
                <span className="truncate">{selectedLabel}</span>
                <span><ChevronDown size={16} className={`shrink-0 transition-transform duration-200 ${isOpen ? "rotate-180" : ""}`} /></span>
            </button>

            {isOpen && (
                <div
                    className={`absolute top-9 z-20 rounded border border-gray-300 bg-white py-1 shadow ${width} max-h-60 overflow-y-auto`}
                >
                    {showAll && (
                        <label className="flex cursor-pointer items-center gap-2 px-2 py-1 text-sm hover:bg-gray-100">
                            <input
                                type="checkbox"
                                checked={!value}
                                onChange={handleSelectAll}
                                className="checkbox checkbox-xs"
                                data-testid={getTestIdAll}
                            />
                            <span>{allLabel}</span>
                        </label>
                    )}

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
                                    checked={value === itemKey}
                                    onChange={() => handleSelectItem(itemKey)}
                                    className="checkbox checkbox-xs"
                                    data-testid={getTestId?.(item)}
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