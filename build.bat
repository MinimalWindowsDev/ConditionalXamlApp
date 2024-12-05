@echo off
setlocal EnableDelayedExpansion

REM Initialize flags
set CLEAN=0
set RUN=0

REM Process command-line arguments
:process_args
if "%~1"=="" goto after_args
if /I "%~1"=="clean" set CLEAN=1
if /I "%~1"=="run" set RUN=1
shift
goto process_args

:after_args

REM If CLEAN is specified, perform cleaning
if %CLEAN%==1 (
    echo Cleaning bin and obj directories...
    for /d /r %%d in (bin,obj) do (
        if exist "%%d" (
            echo Deleting directory: "%%d"
            rd /s /q "%%d"
        )
    )
)

REM Build the solution using MSBuild
echo Building solution using MSBuild...
msbuild "ConditionalXamlApp.sln" /t:Rebuild /p:Configuration=Debug /verbosity:minimal /nologo /fileLogger

REM Check if build succeeded
if errorlevel 1 (
    echo Build failed.
    exit /b 1
) else (
    echo Build succeeded.
)

REM If RUN is specified, run the application
if %RUN%==1 (
    echo Running application...
    start "" "ConditionalXamlApp\bin\Debug\ConditionalXamlApp.exe"
)

endlocal
