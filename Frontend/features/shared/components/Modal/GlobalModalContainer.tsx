"use client";

import { useAppDispatch, useAppSelector } from "@/lib/redux/hooks";
import { hideModal } from "@/features/shared/modal.slice";
import ConfirmModal from "./ConfirmModal";

export default function GlobalModalContainer() {
  const dispatch = useAppDispatch();
  const {
    isOpen,
    title,
    body,
    yesButtonLabel,
    noButtonLabel,
    yesActionType,
    yesPayload,
    noActionType,
    noPayload,
  } = useAppSelector((state) => state.modalSlice);

  const handleYes = () => {
    if (yesActionType) {
      dispatch({ type: yesActionType, payload: yesPayload });
    }
    dispatch(hideModal());
  };

  const handleNo = () => {
    if (noActionType) {
      dispatch({ type: noActionType, payload: noPayload });
    }
    dispatch(hideModal());
  };

  return (
    <ConfirmModal
      isOpen={isOpen}
      onYes={handleYes}
      onNo={handleNo}
      onClose={() => dispatch(hideModal())}
      title={title}
      body={body}
      yesButtonLabel={yesButtonLabel}
      noButtonLabel={noButtonLabel}
    />
  );
}
