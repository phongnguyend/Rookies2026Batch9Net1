"use client";

import { useRouter } from "next/navigation";
import { useDeleteAssetMutation } from "@/features/Assets/assets.api";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { useAppDispatch } from "@/lib/redux/hooks";
interface DeleteAssetModalProps {
  assetId: string | null;
  assetName: string;
  hasHistory: boolean;
  onClose: () => void;
}

export default function DeleteAssetModal({
  assetId,
  hasHistory,
  onClose,
}: DeleteAssetModalProps) {
  const dispatch = useAppDispatch();
  const router = useRouter();
  const [deleteAsset, { isLoading }] = useDeleteAssetMutation();
  if (!assetId) return null;
  const handleDelete = async () => {
    try {
      await deleteAsset({
        id: assetId,
      }).unwrap();
      onClose();
    } catch {
        dispatch(
          enqueueToast({
            message: `Something went wrong. Somebody has deleted or assigned this asset.`,
            type: ToastType.Error,
          }),
        );
        onClose();
        router.push("/admin/assets");
    }
  };

return (
  <div className="modal modal-open">
    <div className="modal-box max-w-md p-0 overflow-hidden rounded-lg">

      {hasHistory ? (
        <>
          {/* Header */}
          <div className="flex items-center justify-between border-b border-gray-300 bg-gray-100 px-10 py-4">
            <h3 className="text-lg font-bold text-red-600">
              Cannot Delete Asset
            </h3>
            <button
              onClick={onClose}
              className="text-red-600 hover:text-red-800"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="22"
                height="22"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="3"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <rect
                  x="3"
                  y="3"
                  width="18"
                  height="18"
                  rx="2"
                />
                <path d="m9 9 6 6" />
                <path d="m15 9-6 6" />
              </svg>
            </button>
          </div>

          {/* Body */}
          <div className="px-10 py-5">
            <p className="text-sm leading-6 text-gray-700">
              Cannot delete the asset because it belongs to one or more historical assignments.
              <br />
              If the asset is not able to be used anymore, please update its state in{" "}
              <button
                onClick={() => {
                  onClose();
                  router.push(
                    `/admin/assets/edit?id=${assetId}`
                  );
                }}
                className="text-sky-500 underline hover:text-sky-700"
              >
                Edit Asset page
              </button>
            </p>
          </div>
        </>
      ) : (
        <>
          {/* Header */}
          <div className="border-b border-gray-300 bg-gray-100 px-10 py-4">
            <h3 className="text-lg font-bold text-red-600">
              Are you sure?
            </h3>
          </div>
          {/* Body */}
          <div className="px-10 py-5">
            <p className="text-sm text-gray-700">
              Do you want to delete this asset?
            </p>
          </div>
          {/* Footer */}
          <div className="px-10 pb-6 flex gap-4">
            <button
              onClick={handleDelete}
              disabled={isLoading}
              className="
                rounded-md
                bg-red-600
                px-5
                py-2
                text-sm
                font-medium
                text-white
                hover:bg-red-700
                disabled:opacity-50
              "
            >
              {isLoading ? "Deleting..." : "Delete"}
            </button>
            <button
              onClick={onClose}
              disabled={isLoading}
              className="
                rounded-md
                border
                border-gray-400
                px-5
                py-2
                text-sm
                text-gray-600
                hover:bg-gray-100
              "
            >
              Cancel
            </button>
          </div>
        </>
      )}
    </div>
    <div
      className="modal-backdrop"
      onClick={onClose}
    />
  </div>
  );
}
