import "@utiliread/http/json";

import * as cameras from "./cameras";
import * as weather from "./weather";

import { DI } from "aurelia";

export const IApiClient = DI.createInterface<IApiClient>("IApiClient", (x) => x.singleton(ApiClient));
export type IApiClient = Required<ApiClient>;

export class ApiClient {
  cameras = cameras;
  weather = weather;
}
