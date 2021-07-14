@echo off
cls
echo =========================================================================================================
echo This will build the BriefingRoom library, command-line tool and Desktop webapp to the Release\ directory.
echo.
echo You'll need the following SDKs for the build to complete successfully:
echo - Microsoft Edge WebView2: https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section
echo - .NET 6.0 x64 SDK: https://dotnet.microsoft.com/download/dotnet/6.0
echo =========================================================================================================
echo.
pause
md bin
md Release\bin
copy BriefingRoomDesktop.bat Release\
copy Default.brt Release\
copy Default.cbrt Release\
xcopy Database\ Release\Database\ /E /Y
xcopy Include\ Release\Include\ /E /Y
xcopy Media\ Release\Media\ /E /Y
IF ERRORLEVEL 1 GOTO Error
cd Source\BriefingRoomCommandLine
dotnet build -c Release
IF ERRORLEVEL 1 GOTO Error
cd..
cd BriefingRoomDesktop
xcopy Resources\ ..\..\Release\Resources\ /E /Y
xcopy wwwroot\ ..\..\Release\wwwroot\ /E /Y
IF ERRORLEVEL 1 GOTO Error
dotnet build -c Release
IF ERRORLEVEL 1 GOTO Error
xcopy bin\Release\net6.0-windows\win-x64\ ..\..\Release\bin\ /E /Y
IF ERRORLEVEL 1 GOTO Error
cd..
cd..
cd Release
copy BriefingRoom.dll bin\
if exist *.config del *.config
echo.
echo =============================
echo Build completed successfully.
echo =============================
goto End

:Error
echo.
echo ================================
echo An error occured. Build aborted.
echo ================================
goto End

:End
echo.
pause
