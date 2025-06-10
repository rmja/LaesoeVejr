SELECT
	time,
	station_id,
	wind_speed_avg,
	wind_speed_min,
	wind_speed_max,
	wind_gust_avg,
	wind_gust_min,
	wind_gust_max,
	wind_dir_avg AS wind_direction_avg,
	air_temp_avg AS air_temperature_avg,
	air_temp_min AS air_temperature_min,
	air_temp_max AS air_temperature_max,
	water_temp_avg AS water_temperature_avg,
	water_temp_min AS water_temperature_min,
	water_temp_max AS water_temperature_max,
	sea_level_avg,
	sea_level_min,
	sea_level_max,
	pressure_avg AS atmospheric_pressure_avg,	
	pressure_min AS atmospheric_pressure_min,
	pressure_max AS atmospheric_pressure_max,
	humidity_avg,
	humidity_min,
	humidity_max
	FROM {{view}}
WHERE
	station_id = @stationId
	AND time >= @start AND time < @end
ORDER BY
	time;
