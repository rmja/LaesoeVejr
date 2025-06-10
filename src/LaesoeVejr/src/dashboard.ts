import "chartjs-adapter-luxon";

import { AggregateWeatherData, Step, WeatherData } from "./api/weather";
import {
  BarController,
  BarElement,
  CategoryScale,
  Chart,
  Legend,
  LineController,
  LineElement,
  LinearScale,
  PointElement,
  TimeScale,
} from "chart.js";
import { DateTime, Duration } from "luxon";
import { customElement, resolve } from "aurelia";

import { CameraId } from "./cameras";
import { IApiClient } from "./api";
import { StationId } from "./api/primitives";
import template from "./dashboard.html";

Chart.register(
  BarController,
  CategoryScale,
  LinearScale,
  BarElement,
  TimeScale,
  LineController,
  PointElement,
  LineElement,
  Legend,
);

interface DateTimePoint {
  x: DateTime;
  y: number;
}

const STATION_ID: StationId = "vesteroe-havn";
type Period = "past" | "PT24H" | "P30D" | "P24M";

@customElement({ name: "dashboard-page", template })
export class DashboardPage {
  current!: WeatherDataViewModel;
  private past!: WeatherData[];
  private history!: AggregateWeatherData[];
  periods: Period[] = ["past", "PT24H", "P30D", "P24M"];
  selectedPeriod: Period = "past";
  cameras: CameraId[] = [
    "laesoefaergen-oest",
    "laesoefaergen-vest",
    "havnekontoret-syd",
    "havnekontoret-nord",
    "flyvepladsen-telemast",
    "flyvepladsen-vindpose",
  ];

  private windChart!: Chart<"line", DateTimePoint[]>;
  private temperatureChart!: Chart<"line", DateTimePoint[]>;
  private waterLevelChart!: Chart<"bar", DateTimePoint[]>;
  windCanvas!: HTMLCanvasElement;
  temperatureCanvas!: HTMLCanvasElement;
  waterLevelCanvas!: Chart<"bar", DateTimePoint[]>;

  constructor(private api = resolve(IApiClient)) {}

  getCameraImageUrl(cameraId: CameraId): string {
    return this.api.cameras.getImageUrl(cameraId);
  }

  async loading() {
    this.past = await this.api.weather.getWeather(STATION_ID, { limit: 500 });
    this.current = this.past[0];
  }

  attached() {
    this.windChart = new Chart<"line", DateTimePoint[]>(this.windCanvas, {
      type: "line",
      data: {
        datasets: [
          {
            label: "Middelvind",
            data: getChartData(this.past, (x) => x.windSpeed),
            borderColor: "#1ab394",
            backgroundColor: "#ffffff00",
          },
          {
            label: "Vindstød",
            data: getChartData(this.past, (x) => x.windGust),
            borderDash: [5, 5],
            borderColor: "#1ab394",
            backgroundColor: "#ffffff00",
          },
        ],
      },
      options: {
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: true,
          },
        },
        scales: {
          x: {
            type: "time",
          },
          y: {
            title: {
              display: true,
              text: "Vindhastighed (m/s)",
            },
          },
        },
      },
    });

    this.temperatureChart = new Chart<"line", DateTimePoint[]>(this.temperatureCanvas, {
      type: "line",
      data: {
        datasets: [
          {
            label: "Lufttemperatur",
            data: getChartData(this.past, (x) => x.airTemperature),
            borderColor: "#ed5565",
            backgroundColor: "#ffffff00",
          },
          {
            label: "Vandtemperatur",
            data: getChartData(this.past, (x) => x.waterTemperature),
            borderColor: "#1c84c6",
            backgroundColor: "#ffffff00",
          },
        ],
      },
      options: {
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: true,
          },
        },
        scales: {
          x: {
            type: "time",
          },
          y: {
            title: {
              display: true,
              text: "Temperatur (°C)",
            },
          },
        },
      },
    });

    this.waterLevelChart = new Chart<"bar", DateTimePoint[]>(this.waterLevelCanvas, {
      type: "bar",
      data: {
        datasets: [
          {
            data: getChartData(this.past, (x) => x.seaLevel),
            backgroundColor: "#7b70ef",
          },
        ],
      },
      options: {
        maintainAspectRatio: false,
        plugins: {
          legend: {
            display: false,
          },
        },
        scales: {
          x: {
            type: "time",
          },
          y: {
            title: {
              display: true,
              text: "Vandstand (m)",
            },
            beginAtZero: true,
          },
        },
      },
    });
  }

  detaching() {
    this.windChart.destroy();
    this.temperatureChart.destroy();
    this.waterLevelChart.destroy();
  }

  async setChartPeriod(period: Period) {
    this.selectedPeriod = period;

    if (this.selectedPeriod === "past") {
      this.windChart.data.datasets[0].data = getChartData(this.past, (x) => x.windSpeed);
      this.windChart.data.datasets[1].data = getChartData(this.past, (x) => x.windGust);
      this.windChart.update();

      this.temperatureChart.data.datasets[0].data = getChartData(this.past, (x) => x.airTemperature);
      this.temperatureChart.data.datasets[1].data = getChartData(this.past, (x) => x.waterTemperature);
      this.temperatureChart.update();

      this.waterLevelChart.data.datasets[0].data = getChartData(this.past, (x) => x.seaLevel);
      this.waterLevelChart.update();
    } else {
      const { start, end, step } = this.getHistoryPeriod();
      this.history = await this.api.weather.getWeatherHistory("vesteroe-havn", {
        start,
        end,
        step,
      });

      this.windChart.data.datasets[0].data = getChartData(this.history, (x) => x.windSpeedAvg);
      this.windChart.data.datasets[1].data = getChartData(this.history, (x) => x.windGustAvg);
      this.windChart.update();

      this.temperatureChart.data.datasets[0].data = getChartData(this.history, (x) => x.airTemperatureAvg);
      this.temperatureChart.data.datasets[1].data = getChartData(this.history, (x) => x.waterTemperatureAvg);
      this.temperatureChart.update();

      this.waterLevelChart.data.datasets[0].data = getChartData(this.history, (x) => x.seaLevelAvg);
      this.waterLevelChart.update();
    }
  }

  private getHistoryPeriod() {
    const duration = Duration.fromISO(this.selectedPeriod);
    const step = duration.hours ? "hour" : duration.days ? "day" : "month";
    const end = DateTime.local()
      .startOf(step)
      .plus({ [step]: 1 });
    return {
      start: end.minus(duration),
      end,
      step: step as Step,
    };
  }
}

interface WeatherDataViewModel {
  windSpeed: number | null;
  windGust: number | null;
  windDirection: number | null;
  waterTemperature: number | null;
  airTemperature: number | null;
  seaLevel: number | null;
  atmosphericPressure: number | null;
  humidity: number | null;
}

function getChartData<T extends { time: DateTime }>(data: T[], selector: (x: T) => number | null): DateTimePoint[] {
  return data.filter((x) => selector(x) !== null).map((x) => ({ x: x.time, y: selector(x)! }));
}
