# EPLAN API Tutorial

<p align="center">
    <a href="https://github.com/bingyongcao/eplan-api-tutorial/blob/main/README-cn.md">中文</a>
    |
    <a href="https://github.com/bingyongcao/eplan-api-tutorial/blob/main/README.md">English</a>
</p>

A collection of practical C# examples for extending and automating EPLAN Electric P8. The repository covers the four common EPLAN integration models: add-ins, scripts, remote clients, and standalone offline applications.

The examples currently target EPLAN Platform 2026.0.3. EPLAN API behavior and assembly compatibility are version-specific, so use assemblies from the EPLAN installation that the application will run against.

## Projects

| Project | Purpose | Target framework |
| --- | --- | --- |
| `EPLAN-ADDIN-TUTORIAL` | An `IEplAddIn` loaded inside EPLAN, with custom actions, ribbon commands, WPF/MVVM UI, project/page queries, and parts master-data access | .NET Framework 4.8.1 |
| `EPLAN-SCRIPT-TUTORIAL` | Source-level EPLAN scripts demonstrating actions, ribbon and context-menu customization, events, settings, and command-line parameters | .NET Framework 4.8.1 |
| `EPLAN-REMOTE` | A WPF remote client that connects to a running EPLAN instance and executes EPLAN actions | .NET 8 for Windows |
| `EPLAN_OFFLINE` | A standalone WPF application that initializes the EPLAN runtime, lets the user select a project, and displays its page count | .NET Framework 4.8.1, x64 |

## Repository layout

```text
eplan-api-tutorial/
|-- DLLs/                         Shared EPLAN 2026 API references
|-- EPLAN-ADDIN-TUTORIAL/         In-process add-in example
|   |-- Actions/                  Registered IEplAction implementations
|   |-- Models, ViewModels, Views WPF/MVVM project-properties example
|   `-- Utilities/                Reusable EPLAN query and UI helpers
|-- EPLAN-SCRIPT-TUTORIAL/        Individual EPLAN script examples
|-- EPLAN-REMOTE/                 gRPC-based remote client example
|-- EPLAN_OFFLINE/                Standalone offline API application
|-- EPLAN_API_TUTORIAL.slnx       Visual Studio solution
`-- LICENSE.txt                   MIT license
```

## Highlights

### Add-in

`SAC.EplAddIn.Tutorial.dll` implements `IEplAddIn` and creates an `EPLAN_ADDIN_TUTORIAL` ribbon tab containing four commands:

- `ProjInfo` displays the current project and opens a project-properties WPF window.
- `StructInfo` enumerates plant structure identifiers and their properties.
- `PageInfo` creates and filters pages, counts pages, and demonstrates function filtering.
- `MasterDataInfo` queries parts whose part number begins with `PSL`.

The `Utilities` folder contains helpers for selections, pages, functions, properties, settings, EPLAN windows, and ribbon cleanup.

### Scripts

The script examples demonstrate:

- Declaring EPLAN actions and adding ribbon commands.
- Adding context-menu entries that open project or macro directories.
- Showing context-menu identifiers.
- Handling `Eplan.EplApi.OnPostOpenProject`.
- Reading and changing EPLAN user settings.
- Passing parameters to a script executed from the command line.
- Removing a custom script ribbon tab.

See [`EPLAN-SCRIPT-TUTORIAL/README.md`](EPLAN-SCRIPT-TUTORIAL/README.md) for the per-script guide.

### Remote client

The remote example connects to `localhost:49152`, the configured default in this sample. Clicking its button executes the `XPartsManagementStart` action to open Parts Management in the connected EPLAN instance.

### Offline application

The offline application follows this flow:

1. Select an installed EPLAN version.
2. Bind the process to that installation with `AssemblyResolver.PinToEplan()`.
3. Click **Start** and select an `.elk`, `.ell`, `.elp`, or `.els` project file.
4. Initialize the EPLAN runtime and open the selected project inside a `LockingStep`.
5. Display `Project.Pages.Length`, then close the project cleanly.

## Prerequisites

- Windows with a compatible EPLAN Electric P8 / EPLAN Platform 2026 installation.
- Visual Studio 2022 with .NET desktop development tools.
- .NET Framework 4.8.1 Developer Pack.
- .NET 8 SDK for `EPLAN-REMOTE`.
- A valid EPLAN license for the API features being exercised.
- The EPLAN remote server/configuration enabled when running `EPLAN-REMOTE`.

The repository currently references EPLAN Platform 2026.0.3 under:

```text
D:\Eplan\Platform\2026.0.3\Bin
```

If EPLAN is installed elsewhere, update the `HintPath` entries and the add-in post-build destination before building.

## Build

Open `EPLAN_API_TUTORIAL.slnx` in Visual Studio, restore NuGet packages, select the appropriate project, and build it. The projects can also be built individually.

Build the offline application:

```powershell
dotnet build .\EPLAN_OFFLINE\EPLAN_OFFLINE.csproj -c Debug
```

Build the remote client:

```powershell
dotnet build .\EPLAN-REMOTE\EPLAN_REMOTE.csproj -c Debug
```

The add-in and script projects use the traditional .NET Framework project format and `packages.config`; Visual Studio/MSBuild is the most direct build path for them.

## Run

### Add-in

1. Update the EPLAN assembly paths and the `PostBuildEvent` in `EPLAN_ADDIN_TUTORIAL.csproj`.
2. Build the project.
3. Make `SAC.EplAddIn.Tutorial.dll` available to the matching EPLAN Platform installation.
4. Register/load the assembly through EPLAN's add-in management interface.
5. Use the new `EPLAN_ADDIN_TUTORIAL` ribbon tab.

The current Debug post-build event copies the add-in directly to `D:\Eplan\Platform\2026.0.3\Bin`.

### Scripts

Load the desired `.cs` file through EPLAN's script management interface. The source files are the deployable scripts; building the tutorial project is optional when the goal is simply to load one script into EPLAN.

For the command-line example, adapt the executable and script paths to the local installation:

```powershell
W3u.exe ExecuteScript /ScriptFile:"C:\Path\CallScriptByCLI.cs" /Param1:"Hello" /Param2:"EPLAN"
```

### Remote client

Start EPLAN with its remote endpoint available on port `49152`, then run `EPLAN_REMOTE.exe`. Change the host or port in `MainWindow.xaml.cs` when the endpoint differs.

### Offline application

Run `EPLAN_OFFLINE.exe`, select the EPLAN product version when prompted, click **Start**, and select an EPLAN project. The application opens the project, reports its page count, and closes it.

## Important compatibility notes

- EPLAN Platform 2026 API assemblies are built for .NET Framework 4.8.1. Keep `EPLAN_OFFLINE` on `net481`; targeting .NET 8 can fail inside the unmanaged portion of `EplApplication.Init()`.
- EPLAN API assemblies have unmanaged dependencies and must be loaded from the selected EPLAN Platform `Bin` directory.
- In the offline project, only `Eplan.EplApi.Starteru.dll` is copied locally. Other EPLAN API references use `Private=False` and are resolved after `PinToEplan()`.
- Run EPLAN API calls on the expected EPLAN/UI thread unless the API documentation explicitly states otherwise.
- Opening an offline application can consume an EPLAN license. A `MAX 40.43` error means the selected license was not available from the configured license system.
- Do not mix EPLAN assemblies from different Platform versions.

## Documentation

- [EPLAN Platform API 2026](https://www.eplan.help/en-us/Infoportal/Content/api/2026/index.html)
- [EPLAN API offline applications](https://www.eplan.help/en-us/infoportal/content/api/2026/UsingEplanAssemblies.html)
- [EPLAN scripting tutorial by Suplanus](https://eplan-scripting.suplanus.de/v4/en/)

## License

This repository is released under the [MIT License](LICENSE.txt).
