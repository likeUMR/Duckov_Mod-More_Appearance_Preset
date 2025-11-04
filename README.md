# More Appearance Preset - 更多预设皮肤

[English](#english) | [中文](#中文)

---

## 中文

《逃离鸭科夫》(Escape from Duckov) 的一个Mod，为捏脸界面添加预设皮肤快速选择功能。

### ✨ 功能特性

- 🎭 **快速应用预设** - 一键应用预先配置的角色造型
- 🎲 **随机生成** - 生成随机外观，带彩虹色特效
- 🎨 **自定义扩展** - 轻松添加你自己的预设皮肤
- 🖱️ **流畅交互** - 点击即用，无需手动输入代码
- 🎯 **非侵入式** - 利用游戏原生功能，兼容性好

### 📸 预览

进入捏脸界面后，点击"我是什么？"按钮即可打开预设面板：

```
┌─────────────────────┐
│  我是什么？(彩色)   │  ← 点击这里
└─────────────────────┘
         ↓
┌─────────────────────┐
│      预设面板        │
├─────────────────────┤
│  ??? (彩虹色)       │  ← 随机预设
│  我是小黄鸭          │
│  我是坤坤鸭          │
│  我是假面骑士        │
│  ...更多预设...     │
└─────────────────────┘
```

### 🎮 使用方法

1. 下载最新的 [Release](../../releases)
2. 将 `MoreAppearancePreset` 文件夹复制到游戏Mod目录：
   ```
   游戏目录/Mods/MoreAppearancePreset/
   ```
3. 启动游戏，在Mod管理器中启用此Mod
4. 进入捏脸界面，点击"我是什么？"按钮

### 🛠️ 添加自己的预设

#### 方法一：直接修改代码（简单）

1. 在游戏中捏出喜欢的造型
2. 点击游戏的"复制"按钮（预设JSON已复制）
3. 打开 `PresetData.cs`，在字典中添加：
   ```csharp
   { "你的预设名", "粘贴JSON" },
   ```
4. 重新编译：运行 `build.bat`

#### 方法二：使用Python工具（推荐）

1. 将预设JSON保存为 `models及其来源表/models/预设名.json`
2. 编辑 `models及其来源表/来源表.csv`，添加一行
3. 运行 `python generate_preset_data.py`
4. 复制生成的代码到 `PresetData.cs`
5. 重新编译

### 🔧 开发环境

**要求：**
- .NET SDK (支持 .NET Standard 2.1)
- C# 编译器 (Visual Studio / Rider / VSCode)
- 《逃离鸭科夫》游戏本体

**编译：**
1. 编辑 `MoreAppearancePreset.csproj`，设置你的游戏路径：
   ```xml
   <DuckovPath>你的游戏安装路径</DuckovPath>
   ```
2. 运行 `build.bat` (Windows) 或 `dotnet build -c Debug`
3. 编译产物在 `builded/` 目录

### 📦 项目结构

```
MoreAppearancePreset/
├── models及其来源表/          # 预设管理
│   ├── models/               # JSON预设文件
│   ├── 来源表.csv            # 预设列表
│   └── generate_preset_data.py  # 生成工具
├── builded/                  # 编译输出（发布版本）
├── ModBehaviour.cs           # Mod主入口
├── PresetData.cs             # 预设数据存储
├── YellowDuckHandler.cs      # 核心处理逻辑
└── *.cs                      # 其他功能模块
```

### 💡 代码说明

**核心模块**：
- `ModBehaviour.cs` - Mod主入口，检测捏脸场景并初始化
- `PresetData.cs` - 存储所有预设数据的字典
- `PresetCopier.cs` - 复制游戏UI并创建预设面板
- `YellowDuckHandler.cs` - 处理预设按钮的创建和配置
- `ButtonHandler.cs` - 管理按钮点击事件
- `PresetViewManager.cs` - 控制预设面板的显示/隐藏
- `RandomPresetGenerator.cs` - 生成随机预设
- `UIFinder.cs` - Unity UI查找工具
- `TextLocalizerRemover.cs` - 移除游戏本地化组件
- `RainbowColorEffect.cs` - 彩虹色特效

### 🤝 贡献

欢迎提交Issue和Pull Request！

如果你创建了有趣的预设，也欢迎分享！

### 📄 许可证

[MIT License](LICENSE)

### 🙏 致谢

感谢《逃离鸭科夫》游戏开发团队提供的Mod支持。

---

## English

A mod for *Escape from Duckov* that adds preset appearance selection to the character customization interface.

### ✨ Features

- 🎭 **Quick Apply Presets** - One-click to apply pre-configured character appearances
- 🎲 **Random Generator** - Generate random appearance with rainbow color effect
- 🎨 **Custom Extensions** - Easily add your own preset skins
- 🖱️ **Smooth Interaction** - Click to use, no manual code input needed
- 🎯 **Non-intrusive** - Uses native game features for better compatibility

### 🎮 How to Use

1. Download the latest [Release](../../releases)
2. Copy `MoreAppearancePreset` folder to game Mod directory:
   ```
   Game_Directory/Mods/MoreAppearancePreset/
   ```
3. Launch game and enable this mod in Mod Manager
4. Enter character customization, click "What am I?" button

### 🛠️ Add Your Own Presets

#### Method 1: Direct Code Modification

1. Create your appearance in game
2. Click game's "Copy" button (JSON copied)
3. Open `PresetData.cs`, add to dictionary:
   ```csharp
   { "Your Preset Name", "Paste JSON" },
   ```
4. Recompile: run `build.bat`

#### Method 2: Python Tool (Recommended)

1. Save preset JSON as `models及其来源表/models/PresetName.json`
2. Edit `models及其来源表/来源表.csv`, add a row
3. Run `python generate_preset_data.py`
4. Copy generated code to `PresetData.cs`
5. Recompile

### 🔧 Development

**Requirements:**
- .NET SDK (.NET Standard 2.1)
- C# Compiler (Visual Studio / Rider / VSCode)
- Escape from Duckov game

**Build:**
1. Edit `MoreAppearancePreset.csproj`, set your game path:
   ```xml
   <DuckovPath>Your_Game_Installation_Path</DuckovPath>
   ```
2. Run `build.bat` (Windows) or `dotnet build -c Debug`
3. Output in `builded/` directory

### 💡 Code Structure

**Core Modules**:
- `ModBehaviour.cs` - Main entry point, detects character customization scene
- `PresetData.cs` - Stores all preset data in dictionary
- `PresetCopier.cs` - Copies game UI and creates preset panel
- `YellowDuckHandler.cs` - Handles preset button creation and configuration
- `ButtonHandler.cs` - Manages button click events
- `PresetViewManager.cs` - Controls preset panel show/hide
- `RandomPresetGenerator.cs` - Generates random presets
- `UIFinder.cs` - Unity UI finder utility
- `TextLocalizerRemover.cs` - Removes game localization components
- `RainbowColorEffect.cs` - Rainbow color effect

### 🤝 Contributing

Issues and Pull Requests are welcome!

Feel free to share your interesting presets!

### 📄 License

[MIT License](LICENSE)

### 🙏 Credits

Thanks to *Escape from Duckov* development team for mod support.

