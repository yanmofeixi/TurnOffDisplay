# TurnOffDisplay

一个 Windows 系统托盘程序，提供显示器控制、热键管理、麦克风音频接收和提醒功能。配合 [LocalWebsite](../LocalWebsite) 使用，可以通过手机远程控制电脑。

## 功能

| 功能 | 说明 |
|------|------|
| **显示器控制** | 通过 TCP 监听（端口 12345）接收命令，远程关闭/开启显示器 |
| **热键管理** | 游戏存档快速保存/读取（针对 Against the Storm） |
| **麦克风接收** | 通过 SignalR 接收远程音频并播放 |
| **提醒功能** | 读取桌面 ToDo.json 文件，定时弹出提醒 |

## 项目结构

```
TurnOffDisplay/
├── Program.cs              # 应用入口，托盘图标设置
├── DisplayManager.cs       # 显示器控制，TCP 服务器（端口 12345）
├── HotkeyManager.cs        # 全局热键管理（Ctrl+1/2/3）
├── MicrophoneManager.cs    # SignalR 音频接收和播放
├── ReminderManager.cs      # 定时提醒功能
├── TitleBarRemover.cs      # 窗口标题栏移除（未启用）
├── Utility.cs              # 工具函数
├── TurnOffDisplay.csproj   # 项目文件
├── app.manifest            # 应用程序清单
├── icon.ico                # 托盘图标
├── publish-standalone/     # 发布的独立可执行文件
└── README.md               # 本文件
```

## 与 LocalWebsite 的配合

```
手机浏览器
    ↓ 访问 http://192.168.0.100:5000
LocalWebsite (端口 5000)
    ↓ 点击"黑屏"按钮，发送 TCP "turn off" 到 127.0.0.1:12345
TurnOffDisplay (端口 12345)
    ↓ 调用 Windows API
显示器关闭
```

## 开发环境

### 前置条件

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- VS Code（推荐安装 C# 扩展）

### 本地运行

```powershell
# 在项目目录下执行
dotnet run
```

程序启动后会在系统托盘显示图标，右键图标可以看到菜单。

### 开发模式

```powershell
# 热重载模式
dotnet watch run
```

## 部署

### 发布为独立可执行文件

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish-standalone
```

这会在 `publish-standalone/` 目录生成一个约 159MB 的独立可执行文件 `TurnOffDisplay.exe`，无需安装 .NET 运行时即可运行。

### 开机自启动

快捷方式已配置在：
```
%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\TurnOffDisplay.lnk
```

或手动创建：
1. 按 `Win + R`，输入 `shell:startup`
2. 在打开的文件夹中创建 `TurnOffDisplay.exe` 的快捷方式

## 技术栈

- **框架**：.NET 8.0 Windows Forms
- **音频处理**：NAudio
- **实时通信**：SignalR Client
- **Windows API**：P/Invoke（显示器控制、热键注册）

## 热键功能说明

针对游戏 **Against the Storm** 的存档管理：

| 热键 | 功能 |
|------|------|
| `Ctrl+1` | 保存当前存档到 SL 文件夹 |
| `Ctrl+2` | 从 SL 文件夹读取存档 |
| `Ctrl+3` | 从备份文件夹恢复存档 |

> 热键仅在游戏窗口处于前台时生效

## 提醒功能

程序会读取桌面上的 `ToDo.json` 文件，格式如下：

```json
[
  {
    "TaskDescription": "开会",
    "ReminderTime": "2025-11-29T14:00:00"
  }
]
```

到达指定时间时会弹出提醒对话框。

## 注意事项

- 程序需要以**普通权限**运行，如果以管理员权限运行可能导致热键在普通程序中不生效
- 显示器控制功能通过 Windows API `SendMessage` 实现
- SignalR 连接使用自签名证书，已配置跳过证书验证
