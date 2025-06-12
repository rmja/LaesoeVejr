import { CameraId } from "../cameras";
import { http } from "./http";

export const getThumbnailUrl = (cameraId: CameraId) => http.get(`/cameras/${cameraId}/thumbnail.webp`).getUrl();
export const getPreviewImageUrl = (cameraId: CameraId) => http.get(`/cameras/${cameraId}/preview.webp`).getUrl();
export const getOriginalImageUrl = (cameraId: CameraId) => http.get(`/cameras/${cameraId}/image.jpg`).getUrl();

