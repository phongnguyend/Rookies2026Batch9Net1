"use client";

interface RadioGroupProps<T> {
  label?: string;
  items: T[];
  value: string;
  getKey: (item: T) => string;
  getLabel: (item: T) => string;
  onChange: (value: string) => void;
  name: string;
}

export default function RadioGroup<T>({
  label,
  items,
  value,
  getKey,
  getLabel,
  onChange,
  name,
}: RadioGroupProps<T>) {
  return (
    <div className="flex gap-6">
      {label && <label className="w-28 text-sm text-gray-700">{label}</label>}

      <div className="flex gap-8">
        {items.map((item) => {
          const itemKey = getKey(item);

          return (
            <label
              key={itemKey}
              className="label cursor-pointer gap-2"
            >
              <input
                type="radio"
                name={name}
                value={itemKey}
                checked={value === itemKey}
                onChange={() => onChange(itemKey)}
                className="radio radio-primary radio-sm"
              />

              <span className="label-text">{getLabel(item)}</span>
            </label>
          );
        })}
      </div>
    </div>
  );
}
