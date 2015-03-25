#!/bin/sh

cd `dirname $0`

# Fix d-pad button "Left" GPIO setting
echo 165 > /sys/class/gpio/export
echo in > /sys/class/gpio/gpio165/direction

#echo 1 > value

# Compile OLED satellite binary
cd edison-sparkfun-oled/
make
cd ..

# kill old processes
ps | grep app.js | grep -v grep | awk '{print $1}' | xargs kill

# Run node.js app
edison-sparkfun-oled/oled_edison_unity
node app.js
