set dllpath=%1
set dllpath=%dllpath:~1,-1%

set name=%2
set name=%name:~1,-1%

copy "%dllpath%/%name%.dll" "Releases/%name%.dll"
copy "%dllpath%/%name%.xml" "Releases/%name%.xml"

bz c -storeroot:no "Releases/%name%.zip" "Releases"