set dllpath=%1
set dllpath=%dllpath:~1,-1%

set name=%2
set name=%name:~1,-1%

copy "%dllpath%/%name%.dll" "Releases/%name%.dll"
copy "%dllpath%/%name%.xml" "Releases/%name%.xml"
copy "README.md" "Releases/README.md"
:: copy "%dllpath%/Brigadier.NET.dll" "Releases/Brigadier.NET.dll"
:: copy "%dllpath%/System.Buffers.dll" "Releases/System.Buffers.dll"
:: copy "%dllpath%/System.Memory.dll" "Releases/System.Memory.dll"
:: copy "%dllpath%/System.Numerics.Vectors.dll" "Releases/System.Numerics.Vectors.dll"

bz c -storeroot:no "Releases/%name%.zip" "Releases"