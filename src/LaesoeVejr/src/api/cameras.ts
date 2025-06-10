import { CameraId } from "../cameras";
import { http } from "./http";

export const getImageUrl = (cameraId: CameraId) => http.get(`/cameras/${cameraId}/image.jpg`).getUrl();
