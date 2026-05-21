"use client";

import { useAppDispatch, useAppSelector } from "@/lib/redux/hooks";
import { hideModal } from "@/features/shared/modal.slice";
import ConfirmModal from "./ConfirmModal";

export default function GlobalModalContainer() {
  const dispatch = useAppDispatch();
  const { isOpen, title, body, confirmLabel, cancelLabel, confirmActionType, confirmPayload } = useAppSelector(
    (state) => state.modalSlice
  );

  const handleConfirm = () => {
    if (confirmActionType) {
      dispatch({ type: confirmActionType, payload: confirmPayload });
    }
    dispatch(hideModal());
  };

  return (
    <ConfirmModal
      isOpen={isOpen}
      onYes={handleConfirm}
      onClose={() => dispatch(hideModal())}
      title={title}
      body={body}
      yesButtonLabel={confirmLabel}
      noButtonLabel={cancelLabel}
    />
  );
}
