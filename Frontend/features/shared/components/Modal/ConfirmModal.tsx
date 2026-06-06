"use client";

import { useEffect, useRef, type ReactNode } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { X } from "lucide-react";

export interface ConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onYes?: () => void;
  onNo?: () => void;
  title: string;
  body: ReactNode;
  yesButtonLabel?: string;
  noButtonLabel?: string;
  isLoading?: boolean;
  size?: "sm" | "md" | "lg";
  modalTestId?: string;
  confirmBtnTestId?: string;
  cancelBtnTestId?: string;
  closeBtnTestId?: string;
  isError?: boolean;
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
  modalTestId = "dlgConfirmPopup",
  confirmBtnTestId = "btnConfirm",
  cancelBtnTestId = "btnCancel",
  closeBtnTestId = "btnClose",
  isError = false,
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
            data-testid={modalTestId}
            ref={modalRef}
            initial={{ scale: 0.95, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.95, opacity: 0 }}
            transition={{ type: "spring", duration: 0.3, bounce: 0.1 }}
            className={`w-full ${sizeClasses[size]} bg-white rounded-lg border border-gray-400 shadow-xl overflow-hidden`}
          >
            {/* Modal Header */}
            <div className="bg-[#f1f3f5] border-b border-gray-300 px-6 py-3.5 flex items-center justify-between">
              <h2
                className={`text-lg font-bold ${isError ? "text-[#cf2323]" : "text-gray-800"}`}
              >
                {title}
              </h2>
              <button
                data-testid={closeBtnTestId}
                onClick={onClose}
                className={`transition-colors duration-150 p-0.25 border rounded flex items-center justify-center ${
                  isError
                    ? "border-primary bg-white hover:bg-[#f1f3f5] border-2"
                    : "border-gray-400 bg-white text-gray-600 hover:bg-gray-50"
                }`}
                aria-label="Close modal"
              >
                {isError ? (
                  <div className="text-primary p-[2px] rounded-[3px] flex items-center justify-center font-bold">
                    <X size={10} strokeWidth={6} />
                  </div>
                ) : (
                  <X size={14} strokeWidth={1} />
                )}
              </button>
            </div>

            {/* Modal Body */}
            <div
              className={`p-6 text-neutral-800 text-base leading-relaxed ${yesButtonLabel ? "border-b border-gray-200" : ""}`}
            >
              {typeof body === "string" ? <p>{body}</p> : body}
            </div>

            {/* Modal Actions */}
            {yesButtonLabel && (
              <div className="px-6 py-4 bg-white flex items-center justify-end gap-3">
                <button
                  data-testid={confirmBtnTestId}
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
                  data-testid={cancelBtnTestId}
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
            )}
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}
