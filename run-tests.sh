#!/bin/sh

xbuild /p:Configuration=Release /verbosity:minimal PsdSharp.sln
mono ./packages/xunit.runner.console.*/tools/xunit.console.exe ./tests/PsdSharp.Tests/bin/Release/PsdSharp.Tests.dll
