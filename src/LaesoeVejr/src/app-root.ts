import "bootstrap/dist/css/bootstrap.css";
import "bootstrap/dist/js/bootstrap";
import "bootstrap-icons/font/bootstrap-icons.css";
import "./app-root.css";

import { ICustomElementViewModel, customElement, resolve } from "aurelia";

import { ThemeService } from "./theme-service";
import { route } from "@aurelia/router";
import template from "./app-root.html";

@route({
  title: "Vejret på Læsø",
  routes: [
    {
      path: "",
      component: import("./dashboard"),
    },
    {
      path: "cameras/:cameraId",
      component: import("./camera"),
    },
  ],
})
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
