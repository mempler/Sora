@echo off
OpenCover.Console.exe -register:user -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:test -filter:"+[Sora*]* -[Sora.Tests*]*" -output:".\coverage.xml" -oldstyle
codecov -f coverage.xml
