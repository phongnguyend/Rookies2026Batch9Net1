"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import Image from "next/image";

// Validation Schema
const loginFormSchema = z.object({
  username: z
    .string()
    .min(1, "Username is required.")
    .max(100, "Username must not exceed 100 characters.")
    .regex(/^[a-zA-Z0-9]+$/, "Username must contain only letters and digits.")
    .transform((val) => val.trim()),
  password: z.string().min(1, "Password is required."),
});

type LoginFormSchema = z.infer<typeof loginFormSchema>;

interface LoginFormProps {
  onLogin: (data: LoginFormSchema) => void;
  isLoading?: boolean;
  serverErrors?: string[];
}

export default function LoginForm({
  onLogin,
  isLoading = false,
  serverErrors = [],
}: LoginFormProps) {
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    watch,
    formState: { isValid, isSubmitted, errors, touchedFields },
  } = useForm<LoginFormSchema>({
    resolver: zodResolver(loginFormSchema),
    mode: "onChange",
    defaultValues: {
      username: "",
      password: "",
    },
  });

  // watch text fields for dynamic error visibility
  const usernameValue = watch("username");
  const passwordValue = watch("password");

  return (
    <div className="w-full max-w-md p-8 bg-base-100 rounded-2xl shadow-xl border border-base-200">
      <div className="text-center mb-8 flex flex-col items-center gap-4">
        <Image
          quality={100}
          src="/images/nashtech_logo.png"
          alt="NashTech Logo"
          width={120}
          height={0}
          style={{ width: "auto", height: "auto" }}
          loading="eager"
        />
        <h2 className="text-3xl font-bold tracking-tight text-primary">
          Online Asset Management
        </h2>
        <p className="text-sm text-base-content/60">
          Please enter your details to sign in
        </p>
      </div>

      <form onSubmit={handleSubmit(onLogin)} className="space-y-6" noValidate>
        {/* Username Field */}
        <div className="form-control w-full">
          <label className="label">
            <span className="label-text font-medium">Username</span>
          </label>
          <input
            type="text"
            placeholder="Enter your username"
            className={`input input-bordered w-full focus:input-primary transition-colors ${
              errors.username && (touchedFields.username || isSubmitted)
                ? "input-error focus:input-error"
                : ""
            }`}
            disabled={isLoading}
            {...register("username")}
            data-testid="txtUsername"
          />
          {errors.username && (touchedFields.username || isSubmitted) && (
            <label className="label pb-0">
              <span className="label-text-alt text-error font-medium">
                {errors.username.message}
              </span>
            </label>
          )}
        </div>

        {/* Password Field */}
        <div className="form-control w-full">
          <label className="label">
            <span className="label-text font-medium">Password</span>
          </label>
          <div className="relative">
            <input
              type={showPassword ? "text" : "password"}
              placeholder="Enter your password"
              className={`input input-bordered w-full pr-12 focus:input-primary transition-colors ${
                errors.password && (touchedFields.password || isSubmitted)
                  ? "input-error focus:input-error"
                  : ""
              }`}
              disabled={isLoading}
              {...register("password")}
              data-testid="txtPassword"
            />
            {/* Eye toggle button */}
            <button
              type="button"
              className="absolute right-4 top-1/2 -translate-y-1/2 text-base-content/40 hover:text-base-content/75 focus:outline-none transition-colors"
              onClick={() => setShowPassword(!showPassword)}
              tabIndex={-1}
            >
              {showPassword ? (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={1.5}
                  stroke="currentColor"
                  className="w-5 h-5"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88"
                  />
                </svg>
              ) : (
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={1.5}
                  stroke="currentColor"
                  className="w-5 h-5"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z"
                  />
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                  />
                </svg>
              )}
            </button>
          </div>
          {errors.password && (touchedFields.password || isSubmitted) && (
            <label className="label pb-0">
              <span className="label-text-alt text-error font-medium">
                {errors.password.message}
              </span>
            </label>
          )}
        </div>

        {/* Submit Button */}
        {/* enable initially, disable after a failed submission or form is not valid */}
        <button
          type="submit"
          className="w-full bg-primary hover:bg-[#b52222] text-white font-semibold py-2.5 rounded-lg transition-all shadow-md active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed hover:cursor-pointer"
          disabled={!isValid || isLoading}
          data-testid="btnLogin"
        >
          {isLoading ? (
            <span className="loading loading-spinner loading-sm"></span>
          ) : (
            "Sign In"
          )}
        </button>

        {/* Server-Side Error Display on bottom */}
        {serverErrors.length > 0 && (
          <div className="p-3 text-xs text-error bg-error/10 border border-error/20 rounded-lg font-sans space-y-1.5 w-full mt-4 whitespace-pre-line">
            {serverErrors.length === 1 ? (
              <p>{serverErrors[0]}</p>
            ) : (
              <ul className="list-disc pl-4 space-y-0.5">
                {serverErrors.map((msg, idx) => (
                  <li key={idx}>{msg}</li>
                ))}
              </ul>
            )}
          </div>
        )}
      </form>
    </div>
  );
}
