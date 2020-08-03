@echo off

echo Merging AssetBundle files
IF EXIST "AssetBundles" (
  mkdir AssetBundles_combined
  for /d %%i in (AssetBundles\*) do ( xcopy "%%i" "AssetBundles_combined\" /S /I /E /Y ) 
) ELSE (
  echo No AssetBundles folder found!
)

PAUSE