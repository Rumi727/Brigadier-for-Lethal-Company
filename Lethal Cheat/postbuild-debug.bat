set dllpath=%1
set dllpath=%dllpath:~1,-1%

set name=%2
set name=%name:~1,-1%

for /F "usebackq tokens=* delims=" %%x in ("Lethal Path.txt") do set "lethal=%%x"

:wait_for_kill
ping 127.0.0.1 -n 1 -w 5000 > NUL
tasklist | find /i "Lethal Company.exe" > nul && (taskkill /im "Lethal Company.exe" && goto wait_for_kill)

ping 127.0.0.1 -n 1 -w 5000 > NUL

copy "%dllpath%/%name%.dll" "%lethal%/BepInEx/plugins/%name%.dll"

powershell "Start-Process ""%lethal%/Lethal Company.exe"""