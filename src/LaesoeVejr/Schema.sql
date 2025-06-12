CREATE TABLE laesoe_weather (
  time timestamp,
  station_id symbol,
  wind_speed double,
  wind_gust double,
  wind_dir double,
  air_temp double,
  water_temp double,
  sea_level double,
  pressure double,
  humidity double
) TIMESTAMP(time) PARTITION BY MONTH;

-- DROP MATERIALIZED VIEW laesoe_weather_5min;
CREATE MATERIALIZED VIEW IF NOT EXISTS laesoe_weather_5min AS (
  SELECT
  time,
  station_id,
  avg(wind_speed) wind_speed_avg,
  min(wind_speed) wind_speed_min,
  max(wind_speed) wind_speed_max,
  avg(wind_gust) wind_gust_avg,
  min(wind_gust) wind_gust_min,
  max(wind_gust) wind_gust_max,
  avg(wind_dir) wind_dir_avg,
  avg(air_temp) air_temp_avg,
  min(air_temp) air_temp_min,
  max(air_temp) air_temp_max,
  avg(water_temp) water_temp_avg,
  min(water_temp) water_temp_min,
  max(water_temp) water_temp_max,
  avg(sea_level) sea_level_avg,
  min(sea_level) sea_level_min,
  max(sea_level) sea_level_max,
  avg(pressure) pressure_avg,
  min(pressure) pressure_min,
  max(pressure) pressure_max,
  avg(humidity) humidity_avg,
  min(humidity) humidity_min,
  max(humidity) humidity_max
  FROM laesoe_weather
  SAMPLE BY 5m ALIGN TO CALENDAR TIME ZONE 'Europe/Copenhagen'
) PARTITION BY day TTL 7 days;

-- DROP MATERIALIZED VIEW laesoe_weather_hour;
CREATE MATERIALIZED VIEW IF NOT EXISTS laesoe_weather_hour AS (
  SELECT
  time,
  station_id,
  avg(wind_speed) wind_speed_avg,
  min(wind_speed) wind_speed_min,
  max(wind_speed) wind_speed_max,
  avg(wind_gust) wind_gust_avg,
  min(wind_gust) wind_gust_min,
  max(wind_gust) wind_gust_max,
  avg(wind_dir) wind_dir_avg,
  avg(air_temp) air_temp_avg,
  min(air_temp) air_temp_min,
  max(air_temp) air_temp_max,
  avg(water_temp) water_temp_avg,
  min(water_temp) water_temp_min,
  max(water_temp) water_temp_max,
  avg(sea_level) sea_level_avg,
  min(sea_level) sea_level_min,
  max(sea_level) sea_level_max,
  avg(pressure) pressure_avg,
  min(pressure) pressure_min,
  max(pressure) pressure_max,
  avg(humidity) humidity_avg,
  min(humidity) humidity_min,
  max(humidity) humidity_max
  FROM laesoe_weather
  SAMPLE BY 1h ALIGN TO CALENDAR TIME ZONE 'Europe/Copenhagen'
) PARTITION BY month TTL 3 months;

-- DROP MATERIALIZED VIEW laesoe_weather_day;
CREATE MATERIALIZED VIEW IF NOT EXISTS laesoe_weather_day AS (
  SELECT
  time,
  station_id,
  avg(wind_speed) wind_speed_avg,
  min(wind_speed) wind_speed_min,
  max(wind_speed) wind_speed_max,
  avg(wind_gust) wind_gust_avg,
  min(wind_gust) wind_gust_min,
  max(wind_gust) wind_gust_max,
  avg(wind_dir) wind_dir_avg,
  avg(air_temp) air_temp_avg,
  min(air_temp) air_temp_min,
  max(air_temp) air_temp_max,
  avg(water_temp) water_temp_avg,
  min(water_temp) water_temp_min,
  max(water_temp) water_temp_max,
  avg(sea_level) sea_level_avg,
  min(sea_level) sea_level_min,
  max(sea_level) sea_level_max,
  avg(pressure) pressure_avg,
  min(pressure) pressure_min,
  max(pressure) pressure_max,
  avg(humidity) humidity_avg,
  min(humidity) humidity_min,
  max(humidity) humidity_max
  FROM laesoe_weather
  SAMPLE BY 1d ALIGN TO CALENDAR TIME ZONE 'Europe/Copenhagen'
) PARTITION BY year;

-- DROP MATERIALIZED VIEW laesoe_weather_month;
CREATE MATERIALIZED VIEW IF NOT EXISTS laesoe_weather_month AS (
  SELECT
  time,
  station_id,
  avg(wind_speed) wind_speed_avg,
  min(wind_speed) wind_speed_min,
  max(wind_speed) wind_speed_max,
  avg(wind_gust) wind_gust_avg,
  min(wind_gust) wind_gust_min,
  max(wind_gust) wind_gust_max,
  avg(wind_dir) wind_dir_avg,
  avg(air_temp) air_temp_avg,
  min(air_temp) air_temp_min,
  max(air_temp) air_temp_max,
  avg(water_temp) water_temp_avg,
  min(water_temp) water_temp_min,
  max(water_temp) water_temp_max,
  avg(sea_level) sea_level_avg,
  min(sea_level) sea_level_min,
  max(sea_level) sea_level_max,
  avg(pressure) pressure_avg,
  min(pressure) pressure_min,
  max(pressure) pressure_max,
  avg(humidity) humidity_avg,
  min(humidity) humidity_min,
  max(humidity) humidity_max
  FROM laesoe_weather
  SAMPLE BY 1M ALIGN TO CALENDAR TIME ZONE 'Europe/Copenhagen'
) PARTITION BY year;
