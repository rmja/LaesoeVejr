import "bootstrap/dist/css/bootstrap.css";
import "bootstrap/dist/js/bootstrap";
import "bootstrap-icons/font/bootstrap-icons.css";
import "./app-root.css";

import { ICustomElementViewModel, customElement, resolve } from "aurelia";

import { ThemeService } from "./theme-service";
import { routes } from "@aurelia/router";
import template from "./app-root.html";

@routes([
  {
    id: "dashboard",
    // title: "asd",
    path: "",
    component: import("./dashboard"),
  },
  {
    id: "camera",
    path: "cameras/:cameraId",
    component: import("./camera"),
  },
])
@customElement({
  name: "app-root",
  template,
})
export class AppRootCustomElement implements ICustomElementViewModel {
  constructor(private themeService = resolve(ThemeService)) {}

  get theme() {
    return this.themeService.theme;
  }

  useTheme(theme: "light" | "dark" | "auto") {
    this.themeService.useTheme(theme);
  }
}
