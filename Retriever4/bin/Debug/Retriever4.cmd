@echo off
mode con cols=165 lines=255
powershell -WindowStyle Maximized -Mta -ExecutionPolicy Unrestricted -File RunRetriever.ps1
exit