"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { UserRoles } from "@/features/users/users.types";
import { APP_ROUTES } from "@/lib/api/routes";
import { useAppSelector } from "@/lib/redux/hooks";

export default function NotFoundPage() {
  const router = useRouter();
  const user = useAppSelector(state => state.authSlice.user);

  let homePath = APP_ROUTES.HOME;
  switch (user?.role as UserRoles) {
    case UserRoles.Admin: {
      homePath = APP_ROUTES.ADMIN_HOME;
      break;
    }
    case UserRoles.Staff: {
      homePath = APP_ROUTES.STAFF_HOME;
      break;
    }
    default: {
      homePath = APP_ROUTES.HOME;
    }
  }

  return (
    <div className="flex flex-col items-center justify-center min-h-[70vh] w-full px-6 py-12 text-center select-none">
      <div className="max-w-md w-full bg-white border border-gray-200 rounded-2xl shadow-lg p-8 relative overflow-hidden">
        {/* Top Accent Line */}
        <div className="absolute top-0 left-0 right-0 h-1 bg-primary" />

        {/* 404 Graphic */}
        <div className="flex justify-center mb-6">
          <div className="bg-[#eff1f5] text-primary p-4 rounded-full">
            <svg
              className="w-16 h-16"
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

        {/* Big Code */}
        <h1 className="text-6xl font-black text-primary tracking-tight">404</h1>

        {/* Title */}
        <h2 className="text-xl font-bold text-neutral-800 mt-4">
          Page Not Found
        </h2>

        {/* Buttons */}
        <div className="flex flex-col gap-3 mt-8">
          <Link
            href={homePath}
            className="w-full py-3 bg-primary hover:bg-primary/90 text-white font-bold rounded-lg shadow-sm transition-all duration-150 flex items-center justify-center gap-2 text-sm"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={2.5}
              stroke="currentColor"
              className="w-4 h-4"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M2.25 12l8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25"
              />
            </svg>
            Return to Home
          </Link>

          <button
            onClick={() => router.back()}
            className="w-full py-3 border border-gray-300 hover:bg-gray-50 text-neutral-700 font-bold rounded-lg transition-all duration-150 flex items-center justify-center gap-2 text-sm"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={2.5}
              stroke="currentColor"
              className="w-4 h-4"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18"
              />
            </svg>
            Go Back
          </button>
        </div>
      </div>
    </div>
  );
}
