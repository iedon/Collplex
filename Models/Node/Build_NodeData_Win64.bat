@echo off
@title Building NodeData.cs...
cd ../../assets/protoc-win64/bin
protoc -I=../../../Models/Node --csharp_out=../../../Models/Node ../../../Models/Node/NodeData.proto
@exit
