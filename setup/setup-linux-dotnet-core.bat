::@echo off

set version=4.5
set dst=wexflow
set zip=wexflow-%version%-linux-dotnet-core.zip
set dstDir=.\%dst%
set backend=Backend

if exist %zip% del %zip%
if exist %dstDir% rmdir /s /q %dstDir%
mkdir %dstDir%
mkdir %dstDir%\Wexflow\
mkdir %dstDir%\Wexflow\Database\
mkdir %dstDir%\WexflowTesting\
mkdir %dstDir%\%backend%\
mkdir %dstDir%\%backend%\images\
mkdir %dstDir%\%backend%\css\
mkdir %dstDir%\%backend%\css\images\
mkdir %dstDir%\%backend%\js\
mkdir %dstDir%\Wexflow.Scripts.MongoDB
mkdir %dstDir%\Wexflow.Scripts.MongoDB\Workflows

:: WexflowTesting
xcopy ..\samples\WexflowTesting\* %dstDir%\WexflowTesting\ /s /e
xcopy ..\samples\dotnet-core\linux\WexflowTesting\* %dstDir%\WexflowTesting\ /s /e

:: Wexflow
xcopy ..\samples\dotnet-core\linux\Wexflow\* %dstDir%\Wexflow\ /s /e
copy ..\src\dotnet-core\Wexflow.Core\Workflow.xsd %dstDir%\Wexflow\

:: Wexflow backend
copy "..\src\backend\Wexflow.Backend\wwwroot\index.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\forgot-password.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\dashboard.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\manager.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\designer.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\approval.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\history.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\users.html" %dstDir%\%backend%\
copy "..\src\backend\Wexflow.Backend\wwwroot\profiles.html" %dstDir%\%backend%\

xcopy "..\src\backend\Wexflow.Backend\wwwroot\images\*" %dstDir%\%backend%\images\ /s /e

xcopy "..\src\backend\Wexflow.Backend\wwwroot\css\images\*" %dstDir%\%backend%\css\images\ /s /e
copy "..\src\backend\Wexflow.Backend\wwwroot\css\login.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\forgot-password.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\dashboard.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\manager.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\designer.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\approval.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\history.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\users.min.css" %dstDir%\%backend%\css
copy "..\src\backend\Wexflow.Backend\wwwroot\css\profiles.min.css" %dstDir%\%backend%\css

copy "..\src\backend\Wexflow.Backend\wwwroot\js\settings.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\login.min.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\forgot-password.min.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\dashboard.min.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\manager.min.js" %dstDir%\%backend%\js

copy "..\src\backend\Wexflow.Backend\wwwroot\js\ace.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\worker-xml.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\mode-xml.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\ext-searchbox.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\ext-prompt.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\ext-keybinding_menu.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\ext-settings_menu.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\theme-*.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\designer.min.js" %dstDir%\%backend%\js
::copy "..\src\backend\Wexflow.Backend\wwwroot\js\common.js" %dstDir%\%backend%\js
::copy "..\src\backend\Wexflow.Backend\wwwroot\js\authenticate.js" %dstDir%\%backend%\js
::copy "..\src\backend\Wexflow.Backend\wwwroot\js\cytoscape-dagre.min.js" %dstDir%\%backend%\js
::copy "..\src\backend\Wexflow.Backend\wwwroot\js\highlight.pack.js" %dstDir%\%backend%\js
::copy "..\src\backend\Wexflow.Backend\wwwroot\js\designer.js" %dstDir%\%backend%\js

copy "..\src\backend\Wexflow.Backend\wwwroot\js\approval.min.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\history.min.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\users.min.js" %dstDir%\%backend%\js
copy "..\src\backend\Wexflow.Backend\wwwroot\js\profiles.min.js" %dstDir%\%backend%\js

:: Wexflow server
dotnet publish ..\src\dotnet-core\Wexflow.Server\Wexflow.Server.csproj --force --output %~dp0\%dstDir%\Wexflow.Server
copy dotnet-core\linux\appsettings.json %dstDir%\Wexflow.Server

:: MongoDB script
dotnet publish ..\src\dotnet-core\Wexflow.Scripts.MongoDB\Wexflow.Scripts.MongoDB.csproj --force --output %~dp0\%dstDir%\Wexflow.Scripts.MongoDB
copy dotnet-core\linux\MongoDB\appsettings.json %dstDir%\Wexflow.Scripts.MongoDB
xcopy "..\samples\MongoDB\dotnet-core\linux\*" %dstDir%\Wexflow.Scripts.MongoDB\Workflows /s /e

:: License
:: copy ..\LICENSE.txt %dstDir%

:: compress
7z.exe a -tzip %zip% %dstDir%

:: Cleanup
rmdir /s /q %dstDir%

pause