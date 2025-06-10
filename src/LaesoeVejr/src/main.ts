import * as da from "./locales/da.json";

import Aurelia, { LoggerConfiguration } from "aurelia";

import { AppRootCustomElement } from "./app-root";
import { I18nConfiguration } from "@aurelia/i18n";
import { RoundValueConverter } from "./resources/round-value-converter";
import { RouterConfiguration } from "@aurelia/router";
import { Settings } from "luxon";

Settings.throwOnInvalid = true;

const aurelia = new Aurelia()
  .register(
    RouterConfiguration.customize({
      title: "${componentTitles}${appTitleSeparator}Vejret på Læsø",
      useUrlFragmentHash: false,
    }),
  )
  .register(LoggerConfiguration.create())
  .register(RoundValueConverter)
  .register(
    I18nConfiguration.customize(
      (options) =>
        (options.initOptions = {
          lng: "da",
          resources: {
            da: { translation: da },
          },
        }),
    ),
  )
  .app({
    component: AppRootCustomElement,
    host: document.querySelector("app-root")!,
  });

await aurelia.start();
