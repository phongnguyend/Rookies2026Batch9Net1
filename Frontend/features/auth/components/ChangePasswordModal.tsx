/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { useEffect, useRef, useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useChangePasswordMutation } from "../auth.api";
import { useAppDispatch } from "@/lib/redux/hooks";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";

const changePasswordSchema = z
  .object({
    oldPassword: z.string().min(1, "Current password is required"),
    newPassword: z.string().min(6, "New password must be at least 6 characters"),
    confirmPassword: z.string().min(1, "Confirm password is required"),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Confirm password does not match",
    path: ["confirmPassword"],
  });

type ChangePasswordForm = z.infer<typeof changePasswordSchema>;

interface ChangePasswordModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function ChangePasswordModal({
  isOpen,
  onClose,
}: ChangePasswordModalProps) {
  const modalRef = useRef<HTMLDivElement>(null);
  const dispatch = useAppDispatch();
  const [changePassword, { isLoading }] = useChangePasswordMutation();

  const [showOldPassword, setShowOldPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ChangePasswordForm>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      oldPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
  });

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape" && isOpen) onClose();
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  useEffect(() => {
    document.body.style.overflow = isOpen ? "hidden" : "";

    if (!isOpen) {
      reset();
      setErrorMessage(null);
    }

    return () => {
      document.body.style.overflow = "";
    };
  }, [isOpen, reset]);

  const handleBackdropClick = (e: React.MouseEvent) => {
    if (modalRef.current && !modalRef.current.contains(e.target as Node)) {
      onClose();
    }
  };

  const onSubmit = async (data: ChangePasswordForm) => {
    setErrorMessage(null);

    try {
      await changePassword({
        oldPassword: data.oldPassword,
        newPassword: data.newPassword,
      }).unwrap();

      dispatch(
        enqueueToast({
          message: "Password changed successfully.",
          type: ToastType.Success,
        }),
      );

      reset();
      onClose();
    } catch (err: any) {
        console.log(err);
      setErrorMessage(
        err?.detail ||
          err?.title ||"Failed to change password.",
      );
    }
  };

  const inputClass = (hasError?: boolean) =>
    `w-full border rounded px-3 py-2 text-sm outline-none bg-white ${
      hasError
        ? "border-red-500 focus:border-red-500"
        : "border-gray-300 focus:border-primary"
    }`;

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.2 }}
          onClick={handleBackdropClick}
          className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-xs"
          role="dialog"
          aria-modal="true"
        >
          <motion.div
            ref={modalRef}
            initial={{ scale: 0.95, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.95, opacity: 0 }}
            transition={{ type: "spring", duration: 0.3, bounce: 0.1 }}
            className="w-full max-w-lg bg-white rounded-lg border border-gray-300 shadow-xl overflow-hidden"
          >
            <div className="bg-primary text-white px-6 py-4 flex items-center justify-between">
              <h2 className="text-lg font-bold tracking-wide">
                Change Password
              </h2>

              <button
                type="button"
                onClick={onClose}
                className="text-white/80 hover:text-white transition-colors duration-150 p-1 rounded-full hover:bg-white/10"
                aria-label="Close modal"
              >
                ✕
              </button>
            </div>

            <form onSubmit={handleSubmit(onSubmit)}>
              <div className="p-6 text-neutral-800 text-base leading-relaxed border-b border-gray-200 bg-[#fafafa] space-y-4">
                {/* Current Password */}
                <div>
                  <label className="block mb-1 font-semibold">
                    Current password
                  </label>
                  <div className="relative">
                    <input
                      type={showOldPassword ? "text" : "password"}
                      className={`${inputClass(!!errors.oldPassword)} pr-12`}
                      disabled={isLoading}
                      {...register("oldPassword")}
                    />

                    <button
                      type="button"
                      className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
                      onClick={() => setShowOldPassword(!showOldPassword)}
                      tabIndex={-1}
                    >
                      {showOldPassword ? (
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

                {/* New Password */}
                <div>
                  <label className="block mb-1 font-semibold">
                    New password
                  </label>
                  <div className="relative">
                    <input
                      type={showNewPassword ? "text" : "password"}
                      className={inputClass(!!errors.newPassword)}
                      disabled={isLoading}
                      {...register("newPassword")}
                    />
                    <button
                        type="button"
                        className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
                        onClick={() => setShowNewPassword(!showNewPassword)}
                        tabIndex={-1}
                      >
                        {showNewPassword ? (
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
                  {errors.newPassword && (
                    <p className="mt-1 text-sm text-red-600">
                      {errors.newPassword.message}
                    </p>
                  )}
                </div>

                {/* Confirm Password */}
                <div>
                  <label className="block mb-1 font-semibold">
                    Confirm password
                  </label>
                  <div className="relative">
                    <input
                      type={showConfirmPassword ? "text" : "password"}
                      className={inputClass(!!errors.confirmPassword)}
                      disabled={isLoading}
                      {...register("confirmPassword")}
                    />
                    <button
                        type="button"
                        className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                        tabIndex={-1}
                      >
                        {showConfirmPassword ? (
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
                  {errors.confirmPassword && (
                    <p className="mt-1 text-sm text-red-600">
                      {errors.confirmPassword.message}
                    </p>
                  )}
                </div>

                {errorMessage && (
                  <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded p-3">
                    {errorMessage}
                  </div>
                )}
              </div>

              <div className="px-6 py-4 bg-white flex items-center justify-end gap-3">
                <button
                  type="submit"
                  disabled={isLoading}
                  className="px-4 py-2 bg-primary hover:bg-primary/90 active:bg-primary/95 text-white font-semibold rounded flex items-center gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {isLoading && (
                    <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                  )}
                  Save
                </button>

                <button
                  type="button"
                  onClick={onClose}
                  disabled={isLoading}
                  className="px-4 py-2 border border-gray-400 rounded text-neutral-700 font-semibold hover:bg-gray-100 transition-colors duration-150 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  Cancel
                </button>
              </div>
            </form>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}