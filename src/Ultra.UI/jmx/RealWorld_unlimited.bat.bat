@echo off 
set rampup=150
set duration=1800

echo rampup is %rampup%
echo duration is %duration%
START /B CMD /C CALL "D:\LoadTesting\apache-jmeter-2.9\bin\jmeter.bat" -Jhostname=uat.shopyourway.com -Joutputfile=output\mix_unlimited.csv -Jrampup=%rampup% -Jduration=%duration% -n -t "D:\LoadTesting\manual\scripts\Corsica\RealWorld_unlimited.jmx"
