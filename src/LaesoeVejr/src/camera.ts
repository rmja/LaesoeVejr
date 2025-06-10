import { customElement, resolve } from "aurelia";

import { CameraId } from "./cameras";
import { IApiClient } from "./api";
import template from "./camera.html";

@customElement({ name: "camera-page", template })
export class CameraPage {
  cameraId!: CameraId;
  cameraImageUrl!: string;

  constructor(private api = resolve(IApiClient)) {}

  load(params: { cameraId: CameraId }) {
    this.cameraId = params.cameraId;
    this.cameraImageUrl = this.api.cameras.getImageUrl(this.cameraId);
  }
}
