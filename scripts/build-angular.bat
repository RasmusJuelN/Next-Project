@echo off

REM Navigate to the Angular source directory
cd angular-src

REM Install dependencies
call npm install

REM Check if the first argument is "prod"
IF "%~1"=="prod" (
    REM Build the Angular application for production
    call ng build --prod
) ELSE (
    REM Build the Angular application for development
    call ng build
)

REM Copy the built files and directories to the front-end directory
xcopy /E /I /Y dist\angular-src\* ..\front-end

REM Remove the original built files and directories
rd /S /Q dist\angular-src

REM Navigate back to the root directory
cd ..