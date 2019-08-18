@echo off
@title Building Client.cs...
cd ../assets/protoc-win64/bin
protoc -I=../../../Models --csharp_out=../../../Models ../../../Models/Client.proto
@exit
