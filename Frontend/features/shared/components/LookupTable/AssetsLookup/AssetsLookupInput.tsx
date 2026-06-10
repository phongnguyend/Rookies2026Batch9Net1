"use client";

import { LookupAssetsSummary } from "@/features/Assets/assets.types";
import { Search } from "lucide-react";
import { useEffect, useState } from "react";
import { AssetsLookupTable } from "./AssetsLookupTable";
import { FocusTrap } from "focus-trap-react";

export interface AssetsLookupInputProps {
  value?: LookupAssetsSummary | null;
  onChange?: (asset: LookupAssetsSummary | null) => void;
  data_testid?: string;
  placeholder?: string;
}

const AssetsLookupInput = ({
  value,
  onChange,
  data_testid = "txtAssetLookupInput",
  placeholder = "Select an asset",
}: AssetsLookupInputProps) => {
  const [isOpen, setIsOpen] = useState(false);
  // pendingAsset: in-dialog selection only, never drives the input display
  const [pendingAsset, setPendingAsset] = useState<LookupAssetsSummary | null>(
    null,
  );

  const handleOpen = () => {
    // pre-check the currently committed user
    setPendingAsset(value ?? null);
    setIsOpen(true);
  };

  const handleSave = (asset: LookupAssetsSummary | null) => {
    // commit: parent updates value prop → input reflects new name
    onChange?.(asset);
    setIsOpen(false);
    setPendingAsset(null);
  };

  const handleClose = () => {
    setIsOpen(false);
    // discard dialog selection, input stays unchanged
    setPendingAsset(null);
  };

  useEffect(() => {
    if (!isOpen) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") handleClose();
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [isOpen]);

  return (
    <div className="relative">
      <div className="flex flex-col gap-1 md:flex-row md:items-center md:gap-5">
        <label className="text-sm font-medium text-gray-700 md:w-28 md:shrink-0">
          Asset
        </label>
        <div className="flex-1 min-w-0" data-testid={data_testid}>
          <div
            role="button"
            tabIndex={0}
            onClick={handleOpen}
            onKeyDown={(e) => {
              if ((e.key === "Enter" || e.key === " ") && !isOpen) {
                e.preventDefault();
                handleOpen();
              }
            }}
            className="h-9 flex items-center justify-between w-full px-3 py-2 border border-gray-300 rounded cursor-pointer bg-white hover:border-primary focus:outline-none focus:ring-2 focus:ring-primary/30 transition-colors"
          >
            {/* Input display is driven by value prop only, never pendingAsset */}
            {value ? (
              <span className="text-neutral-800 break-all">{value.assetName}</span>
            ) : (
              <span className="text-gray-400">{placeholder}</span>
            )}
            <Search className="w-4 h-4 text-gray-400 shrink-0 ml-2" />
          </div>
        </div>

        {/* Dialog overlay */}
        {isOpen && (
          <FocusTrap
            focusTrapOptions={{
              escapeDeactivates: false, // we handle Escape ourselves above
              allowOutsideClick: true,  // lets the backdrop click still close it
            }}
          >
            <div
              className="fixed inset-0 z-50 bg-black/40 flex items-end sm:items-center justify-center sm:p-4"
              onClick={handleClose}
            >
              <div
                className="bg-white rounded-t-2xl sm:rounded-lg shadow-xl w-full sm:max-w-3xl flex flex-col max-h-[90dvh]"
                onClick={(e) => e.stopPropagation()}
                onKeyDown={(e) => {
                  e.stopPropagation();
                  if (e.key === "Enter") e.preventDefault(); // ← ngăn form submit
                }}
              >
                <AssetsLookupTable
                  isOpen={isOpen}
                  onConfirm={handleSave}
                  onClose={handleClose}
                  pendingAsset={pendingAsset}
                  onPendingAssetChange={setPendingAsset}
                />
              </div>
            </div>
          </FocusTrap>
        )}
      </div>
    </div>
  );
};

export default AssetsLookupInput;
