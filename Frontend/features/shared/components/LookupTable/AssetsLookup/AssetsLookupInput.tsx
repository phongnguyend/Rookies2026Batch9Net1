"use client";

import { LookupAssetsSummary } from "@/features/Assets/assets.types";
import { Search } from "lucide-react";
import { useState } from "react";
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

  return (
    <div className="relative">
      <div className="flex flex-col gap-2 md:flex-row md:items-center md:gap-4">
        <label className="text-sm font-medium text-gray-700 md:w-32 md:shrink-0">
          Asset
        </label>
        <div className="flex-1">
          <div
            role="button"
            tabIndex={0}
            onClick={handleOpen}
            onKeyDown={(e) => e.key === "Enter" && handleOpen()}
            className="flex items-center justify-between w-full px-3 py-2 border border-gray-300 rounded cursor-pointer bg-white hover:border-primary focus:outline-none focus:ring-2 focus:ring-primary/30 transition-colors"
          >
            {/* Input display is driven by value prop only, never pendingAsset */}
            {value ? (
              <span className="text-neutral-800">{value.assetName}</span>
            ) : (
              <span className="text-gray-400">{placeholder}</span>
            )}
            <Search className="w-4 h-4 text-gray-400 shrink-0 ml-2" />
          </div>
        </div>

        {/* Dialog overlay */}
        {isOpen && (
          <div
            className="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
            onClick={handleClose}
          >
            <div
              className="bg-white rounded-lg shadow-xl p-6 w-full max-w-3xl mx-4 max-h-[100vh]"
              onClick={(e) => e.stopPropagation()}
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
