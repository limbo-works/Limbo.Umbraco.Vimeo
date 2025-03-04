@echo off
dotnet build src/Limbo.Umbraco.Vimeo --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:\nuget\Umbraco13