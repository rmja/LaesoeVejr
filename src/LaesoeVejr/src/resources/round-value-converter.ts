import { valueConverter } from "aurelia";

@valueConverter("round")
export class RoundValueConverter {
  toView(value: number, decimals: number = 0): string {
    if (isNaN(value)) {
      return "";
    }
    return value.toFixed(decimals);
  }
}
