import { customElement, resolve } from "aurelia";

import { CameraId } from "./cameras";
import { IApiClient } from "./api";
import template from "./camera.html";

@customElement({ name: "camera-page", template })
export class CameraPage {
  cameraId!: CameraId;
  previewImageUrl!: string;
  originalImageUrl!: string;

  constructor(private api = resolve(IApiClient)) {}

  loading(params: { cameraId: CameraId }) {
    this.cameraId = params.cameraId;
    this.previewImageUrl = this.api.cameras.getPreviewImageUrl(this.cameraId);
    this.originalImageUrl = this.api.cameras.getOriginalImageUrl(this.cameraId);
  }
}
