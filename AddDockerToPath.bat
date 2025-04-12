@echo off
set "DOCKER_PATH=C:\Program Files\Docker\Docker\resources\bin"

:: Zjistí aktuální PATH pro uživatele
for /f "tokens=2* delims=    " %%a in ('reg query HKCU\Environment /v PATH 2^>nul') do set "USER_PATH=%%b"

:: Zkontroluj, zda už cesta existuje
echo %USER_PATH% | find /i "%DOCKER_PATH%" >nul
if %errorlevel%==0 (
    echo ✅ Docker path uz je v PATH.
) else (
    echo ➕ Pridavam Docker path do uzivatelske PATH...
    setx PATH "%USER_PATH%;%DOCKER_PATH%" >nul
    echo ✅ Hotovo. Zavri a znovu otevri terminal.
)

pause
