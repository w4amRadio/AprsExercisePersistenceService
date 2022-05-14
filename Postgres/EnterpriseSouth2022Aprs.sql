-- Table: public.EnterpriseSouth2022Aprs

-- DROP TABLE IF EXISTS public."EnterpriseSouth2022Aprs";

CREATE TABLE IF NOT EXISTS public."EnterpriseSouth2022Aprs"
(
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "TncHeader" text COLLATE pg_catalog."default",
    "AprsHeader" text COLLATE pg_catalog."default",
    "Location" geography,
    "Text" text COLLATE pg_catalog."default",
	"Radio" text COLLATE pg_catalog."default",
    "WeatherInformation" text COLLATE pg_catalog."default",
    "Altitude" text COLLATE pg_catalog."default",
	"Course" text COLLATE pg_catalog."default",
    "DateTime" timestamp with time zone,
	"Voltage" text COLLATE pg_catalog."default",
	"From" text COLLATE pg_catalog."default",
	"To" text COLLATE pg_catalog."default"
    CONSTRAINT "EnterpriseSouth2022Aprs_pkey" PRIMARY KEY ("Id")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."EnterpriseSouth2022Aprs"
    OWNER to postgres;
-- Index: location_index

-- DROP INDEX IF EXISTS public.location_index;

CREATE INDEX IF NOT EXISTS location_index
    ON public."EnterpriseSouth2022Aprs" USING gist
    ("Location")
    TABLESPACE pg_default;
