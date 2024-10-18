@echo off
start ssh -R HumanAddiction:80:localhost:5000 serveo.net &
start "" HumanAddictionServer.exe
