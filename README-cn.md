# EPLAN API 教程

<p align="center">
    <a href="https://github.com/bingyongcao/eplan-api-tutorial/blob/main/README-cn.md">中文</a>
    |
    <a href="https://github.com/bingyongcao/eplan-api-tutorial/blob/main/README.md">English</a>
</p>

本仓库提供一组用于扩展和自动化 EPLAN Electric P8 的 C# 实例，覆盖四种常见的 EPLAN 集成方式：Add-in、脚本、Remote Client 和独立 Offline Application。

当前实例基于 EPLAN Platform 2026.0.3。EPLAN API 的行为及程序集兼容性与版本密切相关，项目引用的 API 程序集应与实际运行时使用的 EPLAN 安装版本一致。

## 项目说明

| 项目 | 用途 | 目标框架 |
| --- | --- | --- |
| `EPLAN-ADDIN-TUTORIAL` | 加载到 EPLAN 进程中的 `IEplAddIn`，包括自定义 Action、功能区按钮、WPF/MVVM 界面、项目/页面查询及部件主数据访问 | .NET Framework 4.8.1 |
| `EPLAN-SCRIPT-TUTORIAL` | EPLAN 源码脚本实例，包括 Action、功能区和右键菜单扩展、事件、设置以及命令行参数 | .NET Framework 4.8.1 |
| `EPLAN-REMOTE` | 连接正在运行的 EPLAN 实例并执行 EPLAN Action 的 WPF Remote Client | .NET 8 for Windows |
| `EPLAN_OFFLINE` | 独立 WPF 程序，初始化 EPLAN Runtime，让用户选择项目并显示项目页数 | .NET Framework 4.8.1 |

## 仓库结构

```text
eplan-api-tutorial/
|-- DLLs/                         公共 EPLAN 2026 API 引用
|-- EPLAN-ADDIN-TUTORIAL/         EPLAN 进程内 Add-in 实例
|   |-- Actions/                  已注册的 IEplAction 实现
|   |-- Models, ViewModels, Views WPF/MVVM 项目属性实例
|   `-- Utilities/                可复用的 EPLAN 查询及 UI 工具
|-- EPLAN-SCRIPT-TUTORIAL/        独立 EPLAN 脚本实例
|-- EPLAN-REMOTE/                 基于 gRPC 的 Remote Client
|-- EPLAN_OFFLINE/                独立 Offline API 程序
|-- EPLAN_API_TUTORIAL.slnx       Visual Studio 解决方案
`-- LICENSE.txt                   MIT 许可证
```

## 主要内容

### Add-in

`SAC.EplAddIn.Tutorial.dll` 实现了 `IEplAddIn`，注册后会创建名为 `EPLAN_ADDIN_TUTORIAL` 的功能区选项卡，其中包含四个命令：

- `ProjInfo`：显示当前项目，并打开项目属性 WPF 窗口。
- `StructInfo`：读取工厂结构标识符及其属性。
- `PageInfo`：创建和筛选页面、统计页数并演示功能筛选。
- `MasterDataInfo`：查询部件编号以 `PSL` 开头的主数据部件。

`Utilities` 文件夹提供选择集、页面、功能、属性、设置、EPLAN 窗口和功能区清理等辅助方法。

### 脚本

脚本实例包含：

- 声明 EPLAN Action，并向功能区添加命令。
- 添加用于打开项目目录或宏目录的右键菜单项。
- 显示右键菜单标识信息。
- 处理 `Eplan.EplApi.OnPostOpenProject` 事件。
- 读取和修改 EPLAN 用户设置。
- 从命令行执行脚本并传入参数。
- 删除自定义脚本功能区选项卡。

每个脚本的具体说明请参阅 [`EPLAN-SCRIPT-TUTORIAL/README.md`](EPLAN-SCRIPT-TUTORIAL/README.md)。

### Remote Client

Remote 实例连接到 `localhost:49152`，这是当前代码配置的默认地址。点击按钮后会执行 `XPartsManagementStart` Action，从而在已连接的 EPLAN 实例中打开部件管理。

### Offline Application

Offline 程序的运行流程如下：

1. 选择本机安装的 EPLAN 版本。
2. 使用 `AssemblyResolver.PinToEplan()` 将当前进程绑定到该安装版本。
3. 点击 **Start**，选择 `.elk`、`.ell`、`.elp` 或 `.els` 项目文件。
4. 初始化 EPLAN Runtime，并在 `LockingStep` 中打开所选项目。
5. 显示 `Project.Pages.Length`，然后正确关闭项目。

## 环境要求

- Windows，以及兼容的 EPLAN Electric P8 / EPLAN Platform 2026。
- Visual Studio 2022，并安装 .NET 桌面开发组件。
- .NET Framework 4.8.1 Developer Pack。
- 用于构建 `EPLAN-REMOTE` 的 .NET 8 SDK。
- 当前实例所用 API 功能对应的有效 EPLAN 许可证。
- 运行 `EPLAN-REMOTE` 时，需要启用并配置 EPLAN Remote 服务。

当前仓库中的部分项目将 EPLAN Platform 路径配置为：

```text
D:\Eplan\Platform\2026.0.3\Bin
```

如果 EPLAN 安装在其他位置，请在编译前修改项目中的 `HintPath`，以及 Add-in 项目的生成后复制路径。

## 编译

使用 Visual Studio 打开 `EPLAN_API_TUTORIAL.slnx`，还原 NuGet 包，然后选择对应项目进行编译。也可以分别编译各个项目。

编译 Offline Application：

```powershell
dotnet build .\EPLAN_OFFLINE\EPLAN_OFFLINE.csproj -c Debug
```

编译 Remote Client：

```powershell
dotnet build .\EPLAN-REMOTE\EPLAN_REMOTE.csproj -c Debug
```

Add-in 和脚本项目使用传统 .NET Framework 项目格式及 `packages.config`，建议直接使用 Visual Studio/MSBuild 编译。

## 运行

### Add-in

1. 修改 `EPLAN_ADDIN_TUTORIAL.csproj` 中的 EPLAN 程序集路径和 `PostBuildEvent`。
2. 编译项目。
3. 将 `SAC.EplAddIn.Tutorial.dll` 放到匹配的 EPLAN Platform 环境中。
4. 通过 EPLAN 的 Add-in 管理界面注册并加载程序集。
5. 使用新增的 `EPLAN_ADDIN_TUTORIAL` 功能区选项卡。

当前 Debug 生成后事件会将 Add-in 直接复制到 `D:\Eplan\Platform\2026.0.3\Bin`。

### 脚本

通过 EPLAN 脚本管理界面加载需要的 `.cs` 文件。源码文件本身就是可以加载的脚本；如果只是希望在 EPLAN 中运行某个脚本，不一定需要先编译整个教程项目。

命令行脚本实例需要根据本机环境修改可执行文件和脚本路径：

```powershell
W3u.exe ExecuteScript /ScriptFile:"C:\Path\CallScriptByCLI.cs" /Param1:"Hello" /Param2:"EPLAN"
```

### Remote Client

先启动 EPLAN，并保证 Remote Endpoint 可通过 `49152` 端口访问，然后运行 `EPLAN_REMOTE.exe`。如果服务地址不同，请修改 `MainWindow.xaml.cs` 中的主机名和端口。

### Offline Application

运行 `EPLAN_OFFLINE.exe`，在提示时选择 EPLAN 产品版本，然后点击 **Start** 并选择一个 EPLAN 项目。程序会打开项目、显示页数并关闭项目。

## 重要兼容性说明

- EPLAN Platform 2026 API 程序集基于 .NET Framework 4.8.1。`EPLAN_OFFLINE` 应保持使用 `net481`；使用 .NET 8 可能在 `EplApplication.Init()` 的非托管初始化阶段失败。
- EPLAN API 程序集依赖非托管组件，必须从所选 EPLAN Platform 的 `Bin` 目录加载。
- Offline 项目只在输出目录中复制 `Eplan.EplApi.Starteru.dll`。其他 EPLAN API 引用使用 `Private=False`，并在 `PinToEplan()` 后解析。
- 除非 API 文档明确说明可以使用其他线程，否则应在预期的 EPLAN/UI 线程中执行 EPLAN API 调用。
- 启动 Offline Application 可能占用一个 EPLAN 许可证。`MAX 40.43` 表示许可证系统中没有可用的所选许可证。
- 不要混用不同 EPLAN Platform 版本的 API 程序集。

## 参考资料

- [EPLAN Platform API 2026](https://www.eplan.help/en-us/Infoportal/Content/api/2026/index.html)
- [EPLAN API Offline Application](https://www.eplan.help/en-us/infoportal/content/api/2026/UsingEplanAssemblies.html)
- [Suplanus EPLAN 脚本教程](https://eplan-scripting.suplanus.de/v4/en/)

## 许可证

本仓库使用 [MIT License](LICENSE.txt)。
