/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { useLoginMutation, useLazyGetMeQuery } from "@/features/auth/auth.api";
import LoginForm from "@/features/auth/components/LoginForm";
import { useAppDispatch } from "@/lib/redux/hooks";
import { loginSuccess, loginFailure } from "@/features/auth/auth.slice";
import { UserRoles } from "@/features/users/users.types";
import { useState } from "react";
import { Login } from "@/features/auth/auth.types";

export default function HomePage() {
  // Queries and hooks
  const [login, { isLoading: isLoggingIn }] = useLoginMutation();
  const [getMe] = useLazyGetMeQuery();
  const dispatch = useAppDispatch();

  // States
  const [serverErrors, setServerErrors] = useState<string[]>([]);

  const handleLoginSubmit = async (data: Login.Request) => {
    setServerErrors([]);
    try {
      await login({
        username: data.username,
        password: data.password,
      }).unwrap();

      const profile = await getMe().unwrap();
      const userRole = profile.roles.includes(UserRoles.Admin)
        ? UserRoles.Admin
        : UserRoles.Staff;

      dispatch(
        loginSuccess({
          username: profile.userName,
          role: userRole,
          isFirstLogin: profile.isFirstLogin,
          locationName: profile.locationName,
        }),
      );
    } catch (err: any) {
      // console.error("Login failed error object:", err);

      const errorData = err?.data || err;
      let errorMessage = "";
      if (errorData?.errors) {
        const errorList: string[] = [];
        Object.values(errorData.errors).forEach((messages: any) => {
          if (Array.isArray(messages)) {
            errorList.push(...messages);
          } else if (typeof messages === "string") {
            errorList.push(messages);
          }
        });
        if (errorList.length > 0) {
          errorMessage = errorList.join("\n");
        }
      }

      if (!errorMessage) {
        errorMessage =
          errorData?.detail || "Something went wrong. Please try again later.";
      }

      const parsedErrors = [errorMessage];
      setServerErrors(parsedErrors);
      dispatch(loginFailure(parsedErrors[0]));
    }
  };

  return (
    <main className="relative min-h-screen flex items-center justify-center bg-base-200 px-4">
      <div className="absolute inset-0 overflow-hidden pointer-events-none z-0">
        <div className="absolute top-[-20%] left-[-10%] w-125 h-125 rounded-full bg-primary/5 blur-[120px]"></div>
        <div className="absolute bottom-[-20%] right-[-10%] w-125 h-125 rounded-full bg-primary/5 blur-[120px]"></div>
      </div>

      <div className="relative z-10 w-full flex justify-center">
        <LoginForm
          onLogin={handleLoginSubmit}
          isLoading={isLoggingIn}
          serverErrors={serverErrors}
        />
      </div>
    </main>
  );
}
