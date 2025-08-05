import { dateTimeConverter, jsonProperty } from "@utiliread/json";

import { DateTime } from "luxon";
import { StationId } from "./primitives";
import { http } from "./http";

export class WeatherData {
  @jsonProperty({ converter: dateTimeConverter })
  time!: DateTime;
  @jsonProperty()
  stationId!: StationId;
  @jsonProperty()
  windSpeed: number | null = null;
  @jsonProperty()
  windGust: number | null = null;
  @jsonProperty()
  windDirection: number | null = null;
  @jsonProperty()
  airTemperature: number | null = null;
  @jsonProperty()
  waterTemperature: number | null = null;
  @jsonProperty()
  seaLevel: number | null = null;
  @jsonProperty()
  humidity: number | null = null;
  @jsonProperty()
  atmosphericPressure: number | null = null;
}

export class AggregateWeatherData {
  @jsonProperty({ converter: dateTimeConverter })
  time!: DateTime;
  @jsonProperty()
  stationId!: StationId;
  @jsonProperty()
  windSpeedAvg: number | null = null;
  @jsonProperty()
  windSpeedMin: number | null = null;
  @jsonProperty()
  windSpeedMax: number | null = null;
  @jsonProperty()
  windGustAvg: number | null = null;
  @jsonProperty()
  windGustMin: number | null = null;
  @jsonProperty()
  windGustMax: number | null = null;
  @jsonProperty()
  windDirectionAvg: number | null = null;
  @jsonProperty()
  airTemperatureAvg: number | null = null;
  @jsonProperty()
  airTemperatureMin: number | null = null;
  @jsonProperty()
  airTemperatureMax: number | null = null;
  @jsonProperty()
  waterTemperatureAvg: number | null = null;
  @jsonProperty()
  waterTemperatureMin: number | null = null;
  @jsonProperty()
  waterTemperatureMax: number | null = null;
  @jsonProperty()
  seaLevelAvg: number | null = null;
  @jsonProperty()
  seaLevelMin: number | null = null;
  @jsonProperty()
  seaLevelMax: number | null = null;
  @jsonProperty()
  humidityAvg: number | null = null;
  @jsonProperty()
  humidityMin: number | null = null;
  @jsonProperty()
  humidityMax: number | null = null;
  @jsonProperty()
  atmosphericPressureAvg: number | null = null;
  @jsonProperty()
  atmosphericPressureMin: number | null = null;
  @jsonProperty()
  atmosphericPressureMax: number | null = null;
}

export type Step = "month" | "day" | "hour" | "5min";

export const getWeather = (stationId: StationId, params?: { limit?: number }) =>
  http.get(`/stations/${stationId}/weather`, params).expectJsonArray(WeatherData).transfer();

export const getWeatherEventSource = (stationId: StationId) =>
  new EventSource(http.get(`/stations/${stationId}/weather/events`).getUrl());

export const getWeatherHistory = (stationId: StationId, params?: { start: DateTime; end: DateTime; step: Step }) =>
  http.get(`/stations/${stationId}/weather/history`, params).expectJsonArray(AggregateWeatherData).transfer();
