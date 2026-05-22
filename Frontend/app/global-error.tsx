"use client";

import { useEffect } from "react";
import Link from "next/link";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { UserRoles } from "@/features/users/users.types";
import { APP_ROUTES } from "@/lib/api/routes";
import { useAppSelector } from "@/lib/redux/hooks";
import Link from "next/link";
import StoreProvider from "./StoreProvider";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

type GlobalErrorProps = {
  error: Error & { digest?: string };
  reset: () => void;
};

export default function GlobalError({ error, reset }: GlobalErrorProps) {
  return (
    <StoreProvider>
      <GlobalErrorContent error={error} reset={reset} />
    </StoreProvider>
  );
}

function GlobalErrorContent({ error, reset }: GlobalErrorProps) {
  useEffect(() => {
    console.error(error);
  }, [error]);

  const user = useAppSelector((state) => state.authSlice.user);

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
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col items-center justify-center bg-[#f8f9fa] px-6 py-12 select-none">
        <div className="max-w-md w-full bg-white border border-gray-200 rounded-2xl shadow-lg p-8 text-center relative overflow-hidden">
          {/* top error accent line */}
          <div className="absolute top-0 left-0 right-0 h-1 bg-primary" />

          {/* 500 error graphic */}
          <div className="flex justify-center mb-6">
            <div className="bg-[#fff1f2] text-primary p-4 rounded-full">
              <svg
                className="w-16 h-16"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                <line x1="12" y1="9" x2="12" y2="13" />
                <line x1="12" y1="17" x2="12.01" y2="17" />
              </svg>
            </div>
          </div>

          {/* big code */}
          <h1 className="text-6xl font-black text-primary tracking-tight">
            500
          </h1>

          {/* title */}
          <h2 className="text-xl font-bold text-neutral-800 mt-4">
            Unexpected Error Occurred
          </h2>

          {/* description */}
          <p className="text-gray-500 mt-2 text-sm leading-relaxed">
            Something went wrong in the system. Our team of highly trained
            administrators has been notified.
          </p>

          {/* buttons */}
          <div className="flex flex-col gap-3 mt-8">
            <button
              onClick={() => reset()}
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
                  d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0l3.181 3.183a8.25 8.25 0 0013.803-3.7M4.031 9.865a8.25 8.25 0 0113.803-3.7l3.181 3.182m0-4.991v4.99"
                />
              </svg>
              Try Again
            </button>

            {/* return to home path */}
            <Link
              href={homePath}
              className="w-full py-3 bg-neutral-100 hover:bg-neutral-200 text-neutral-800 font-bold rounded-lg shadow-sm transition-all duration-150 flex items-center justify-center gap-2 text-sm"
            >
              Go Back Home
            </Link>
          </div>
        </div>
      </body>
    </html>
  );
}
