"use client";

interface SearchInputProps {
  value: string;
  onChange: (value: string) => void;
  onSearch?: (value: string) => void;
  placeholder?: string;
  width?: string;
  txtInputTestId?: string;
  btnSearchTestId?: string;
}

export default function SearchInput({
  value,
  onChange,
  onSearch,
  placeholder = "Search...",
  width = "w-full sm:w-60",
  txtInputTestId = "txtSearch",
  btnSearchTestId = "btnSearch",
}: SearchInputProps) {
  return (
    <div
      className={`flex h-9 w-full items-center rounded border border-gray-400 bg-white sm:max-w-xs ${width}`}
    >
      <input
        type="text"
        maxLength={100}
        value={value}
        placeholder={placeholder}
        onChange={(e) => onChange(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === "Enter") {
            onSearch?.(value);
          }
        }}
        className="h-full flex-1 px-3 text-sm outline-none"
        data-testid={txtInputTestId}
      />

      <button
        type="button"
        onClick={() => onSearch?.(value)}
        className="flex h-full w-10 items-center justify-center border-l border-gray-400 text-gray-600 hover:bg-gray-50"
        data-testid={btnSearchTestId}
      >
        🔍
      </button>
    </div>
  );
}
