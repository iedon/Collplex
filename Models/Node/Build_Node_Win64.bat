@echo off
@title Building Node.cs...
cd ../../assets/protoc-win64/bin
protoc -I=../../../Models/Node --csharp_out=../../../Models/Node ../../../Models/Node/Node.proto
@exit
