@echo off
    rem Compiles all .cs files in current directory into single exe and runs it.
    rem Yeah, I know its bad practice to do this, but it works for my purposes.
@echo on

cls
call csc -define:DEBUG -optimize -out:application.exe *.cs

@echo off
    set BUILD_STATUS=%ERRORLEVEL%
    timeout /t 1
    if %BUILD_STATUS% == 0 call .\application.exe
    if not %BUILD_STATUS% == 0 echo Build Failed
@echo on