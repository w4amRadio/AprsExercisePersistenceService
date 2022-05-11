-- PROCEDURE: public.sp_persist_aprs(text, text, text, text, text, text)

-- DROP PROCEDURE IF EXISTS public.sp_persist_aprs(text, text, text, text, text, text);

CREATE OR REPLACE PROCEDURE public.sp_persist_aprs(
	IN _tncheader text,
	IN _aprsheader text,
	IN _location text,
	IN _text text,
	IN _weatherinformation text,
	IN _altitude text)
LANGUAGE 'plpgsql'
AS $BODY$
  begin
    INSERT INTO public."EnterpriseSouth2022Aprs" ("DateTime", "TncHeader", "AprsHeader", "Location", "Text", "WeatherInformation", "Altitude") VALUES
    (Now(), _TncHeader, _AprsHeader, ST_GeographyFromText(_Location), _Text, _WeatherInformation, _Altitude);
    commit;
  end;
$BODY$;
ALTER PROCEDURE public.sp_persist_aprs(text, text, text, text, text, text)
    OWNER TO postgres;
