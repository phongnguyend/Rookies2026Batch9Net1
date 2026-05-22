"use client";

import { useEffect, useRef, type ReactNode } from "react";
import { motion, AnimatePresence } from "framer-motion";

export interface ConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onYes: () => void;
  onNo?: () => void;
  title: string;
  body: ReactNode;
  yesButtonLabel: string;
  noButtonLabel?: string;
  isLoading?: boolean;
  size?: "sm" | "md" | "lg";
}

export default function ConfirmModal({
  isOpen,
  onClose,
  onYes,
  onNo,
  title,
  body,
  yesButtonLabel,
  isLoading = false,
  noButtonLabel = "Cancel",
  size = "md",
}: ConfirmModalProps) {
  const modalRef = useRef<HTMLDivElement>(null);

  // Close modal on ESC key press
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape" && isOpen) {
        onClose();
      }
    };
    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  // Prevent scroll on body when modal is open
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

  // Handle backdrop click
  const handleBackdropClick = (e: React.MouseEvent) => {
    if (modalRef.current && !modalRef.current.contains(e.target as Node)) {
      onClose();
    }
  };

  const sizeClasses = {
    sm: "max-w-md",
    md: "max-w-lg",
    lg: "max-w-2xl",
  };

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
            className={`w-full ${sizeClasses[size]} bg-white rounded-lg border border-gray-300 shadow-xl overflow-hidden`}
          >
            {/* Modal Header */}
            <div className="bg-primary text-white px-6 py-4 flex items-center justify-between">
              <h2 className="text-lg font-bold tracking-wide">{title}</h2>
              <button
                onClick={onClose}
                className="text-white/80 hover:text-white transition-colors duration-150 p-1 rounded-full hover:bg-white/10"
                aria-label="Close modal"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={2.5}
                  stroke="currentColor"
                  className="w-5 h-5"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            </div>

            {/* Modal Body */}
            <div className="p-6 text-neutral-800 text-base leading-relaxed border-b border-gray-200 bg-[#fafafa]">
              {typeof body === "string" ? <p>{body}</p> : body}
            </div>

            {/* Modal Actions */}
            <div className="px-6 py-4 bg-white flex items-center justify-end gap-3">
              <button
                onClick={onYes}
                disabled={isLoading}
                className="px-4 py-2 bg-primary hover:bg-primary/90 active:bg-primary/95 text-white font-semibold rounded flex items-center gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isLoading && (
                  <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                )}
                {yesButtonLabel}
              </button>

              <button
                onClick={() => {
                  if (onNo) {
                    onNo();
                    return;
                  }
                  onClose();
                }}
                disabled={isLoading}
                className={
                  onNo
                    ? "px-4 py-2 bg-primary hover:bg-primary/90 active:bg-primary/95 text-white font-semibold rounded flex items-center gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
                    : "px-4 py-2 border border-gray-400 rounded text-neutral-700 font-semibold hover:bg-gray-100 transition-colors duration-150 disabled:opacity-50 disabled:cursor-not-allowed"
                }
              >
                {onNo && isLoading && (
                  <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                )}

                {noButtonLabel}
              </button>
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}
