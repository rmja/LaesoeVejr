SELECT
	time,
	station_id,
	wind_speed,
	wind_gust,
	wind_dir AS wind_direction,
	air_temp AS air_temperature,
	water_temp AS water_temperature,
	sea_level,
	pressure AS atmospheric_pressure,	
	humidity
	FROM laesoe_weather
WHERE
	station_id = @stationId
ORDER BY
	time DESC
LIMIT @limit;
