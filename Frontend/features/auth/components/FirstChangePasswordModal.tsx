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
    formState: { errors, isValid, isSubmitted, touchedFields },
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
          errorData?.detail ||
          "An unexpected error occurred. Please try again.";
      }

      const parsedErrors = [errorMessage];
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
                  </div>
                  {errors.newPassword &&
                    (touchedFields.newPassword || isSubmitted) && (
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
                  </div>
                  {/* Show the errors message if the user has entered a value for the field or if the form has been submitted */}
                  {errors.confirmPassword &&
                    (touchedFields.confirmPassword || isSubmitted) && (
                      <p className="text-xs text-red-600 pl-30.5 leading-snug font-sans">
                        {errors.confirmPassword.message}
                      </p>
                    )}
                </div>

                {/* Server-Side Error Display on botom */}
                {serverErrors.length > 0 && (
                  <div className="p-3 text-xs text-red-600 bg-red-50 border border-red-200 rounded font-sans space-y-1.5 whitespace-pre-line">
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
                    className="px-5 py-1.5 bg-[#cf2a2a] hover:bg-[#b52222] text-white font-semibold text-[13px] rounded transition-all active:scale-[0.98] disabled:opacity-50 font-sans cursor-pointer disabled:cursor-not-allowed"
                    disabled={!isValid || isLoading}
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
