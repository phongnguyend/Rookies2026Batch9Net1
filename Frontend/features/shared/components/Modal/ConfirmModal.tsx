"use client";

import { useEffect, useRef, type ReactNode } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { X } from "lucide-react";
import { FocusTrap } from "focus-trap-react";

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
}: ConfirmModalProps) {
  const modalRef = useRef<HTMLDivElement>(null);
  const showCloseIcon = !onYes || !!onNo;

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
        <FocusTrap
          focusTrapOptions={{
            escapeDeactivates: false, // we handle Escape ourselves above
            allowOutsideClick: true,  // lets the backdrop click still close it
          }}
        >
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
              className={`w-full ${sizeClasses[size]} bg-white rounded-lg border border-gray-500 shadow-xl overflow-hidden`}
            >
              {/* Modal Header */}
              <div className="bg-[#f1f3f5] border-b border-gray-500 px-6 py-3.5 flex items-center justify-between">
                <h2 className="text-lg font-bold text-primary">{title}</h2>
                {showCloseIcon && (
                  <button
                    type="button"
                    data-testid={closeBtnTestId}
                    onClick={onClose}
                    className="hover:cursor-pointer transition-colors duration-150 p-px rounded flex items-center justify-center border-primary bg-white hover:bg-primary hover:border-primary text-primary hover:text-white border-3"
                    aria-label="Close modal"
                  >
                    <div className="p-0.5 rounded-[3px] flex items-center justify-center font-bold">
                      <X size={10} strokeWidth={6} />
                    </div>
                  </button>
                )}
              </div>

              {/* Modal Body */}
              <div
                className={`px-6 pt-6 text-neutral-800 text-base leading-relaxed ${yesButtonLabel ? "pb-4" : "pb-6"}`}
              >
                {typeof body === "string" ? <p>{body}</p> : body}
              </div>

              {/* Modal Actions */}
              {yesButtonLabel && (
                <div className="px-6 pb-6 pt-0 bg-white flex items-center justify-start gap-3">
                  <button
                    type="button"
                    data-testid={confirmBtnTestId}
                    onClick={onYes}
                    disabled={isLoading}
                    className="hover:cursor-pointer px-4 py-2 bg-primary hover:bg-red-600 active:bg-primary/95 text-white font-semibold rounded flex items-center gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
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
                    className="px-4 py-2 border border-gray-400 rounded text-[#6c757d] font-normal hover:bg-gray-100 transition-colors duration-150 disabled:opacity-50 disabled:cursor-not-allowed hover:cursor-pointer"
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
        </FocusTrap>
      )}
    </AnimatePresence>
  );
}
