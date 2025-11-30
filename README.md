# TurnOffDisplay

一个 Windows 系统托盘程序，提供显示器控制、热键管理和提醒功能。

## 功能

| 功能 | 说明 |
|------|------|
| **显示器控制** | 通过托盘菜单关闭/开启显示器 |
| **热键管理** | 游戏存档快速保存/读取（针对 Against the Storm） |
| **提醒功能** | 读取桌面 ToDo.json 文件，定时弹出提醒 |

## 项目结构

```
TurnOffDisplay/
 Program.cs              # 应用入口，托盘图标设置
 DisplayManager.cs       # 显示器控制
 HotkeyManager.cs        # 全局热键管理（Ctrl+1/2/3）
 ReminderManager.cs      # 定时提醒功能
 TitleBarRemover.cs      # 窗口标题栏移除（未启用）
 Utility.cs              # 工具函数
 TurnOffDisplay.csproj   # 项目文件
 app.manifest            # 应用程序清单
 icon.ico                # 托盘图标
 publish-standalone/     # 发布的独立可执行文件
 README.md
```

## 开发环境

### 前置条件

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- VS Code（推荐安装 C# 扩展）

### 本地运行

```powershell
dotnet run
```

程序启动后在系统托盘显示图标，右键可看到菜单。

### 开发模式

```powershell
dotnet watch run
```

## 部署

### 发布为独立可执行文件

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish-standalone
```

### 开机自启动

快捷方式位置：`%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\TurnOffDisplay.lnk`

## 技术栈

- .NET 8.0 Windows Forms
- Windows API（P/Invoke）
- Newtonsoft.Json

## 热键功能

针对游戏 **Against the Storm** 的存档管理：

| 热键 | 功能 |
|------|------|
| `Ctrl+1` | 保存当前存档到 SL 文件夹 |
| `Ctrl+2` | 从 SL 文件夹读取存档 |
| `Ctrl+3` | 从备份文件夹恢复存档 |

> 热键仅在游戏窗口处于前台时生效

## 提醒功能

读取桌面 `ToDo.json` 文件：

```json
[
  {
    "TaskDescription": "开会",
    "ReminderTime": "2025-11-29T14:00:00"
  }
]
```

## 显示器控制

```csharp
SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
```

## 注意事项

- 以普通权限运行，管理员权限可能导致热键失效
- 如需手机远程控制显示器，请使用 [LocalWebsite](../LocalWebsite)
