@echo off
@title Building protobuf...
cd ../../assets/protoc-win64/bin
protoc -I=../../../Models/Node --csharp_out=../../../Models/Node ../../../Models/Node/*.proto
@exit
