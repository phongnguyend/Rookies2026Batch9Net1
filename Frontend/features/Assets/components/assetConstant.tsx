import { AssetState } from "../assets.types";

//Character input validator
const EMOJI_REGEX = /(\p{Extended_Pictographic}|\p{Emoji_Component})/gu;
export const stripEmoji = (value: string) => value.replace(EMOJI_REGEX, "");
export const normalizeText = (value: string) =>
  stripEmoji(value).replace(/\s+/g, " ").trim();
export const ALLOWED_REGEX = /^(?=.*[\p{L}])[\p{L}\p{N}"\/\-\|\(\)\+\., ]+$/u;

//Format date
export const formatDateToISO = (date: Date | null): string => {
  if (!date || isNaN(date.getTime())) return "";
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
};

//State

export const EDITABLE_STATES = [
  {
    value: AssetState.Available,
    label: "Available",
    testId: "rdoAvailable",
  },
  {
    value: AssetState.NotAvailable,
    label: "Not available",
    testId: "rdoNotAvailable",
  },
  {
    value: AssetState.WaitingForRecycling,
    label: "Waiting for recycling",
    testId: "rdoWaiting",
  },
  {
    value: AssetState.Recycled,
    label: "Recycled",
    testId: "rdoRecycled",
  },
];
