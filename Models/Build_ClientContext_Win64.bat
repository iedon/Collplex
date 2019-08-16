@echo off
@title Building ClientContext.cs...
cd ../assets/protoc-win64/bin
protoc -I=../../../Models --csharp_out=../../../Models ../../../Models/ClientContext.proto
@exit
