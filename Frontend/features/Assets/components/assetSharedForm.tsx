import { ReactNode } from "react";
import { normalizeText } from "./assetConstant";

export function CharacterCounter({ value, max }: { value: string; max: number }) {
  const len = normalizeText(value).length;
  const isMax = len === max;
  const isEmpty = len === 0;
  return (
    <div className="mt-1 flex justify-between text-xs">
      <span>
        {isMax && (
          <span className="text-red-500">Maximum characters is {max}.</span>
        )}
        {isEmpty && (
          <span className="text-red-500">This field is required.</span>
        )}
      </span>
      <span className={isMax || isEmpty ? "text-red-500" : "text-gray-500"}>
        {len}/{max}
      </span>
    </div>
  );
}

export function FieldError({ message }: { message?: string }) {
  if (!message) return null;
  return <p className="mt-1 text-sm text-red-500">{message}</p>;
}

export function FormField({
  label,
  children,
}: {
  label: string;
  children: ReactNode;
}) {
  return (
    <div
      className="
        flex
        flex-col
        gap-2
        sm:flex-row
        sm:items-start
        sm:gap-3
        w-full
      "
    >
      <label
        className="
          w-full
          text-sm
          font-medium
          text-gray-700
          sm:w-32
          md:w-36
          shrink-0
          sm:pt-2
        "
      >
        {label}
      </label>

      <div className="flex-1 min-w-0 w-full">
        {children}
      </div>
    </div>
  );
}
