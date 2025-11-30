# TurnOffDisplay

Windows 系统托盘工具：显示器控制 + 热键管理 + 定时提醒

## 功能

| 功能 | 说明 |
|------|------|
| **显示器控制** | 托盘菜单一键关闭/开启显示器 |
| **热键管理** | Against the Storm 存档快捷键（Ctrl+1/2/3） |
| **提醒功能** | 读取桌面 ToDo.json，定时弹窗提醒 |

## 项目结构

```
TurnOffDisplay/
├── Program.cs           # 入口，托盘图标与菜单
├── DisplayManager.cs    # 显示器开关（Windows API）
├── HotkeyManager.cs     # 全局热键（Ctrl+1/2/3）
├── ReminderManager.cs   # ToDo.json 定时提醒
├── TurnOffDisplay.csproj
├── app.manifest
├── icon.ico
└── publish-standalone/  # 发布输出
```

## 开发

```powershell
# 运行
dotnet run

# 热重载
dotnet watch run
```

## 发布

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish-standalone
```

## 开机自启

快捷方式：`%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\TurnOffDisplay.lnk`

## 热键

| 热键 | 功能 | 条件 |
|------|------|------|
| `Ctrl+1` | 保存存档到 SL 文件夹 | Against the Storm 窗口激活时 |
| `Ctrl+2` | 从 SL 文件夹读取存档 | Against the Storm 窗口激活时 |
| `Ctrl+3` | 从备份恢复存档 | Against the Storm 窗口激活时 |

## ToDo.json 格式

```json
[{"TaskDescription": "开会", "ReminderTime": "2025-01-15T14:00:00"}]
```

## 技术栈

- .NET 8.0-windows（WinForms）
- Windows API P/Invoke
- System.Text.Json

## 相关项目

手机远程控制显示器：[LocalWebsite](../LocalWebsite)
