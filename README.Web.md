# DigitalTwin project important notes

## Prerequisites

- (Optional - Recommended) Visual Studio 2022
- (Required without VS2022) [Download](https://dotnet.microsoft.com/download/dotnet/6.0) latest dotnet SDK release.
- Install wasm-tools (root terminal): `dotnet workload install wasm-tools --skip-manifest-update`
- Deactivate bullet on base project: `DigitalTwin/MyScene.cs:L15`,`DigitalTwin/DigitalTwin.csproj:L16`

## Build

Use VS2022 or VSCode/Terminal. You can build and test only the client project (Web), the server is only needed for publishing with compression (see below).

`dotnet build -c [Debug|Release] ./DigitalTwin.Web/DigitalTwin.Web.[Server.]csproj`

## Run

From VS2022 you can run the profile `DigitalTwin.Web[.Server]`. There is also an IIS Express profile for each cliente and server projects, but it is usually slower.

Additionally you can publish the app

`dotnet publish -c [Debug|Release] ./DigitalTwin.Web/DigitalTwin.Web.csproj`

and run the app by populating the folder `./DigitalTwin.Web/bin/[Debug|Release]/net6.0/publish/wwwroot`. In this second case we do recommend to use VSCode Live Server, instead of Fenix, as the second has known issues with Web Assembly.

### Debug

Debug is in an experimental phase and currently some workarounds are needed to make it work.

1. Install the latests rc2 night from <https://github.com/dotnet/installer#installers-and-binaries>.
1. Go to `C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.MonoTargets.Sdk` and copy the folder `6.0.0-rc2.X.X/tasks/net472` to `6.0.0-rtm.X/tasks/net472`
1. Modify _Evergine_ references to 6.0.0-rc1.X.X to your 6.0.0-rc2.X.X, and generate local nugets.
1. Modify in your project all references to 6.0.0-rc1.X.X to your 6.0.0-rc2.X.X, and point _Evergine_ references to your local nugets.
1. Finally clean and rebuild the projects from VS2022. After that you will be able to put a break point and debug your Wasm .Net6 app.

## Publish with Compression

To publish the application with automatic compression (Brotli & GZip), the AspNetCore server is needed. Use VS2022 or from Terminal:

`dotnet publish -c Release -r win-x86 --self-contained ./DigitalTwin.Web.Server/DigitalTwin.Web.Server.csproj`

If done from terminal, you can publish the files in `DigitalTwin.Web.Server/bin/Release/net6.0/win-x86/publish` to an AspNetCore server, from VSCode or manually.

## Known issues

- PNG textures produce unexpected behaviour due to a bug in ImageSharp, please use jpg textures.
- Bullet not working, need to be deactivated in base project.
