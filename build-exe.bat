@echo off
REM ====================================================================
REM  Valorant Shop Checker - tek dosyalik .exe uretir (self-contained).
REM  Cift tiklayarak calistirabilirsiniz. .NET SDK 9+ gerektirir.
REM ====================================================================
setlocal
cd /d "%~dp0"

echo.
echo === Valorant Shop Checker derleniyor (tek .exe) ===
echo.

dotnet publish -c Release -r win-x64 --self-contained true

if errorlevel 1 (
    echo.
    echo HATA: Derleme basarisiz. .NET 9 SDK yuklu mu? ^(dotnet --version^)
    pause
    exit /b 1
)

echo.
echo === TAMAM ===
echo Olusan dosya:
echo   bin\Release\net9.0-windows\win-x64\publish\ValorantShopChecker.exe
echo.
echo Bu tek dosyayi istediginiz bilgisayara kopyalayip calistirabilirsiniz
echo (.NET kurulumu gerekmez).
echo.
pause
