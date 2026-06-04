import type { AssetListItem } from "./assets.types";

let _pinnedEditedAsset: AssetListItem | null = null;

export const setPinnedEditedAsset = (asset: AssetListItem) => {
  _pinnedEditedAsset = asset;
};

export const getPinnedEditedAsset = () => _pinnedEditedAsset;

export const clearPinnedEditedAsset = () => {
  _pinnedEditedAsset = null;
};
