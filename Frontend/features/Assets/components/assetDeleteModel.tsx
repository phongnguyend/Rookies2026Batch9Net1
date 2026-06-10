"use client";

import { useRouter } from "next/navigation";
import { useDeleteAssetMutation } from "@/features/Assets/assets.api";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { useAppDispatch } from "@/lib/redux/hooks";
import { useEffect, useRef } from "react";
import { X } from "lucide-react";
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
  const dialogRef = useRef<HTMLDialogElement>(null);
  const [deleteAsset, { isLoading }] = useDeleteAssetMutation();
  useEffect(() => {
    const dialog = dialogRef.current;
    if (dialog && assetId) {
      if (!dialog.open) {
        dialog.showModal();
      }
    }
  }, [assetId]);

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
    <dialog ref={dialogRef} className="modal" onClose={onClose}>
      <div className="modal modal-open">
        <div className="modal-box max-w-md p-0 overflow-hidden rounded-lg">
          {hasHistory ? (
            <>
              {/* Header */}
              <div className="bg-[#f1f3f5] border-b border-gray-500 px-6 py-3.5 flex items-center justify-between">
                <h3 className="text-lg font-bold text-primary">
                  Cannot Delete Asset
                </h3>
                <button
                  data-testid="btnCloseDisablePopup"
                  onClick={onClose}
                  className="hover:cursor-pointer flex h-7 w-7 items-center justify-center rounded border-2 border-primary text-primary transition hover:bg-primary hover:text-white"
                >
                  <X />
                </button>
              </div>

              {/* Body */}
              <div className="px-6 py-6 text-neutral-800 text-base leading-relaxed pb-4">
                <p className="">
                  Cannot delete the asset because it belongs to one or more
                  historical assignments.
                  <br />
                  If the asset is not able to be used anymore, please update its
                  state in{" "}
                  <button
                    data-testid="lnkEditAssetPage"
                    onClick={() => {
                      onClose();
                      router.push(`/admin/assets/edit?id=${assetId}`);
                    }}
                    className="text-sky-500 underline hover:text-sky-700 hover:cursor-pointer"
                  >
                    Edit Asset page
                  </button>
                </p>
              </div>
            </>
          ) : (
            <>
              {/* Header */}
              <div className="bg-[#f1f3f5] border-b border-gray-500 px-6 py-3.5 flex items-center justify-between">
                <h3 className="text-lg font-bold text-primary">
                  Are you sure?
                </h3>
              </div>
              {/* Body */}
              <div className="px-6 pt-6 text-neutral-800 text-base leading-relaxed pb-4">
                <p>
                  Do you want to delete this asset?
                </p>
              </div>
              {/* Footer */}
              <div className="px-6 pb-6 pt-0 bg-white flex items-center justify-start gap-3">
                <button
                  data-testid="btnDelete"
                  onClick={handleDelete}
                  disabled={isLoading}
                  className="hover:cursor-pointer px-4 py-2 bg-primary hover:bg-red-600 active:bg-primary/95 text-white font-semibold rounded flex items-center gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {isLoading ? "Deleting..." : "Delete"}
                </button>
                <button
                  data-testid="btnCancel"
                  onClick={onClose}
                  disabled={isLoading}
                  className="px-4 py-2 border border-gray-400 rounded text-[#6c757d] font-normal hover:bg-gray-100 transition-colors duration-150 disabled:opacity-50 disabled:cursor-not-allowed hover:cursor-pointer"
                >
                  Cancel
                </button>
              </div>
            </>
          )}
        </div>
        <div className="modal-backdrop" onClick={onClose} />
      </div>
    </dialog>
  );
}
