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
          <span className="text-orange-500">Maximum characters is {max}.</span>
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
    <div className="flex flex-col gap-2 md:flex-row md:items-start md:gap-4">
      <label className="w-full pt-0 text-sm font-medium text-gray-700 md:w-36 md:shrink-0 md:pt-2">
        {label}
      </label>
      <div className="flex-1">{children}</div>
    </div>
  );
}
