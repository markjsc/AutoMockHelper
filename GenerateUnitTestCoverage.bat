@echo off

SET dotnet="C:\Program Files\dotnet\dotnet.exe"
SET opencover="%userprofile%\.nuget\packages\OpenCover\4.6.519\tools\OpenCover.Console.exe"
SET reportgenerator="%userprofile%\.nuget\packages\reportgenerator\4.0.0-rc4\tools\net47\ReportGenerator.exe"

SET targetargs="test"
SET filter=""
SET coveragedir=".coverage"

%opencover% ^
-register:user ^
-target:%dotnet% ^
-targetargs:%targetargs% ^
-filter:"+[AutoMockHelper.SampleLogic*]*" ^
-mergebyhash ^
-skipautoprops ^
-output:"%~dp0.coverage\AutoMockHelper.Samples.MSTest.xml"

%reportgenerator% -targetdir:%coveragedir% -reporttypes:Html;Badges -reports:"AutoMockHelper.Samples.MSTest.xml" -verbosity:Warning

REM "%userprofile%\.nuget\packages\OpenCover\4.6.519\tools\OpenCover.Console.exe" ^
REM -register:user ^
REM -target:"C:\Program Files\dotnet\dotnet.exe" ^
REM -targetargs:"test" ^
REM -filter:"+[AutoMockHelper.SampleLogic*]*" ^
REM -mergebyhash ^
REM -skipautoprops ^
REM -output:"%~dp0.GeneratedReports\AutoMockHelper.Samples.MSTest.xml"