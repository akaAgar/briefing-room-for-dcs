@echo off
cls
echo [33m
echo =========================================================================================================
echo This will build the BriefingRoom library, command-line tool and Desktop webapp to the Release\ directory.
echo.
echo You'll need the following SDKs for the build to complete successfully:
echo - Microsoft Edge WebView2: https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section
echo - .NET 6.0 x64 SDK: https://dotnet.microsoft.com/download/dotnet/6.0
echo =========================================================================================================
echo [0m
echo.
pause
md Release
md Release\Bin
copy Default.brt Release\
copy Default.cbrt Release\
copy LICENSE Release\
copy *.html Release\
copy *.md Release\
xcopy Database\ Release\Database\ /E /Y
xcopy docs\ Release\Docs\ /E /Y
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
xcopy bin\Release\net6.0-windows\win-x64\ ..\..\Release\Bin\ /E /Y
IF ERRORLEVEL 1 GOTO Error
cd..
cd..
cd Release
copy BriefingRoom.dll Bin\
if exist *.config del *.config
echo.
echo [32m
echo =============================
echo Build completed successfully.
echo =============================
echo [0m
goto End

:Error
echo.
echo [31m
echo ================================
echo An error occured. Build aborted.
echo ================================
echo [0m
goto End

:End
echo.
pause
