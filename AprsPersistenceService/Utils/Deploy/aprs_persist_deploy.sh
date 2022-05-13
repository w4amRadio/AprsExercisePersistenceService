#!/bin/bash
rm -r /home/pi/aprs_persist
mkdir /home/pi/aprs_persist
sleep 2s
mv aprs_persist.tar /home/pi/aprs_persist/
sleep 2s
tar -xvf /home/pi/aprs_persist/aprs_persist.tar -C /home/pi/aprs_persist/
rm /home/pi/aprs_persist/aprs_persist.tar