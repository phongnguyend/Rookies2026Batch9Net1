/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { motion, AnimatePresence } from "framer-motion";
import { useFirstChangePasswordMutation } from "../auth.api";
import { useAppDispatch } from "@/lib/redux/hooks";
import { completeFirstLogin } from "../auth.slice";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";

// password schema rules
const changePasswordSchema = z
  .object({
    newPassword: z
      .string()
      .min(1, "New password is required.")
      .min(
        6,
        "New password must be at least 6 characters long and less than 100 characters.",
      )
      .max(
        100,
        "New password must be at least 6 characters long and less than 100 characters.",
      )
      .regex(
        /^(?=.*[A-Za-z])(?=.*\d)(?=.*@)[A-Za-z\d@]+$/,
        "New password must contain at least one letter, one number, and one @ character.",
      )
      .transform((val) => val.trim()),

    confirmPassword: z.string().min(1, "Confirm password is required."),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Confirm password does not match.",
    path: ["confirmPassword"],
  });

type ChangePasswordForm = z.infer<typeof changePasswordSchema>;

interface FirstChangePasswordModalProps {
  isOpen: boolean;
}

export default function FirstChangePasswordModal({
  isOpen,
}: FirstChangePasswordModalProps) {
  const [firstChangePassword, { isLoading }] = useFirstChangePasswordMutation();
  const dispatch = useAppDispatch();

  const [showNew, setShowNew] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [serverErrors, setServerErrors] = useState<string[]>([]);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isValid, isSubmitted },
    reset,
  } = useForm<ChangePasswordForm>({
    resolver: zodResolver(changePasswordSchema),
    mode: "onChange",
    defaultValues: {
      newPassword: "",
      confirmPassword: "",
    },
  });

  // watch the change of value typedin text field
  const newPasswordValue = watch("newPassword");
  const confirmPasswordValue = watch("confirmPassword");

  // lock scroll when open
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }
    return () => {
      document.body.style.overflow = "";
    };
  }, [isOpen]);

  const onSubmit = async (data: ChangePasswordForm) => {
    setServerErrors([]);

    try {
      await firstChangePassword({
        newPassword: data.newPassword,
      }).unwrap();

      dispatch(
        enqueueToast({
          message: "Password changed successfully! Welcome to your dashboard.",
          type: ToastType.Success,
        }),
      );

      // complete first login flow
      dispatch(completeFirstLogin());
      reset();
    } catch (err: any) {
      console.error("Change password error:", err);
      const parsedErrors: string[] = [];

      // parse validation error list if present
      if (err?.errors) {
        Object.values(err.errors).forEach((messages: any) => {
          if (Array.isArray(messages)) {
            parsedErrors.push(...messages);
          } else if (typeof messages === "string") {
            parsedErrors.push(messages);
          }
        });
      }

      // fallback to single error description
      if (parsedErrors.length === 0) {
        const fallbackMsg =
          err?.detail || "An unexpected error occurred. Please try again.";
        parsedErrors.push(fallbackMsg);
      }

      setServerErrors(parsedErrors);
    }
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.2 }}
          className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40"
          role="dialog"
          aria-modal="true"
        >
          <motion.div
            initial={{ scale: 0.95, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.95, opacity: 0 }}
            transition={{ type: "spring", duration: 0.3, bounce: 0.1 }}
            className="w-full max-w-115 bg-white rounded-xl border border-gray-300 shadow-2xl overflow-hidden text-black"
          >
            {/* Header */}
            <div className="bg-[#f1f3f5] px-6 py-3 border-b border-gray-200">
              <h2 className="text-md font-bold text-[#cf2a2a] uppercase tracking-wide">
                Change password
              </h2>
            </div>

            {/* Body */}
            <div className="p-6 space-y-5">
              <div className="text-[13px] text-gray-700 leading-relaxed font-sans">
                <p>This is the first time you logged in.</p>
                <p>You have to change your password to continue.</p>
              </div>

              <form
                onSubmit={handleSubmit(onSubmit)}
                className="space-y-4.5"
                noValidate
              >
                {/* New Password Field */}
                <div className="space-y-1">
                  <div className="flex flex-row items-center gap-3">
                    <label className="text-[13px] text-gray-800 font-medium w-27.5 shrink-0 font-sans">
                      New password
                    </label>
                    <div className="relative flex-1">
                      <input
                        type={showNew ? "text" : "password"}
                        placeholder=""
                        className={`w-full px-3 py-1.5 pr-10 text-[13px] border rounded focus:outline-none focus:ring-1 focus:ring-[#cf2a2a] bg-white text-black transition-colors font-sans ${
                          errors.newPassword
                            ? "border-red-500"
                            : "border-gray-400"
                        }`}
                        disabled={isLoading}
                        {...register("newPassword")}
                        data-testid="txtChangePasswordFirstTime"
                      />
                      <button
                        type="button"
                        className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-600 hover:text-black transition-colors focus:outline-none"
                        onClick={() => setShowNew(!showNew)}
                        tabIndex={-1}
                      >
                        {showNew ? (
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 20 20"
                            fill="currentColor"
                            className="w-4 h-4"
                          >
                            <path
                              fillRule="evenodd"
                              d="M3.28 2.22a.75.75 0 00-1.06 1.06l14.5 14.5a.75.75 0 101.06-1.06L3.28 2.22zM8.14 5.2a7.5 7.5 0 017.58 3.5 7.21 7.21 0 01-.68 1.05l-1.12-1.12a5.75 5.75 0 00-5.78-4.23l-.68-.68c.22-.01.44-.02.68-.02zM12.92 10l-1.92-1.92a2.25 2.25 0 00-2.25 2.25l1.92 1.92a2.25 2.25 0 002.25-2.25z"
                              clipRule="evenodd"
                            />
                            <path d="M10 14a4.002 4.002 0 003.58-2.215l-1.282-1.282a2.5 2.5 0 01-4.596-1.03l-1.503-1.503A7.514 7.514 0 002.5 8.7a7.5 7.5 0 007.5 5.3z" />
                          </svg>
                        ) : (
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 20 20"
                            fill="currentColor"
                            className="w-4 h-4"
                          >
                            <path d="M10 12.5a2.5 2.5 0 100-5 2.5 2.5 0 000 5z" />
                            <path
                              fillRule="evenodd"
                              d="M.664 10.59a1.651 1.651 0 010-1.186A10.004 10.004 0 0110 3c4.257 0 7.893 2.66 9.336 6.41.08.21.08.44 0 .65A10.004 10.004 0 0110 17c-4.257 0-7.893-2.66-9.336-6.41zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                              clipRule="evenodd"
                            />
                          </svg>
                        )}
                      </button>
                    </div>
                  </div>
                  {errors.newPassword &&
                    (newPasswordValue !== "" || isSubmitted) && (
                      <p className="text-xs text-red-600 pl-30.5 leading-snug font-sans">
                        {errors.newPassword.message}
                      </p>
                    )}
                </div>

                {/* Confirm Password Field */}
                <div className="space-y-1">
                  <div className="flex flex-row items-center gap-3">
                    <label className="text-[13px] text-gray-800 font-medium w-27.5 shrink-0 font-sans">
                      Confirm Password
                    </label>
                    <div className="relative flex-1">
                      <input
                        type={showConfirm ? "text" : "password"}
                        placeholder=""
                        className={`w-full px-3 py-1.5 pr-10 text-[13px] border rounded focus:outline-none focus:ring-1 focus:ring-[#cf2a2a] bg-white text-black transition-colors font-sans ${
                          errors.confirmPassword
                            ? "border-red-500"
                            : "border-gray-400"
                        }`}
                        disabled={isLoading}
                        {...register("confirmPassword")}
                      />
                      <button
                        type="button"
                        className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-600 hover:text-black transition-colors focus:outline-none"
                        onClick={() => setShowConfirm(!showConfirm)}
                        tabIndex={-1}
                      >
                        {showConfirm ? (
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 20 20"
                            fill="currentColor"
                            className="w-4 h-4"
                          >
                            <path
                              fillRule="evenodd"
                              d="M3.28 2.22a.75.75 0 00-1.06 1.06l14.5 14.5a.75.75 0 101.06-1.06L3.28 2.22zM8.14 5.2a7.5 7.5 0 017.58 3.5 7.21 7.21 0 01-.68 1.05l-1.12-1.12a5.75 5.75 0 00-5.78-4.23l-.68-.68c.22-.01.44-.02.68-.02zM12.92 10l-1.92-1.92a2.25 2.25 0 00-2.25 2.25l1.92 1.92a2.25 2.25 0 002.25-2.25z"
                              clipRule="evenodd"
                            />
                            <path d="M10 14a4.002 4.002 0 003.58-2.215l-1.282-1.282a2.5 2.5 0 01-4.596-1.03l-1.503-1.503A7.514 7.514 0 002.5 8.7a7.5 7.5 0 007.5 5.3z" />
                          </svg>
                        ) : (
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 20 20"
                            fill="currentColor"
                            className="w-4 h-4"
                          >
                            <path d="M10 12.5a2.5 2.5 0 100-5 2.5 2.5 0 000 5z" />
                            <path
                              fillRule="evenodd"
                              d="M.664 10.59a1.651 1.651 0 010-1.186A10.004 10.004 0 0110 3c4.257 0 7.893 2.66 9.336 6.41.08.21.08.44 0 .65A10.004 10.004 0 0110 17c-4.257 0-7.893-2.66-9.336-6.41zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                              clipRule="evenodd"
                            />
                          </svg>
                        )}
                      </button>
                    </div>
                  </div>
                  {/* Show the errors message if the user has entered a value for the field or if the form has been submitted */}
                  {errors.confirmPassword &&
                    (confirmPasswordValue !== "" || isSubmitted) && (
                      <p className="text-xs text-red-600 pl-30.5 leading-snug font-sans">
                        {errors.confirmPassword.message}
                      </p>
                    )}
                </div>

                {/* Server-Side Error Display on botom */}
                {serverErrors.length > 0 && (
                  <div className="p-3 text-xs text-red-600 bg-red-50 border border-red-200 rounded font-sans space-y-1.5">
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

                {/* Save Button Row */}
                <div className="flex justify-end pt-1.5">
                  <button
                    type="submit"
                    className="px-5 py-1.5 bg-[#cf2a2a] hover:bg-[#b52222] text-white font-semibold text-[13px] rounded transition-all active:scale-[0.98] disabled:opacity-50 font-sans cursor-pointer"
                    disabled={(isSubmitted && !isValid) || isLoading}
                    data-testid="btnSave"
                  >
                    {isLoading ? (
                      <span className="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-1"></span>
                    ) : (
                      "Save"
                    )}
                  </button>
                </div>
              </form>
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}
