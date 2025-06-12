import { valueConverter } from "aurelia";

@valueConverter("round")
export class RoundValueConverter {
  toView(value: number | null | undefined, decimals: number = 0): string {
    if (value === null || value == undefined || isNaN(value)) {
      return "";
    }
    return value.toFixed(decimals);
  }
}
