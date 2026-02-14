@echo off
rar > nul 2>&1
if "%errorlevel%" == "9009" (
    echo WinRar not found... Trying 7-Zip
    goto :7zip
) else (
    winrar a -afzip PoW_English.zip Mod
    winrar a -afzip QoL.zip Qol_Mod
    goto :end
)

:7zip
7z > nul 2>&1
if "%errorlevel%" == "9009" (
    echo 7-Zip not found... Please install either WinRar or 7-Zip and add their binary locations to your environment path
) else (
    7z a -tzip PoW_English.zip Mod
    7z a -tzip QoL.zip Qol_Mod
    goto :end
)

:end
echo Done!
START https://github.com/magxm/PoW-Loader/releases/new