@echo off
for %%i in (*.proto) do (
    protoc --csharp_out=. --grpc_out=. --plugin=protoc-gen-grpc=grpc_csharp_plugin.exe %%i 
    rem ignore
    echo From %%i To %%~ni.cs Successfully!  
)
pause