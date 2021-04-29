#!/usr/bin/env bash
rm -rf .publish **/bin **/obj
dotnet test --collect:"XPlat Code Coverage"
dotnet restore -r ubuntu.20.04-x64
dotnet build sln/Collector -c Release -r ubuntu.20.04-x64
dotnet publish sln/Collector -o .publish -c Release -r ubuntu.20.04-x64