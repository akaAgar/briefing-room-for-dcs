@echo off

rem -----------------------------------------------------------------------
rem - Packs the built release into a neat .ZIP file ready to be uploaded. -
rem - Requires the 7z command-line tool to be in the PATH to work.        -
rem -----------------------------------------------------------------------

echo DON'T FORGET TO:
echo 1. Clean build a release version of BriefingRoom.exe
echo 2. Check the assembly version
echo 3. Update README.md and website with latest assembly version
echo 4. Export README and manuals markdown files to HTML (with Github theme)
echo.
pause
@7z a -tzip Release.zip @PackRelease.filelist
echo.
pause