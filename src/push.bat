@echo off
echo Determing which files were added and removed...
hg addremove
echo.
echo Changes:
hg status
echo.
set /p message=Commit message: 
hg commit -m "%message%"
echo.
echo Pushing to server...
hg push
ping -n 2 127.0.0.1>nul