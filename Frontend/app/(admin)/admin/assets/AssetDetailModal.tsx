"use client";

import { useGetAssetByIdQuery } from "@/features/assets/assets.api";

interface AssetDetailModalProps {
  assetId: string | null;
  onClose: () => void;
}

export default function AssetDetailModal({ assetId, onClose }: AssetDetailModalProps) {
  const { data, isLoading } = useGetAssetByIdQuery(assetId!, {
    skip: !assetId,   // ← don't fetch if no id
  });

  if (!assetId) return null;

  return (
    <dialog open className="modal modal-open">
      <div className="modal-box max-w-lg">

        {/* Header */}
        <div className="bg-primary text-white -mx-6 -mt-6 px-6 py-4 mb-6 flex items-center justify-between rounded-t-2xl">
          <h3 className="font-bold text-lg">Detailed Asset Information</h3>
          <button onClick={onClose} className="text-white/80 hover:text-white">✕</button>
        </div>

        {/* Body */}
        {isLoading ? (
          <div className="flex justify-center py-8">
            <span className="loading loading-spinner loading-md" />
          </div>
        ) : data ? (
          <div className="space-y-3 text-sm">
            <Row label="Asset Code" value={data.assetCode} />
            <Row label="Name" value={data.name} />
            <Row label="Category" value={data.category} />
            <Row label="Location" value={data.location} />
            <Row label="State" value={data.state} />
            <Row label="Specification" value={data.specification} />
            <Row
              label="Installed Date"
              value={new Date(data.installedAtUtc).toLocaleDateString()}
            />
          </div>
        ) : null}
      </div>

      {/* Backdrop */}
      <div className="modal-backdrop" onClick={onClose} />
    </dialog>
  );
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex gap-4 border-b border-gray-100 pb-2">
      <span className="w-36 font-semibold text-gray-500 shrink-0">{label}</span>
      <span className="text-gray-800">{value}</span>
    </div>
  );
}
