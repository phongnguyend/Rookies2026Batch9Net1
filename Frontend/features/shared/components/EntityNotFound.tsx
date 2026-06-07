"use client";

import { useRouter } from "next/navigation";

interface NotFoundProps {
  pageTitle?: string;
  entityName?: string;
  action?: string;
  description?: string;
  redirectPath?: string;
  redirectText?: string;
}

export default function EntityNotFound({
  pageTitle = "Home",
  entityName = "Item",
  action = "access",
  description,
  redirectPath = "/",
  redirectText = "Go Back",
}: NotFoundProps) {
  const router = useRouter();

  const defaultDescription = `The ${entityName.toLowerCase()} you are trying to ${action} does not exist or is no longer available.`;

  return (
    <div className="flex w-full min-h-[70vh]">
      <div className="w-full">
        <h2 className="mb-8 text-xl font-bold text-red-600">{pageTitle}</h2>
        <div className="relative mx-auto w-full max-w-md overflow-hidden rounded-2xl border border-gray-200 bg-white p-8 shadow-lg">
          <div className="absolute left-0 right-0 top-0 h-1 bg-primary" />

          <div className="mb-6 flex justify-center">
            <div className="rounded-full bg-[#eff1f5] p-4 text-primary">
              <svg
                className="h-16 w-16"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
              </svg>
            </div>
          </div>

          <h1 className="text-primary text-6xl font-black tracking-tight text-center">
            404
          </h1>

          <h2 className="mt-4 text-xl font-bold text-neutral-800 text-center">
            {entityName} Not Found
          </h2>

          <p className="mt-2 text-sm text-gray-500 text-center">
            {description ?? defaultDescription}
          </p>

          <div className="mt-8">
            <button
              onClick={() => router.push(redirectPath)}
              className="flex w-full items-center justify-center gap-2 rounded-lg bg-primary py-3 text-sm font-bold text-white shadow-sm transition-all duration-150 hover:cursor-pointer hover:bg-primary/90"
            >
              {redirectText}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
