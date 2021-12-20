# Code Setup
The code should be easily build by the following command
	dotnet build

# Tests
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=D:\TestResults\

# Code Coverage Report
	reportgenerator -reports:"D:\TestResults\coverage.cobertura.xml" -targetdir:"D:\TestResults\" -reporttypes:Html 
	