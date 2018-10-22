@echo off 

set srcPath=%cd%\

set distCsPath=%srcPath%..\Assets\Scripts\Network\ProtoMsg

set binPath=%srcPath%\bin

::for %%i in (*.proto) do ( %binPath%\protoGen -i:%%i -o:%distCsPath%\%%~ni.cs  )

%binPath%\protoGen -i:%srcPath%\gateway.proto -o:%distCsPath%\gateway.cs 
%binPath%\protoGen -i:%srcPath%\player.proto -o:%distCsPath%\player.cs 
%binPath%\protoGen -i:%srcPath%\account.proto -o:%distCsPath%\account.cs 
%binPath%\protoGen -i:%srcPath%\team.proto -o:%distCsPath%\team.cs 

start %distCsPath%
echo "ok"

pause