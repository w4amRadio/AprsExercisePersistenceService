-- PROCEDURE: public.sp_persist_aprs(text, text, text, text, text, text)

-- DROP PROCEDURE IF EXISTS public.sp_persist_aprs(text, text, text, text, text, text);

CREATE OR REPLACE PROCEDURE public.sp_persist_aprs(
	IN _tncheader text,
	IN _aprsheader text,
	IN _location text,
	IN _text text,
	IN _weatherinformation text,
	IN _altitude text,
	IN _radio text,
	IN _course text,
	IN _voltage text,
	IN _from text,
	IN _to text)
LANGUAGE 'plpgsql'
AS $BODY$
  begin
	IF _Location IS NULL OR _Location = '' THEN
		INSERT INTO public."EnterpriseSouth2022Aprs" ("DateTime", "TncHeader", "AprsHeader", "Location", "Text", "WeatherInformation", "Altitude", "Radio", "Course", "Voltage", "From", "To") VALUES
		(Now(), _TncHeader, _AprsHeader, NULL, _Text, _WeatherInformation, _Altitude, _Radio, _Course, _Voltage, _From, _To);
	ELSE
		INSERT INTO public."EnterpriseSouth2022Aprs" ("DateTime", "TncHeader", "AprsHeader", "Location", "Text", "WeatherInformation", "Altitude", "Radio", "Course", "Voltage", "From", "To") VALUES
		(Now(), _TncHeader, _AprsHeader, ST_GeographyFromText(_Location), _Text, _WeatherInformation, _Altitude, _Radio, _Course, _Voltage, _From, _To);
	END IF;
    commit;
  end;
$BODY$;
ALTER PROCEDURE public.sp_persist_aprs(text, text, text, text, text, text)
    OWNER TO postgres;
