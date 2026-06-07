"use client";

import { LookupAssetsSummary } from "@/features/Assets/assets.types";
import { Search } from "lucide-react";
import { useEffect, useState } from "react";
import { AssetsLookupTable } from "./AssetsLookupTable";

export interface AssetsLookupInputProps {
  value?: LookupAssetsSummary | null;
  onChange?: (asset: LookupAssetsSummary | null) => void;
  placeholder?: string;
}

const AssetsLookupInput = ({
  value,
  onChange,
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
      <div className="flex flex-col gap-2 md:flex-row md:items-center md:gap-4">
        <label className="text-sm font-medium text-gray-700 md:w-32 md:shrink-0">
          Asset
        </label>
        <div className="flex-1 min-w-0">
          <div
            role="button"
            tabIndex={0}
            onClick={handleOpen}
            onKeyDown={(e) => {
              if (e.key === "Enter" && !isOpen) handleOpen(); // ← thêm !isOpen
            }}
            className="flex items-center justify-between w-full px-3 py-2 border border-gray-300 rounded cursor-pointer bg-white hover:border-primary focus:outline-none focus:ring-2 focus:ring-primary/30 transition-colors"
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
        )}
      </div>
    </div>
  );
};

export default AssetsLookupInput;
