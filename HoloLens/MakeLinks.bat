@ECHO OFF

ECHO.
ECHO ===============================================================================
ECHO =                                 Make Links                                  =
ECHO ===============================================================================
ECHO.
ECHO This batch file creates symbolic links for the MRTK and other Unity 
ECHO source-based libraries that are used by this project. 
ECHO.
ECHO The libraries used by this project are:
ECHO.
ECHO * Mixed Reality Toolkit (MRTK) for Unity
ECHO.
ECHO All libraries should be downloaded and extracted before running this batch file. 
ECHO If you continue you will be prompted for the full path of each of the above 
ECHO libraries. 
ECHO.
ECHO Are you ready to continue?
ECHO.
CHOICE /C:YN
IF ERRORLEVEL == 2 GOTO End


:MRTK

SET /p MRTKSource=MRTK for Unity Path? 
IF NOT EXIST "%MRTKSource%\Assets\MRTK\" (
ECHO.
ECHO MRTK for Unity not found at %MRTKSource%
ECHO.
GOTO MRTK
)
ECHO MRTK for Unity FOUND
ECHO.

ECHO.
ECHO ===============================================================================
ECHO =                            Copying MRTK RSPs                                =
ECHO ===============================================================================
ECHO.
XCOPY /Y /Q "%MRTKSource%\Assets\*.rsp" "EnviroLight\Unity\EnviroLight\Assets"
XCOPY /Y /Q "%MRTKSource%\Assets\*.rsp" "LearnBinary\Unity\LearnBinary\Assets"
XCOPY /Y /Q "%MRTKSource%\Assets\*.rsp" "VoiceMemo\Unity\VoiceMemo\Assets"
XCOPY /Y /Q "%MRTKSource%\Assets\*.rsp.meta" "EnviroLight\Unity\EnviroLight\Assets"
XCOPY /Y /Q "%MRTKSource%\Assets\*.rsp.meta" "LearnBinary\Unity\LearnBinary\Assets"
XCOPY /Y /Q "%MRTKSource%\Assets\*.rsp.meta" "VoiceMemo\Unity\VoiceMemo\Assets"
ECHO.

ECHO.
ECHO ===============================================================================
ECHO =                               Linking MRTK                                  =
ECHO ===============================================================================
ECHO.
mklink /J "EnviroLight\Unity\EnviroLight\Assets\MRTK" "%MRTKSource%\Assets\MRTK"
mklink /J "LearnBinary\Unity\LearnBinary\Assets\MRTK" "%MRTKSource%\Assets\MRTK"
mklink /J "VoiceMemo\Unity\VoiceMemo\Assets\MRTK" "%MRTKSource%\Assets\MRTK"
ECHO.

:End