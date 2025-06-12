import * as da from "./locales/da.json";

import Aurelia, { LoggerConfiguration } from "aurelia";
import { DateTime, Settings } from "luxon";

import { AppRootCustomElement } from "./app-root";
import { I18nConfiguration } from "@aurelia/i18n";
import { RoundValueConverter } from "./resources/round-value-converter";
import { RouterConfiguration } from "@aurelia/router";

Settings.defaultLocale = "da-DK";
Settings.throwOnInvalid = true;

console.log("Time is now", DateTime.local().toLocaleString(DateTime.DATETIME_FULL_WITH_SECONDS));

const aurelia = new Aurelia()
  .register(
    RouterConfiguration.customize({
      title: "Vejret på Læsø",
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
