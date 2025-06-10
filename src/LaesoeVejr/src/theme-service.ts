type Theme = "light" | "dark" | "auto";

export class ThemeService {
  private _theme!: Theme;

  get theme(): Theme {
    return this._theme;
  }

  constructor() {
    const storedTheme = this.getStoredTheme();
    this.setTheme(storedTheme ?? "auto");

    window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", () => this.setTheme(this._theme));
  }

  useTheme(theme: Theme) {
    this.setTheme(theme);
    this.setStoredTheme(theme);
  }

  private setTheme(theme: Theme) {
    this._theme = theme;

    const resolvedTheme =
      theme === "auto" ? (window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light") : theme;
    document.documentElement.setAttribute("data-bs-theme", resolvedTheme);
  }

  private getStoredTheme() {
    return localStorage.getItem("theme") as Theme | null;
  }

  private setStoredTheme(theme: Theme) {
    localStorage.setItem("theme", theme);
  }
}
