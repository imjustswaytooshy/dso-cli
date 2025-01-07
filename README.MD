# Psang 🚀

**Psang** is a powerful and versatile command-line tool designed to streamline various system operations and boost your productivity. Whether you need to manage processes, optimize system performance, or simply get help on available commands, Psang has you covered.

![Psang Banner](https://github.com/imjustprism/psang/blob/main/assets/banner.png?raw=true) (dsoon)

## 📖 Table of Contents

- [✨ Features](#-features)
- [💻 Installation](#-installation)
  - [🔧 Prerequisites](#-prerequisites)
  - [⚡ Using PowerShell](#-using-powershell)
  - [🖥️ Using Command Prompt with Curl](#️-using-command-prompt-with-curl)
- [🛠️ Usage](#️-usage)
  - [📜 Available Commands](#-available-commands)
    - [ℹ️ help](#️-help)
    - [🚀 start](#-start)
    - [🔄 restart](#-restart)
    - [🧹 clear](#-clear)
    - [📥 get](#-get)
    - [⚙️ optimize](#️-optimize)
    - [🔧 update](#-update)
    - [🗑️ uninstall](#️-uninstall)
- [🔄 Update Mechanism](#-update-mechanism)
- [🔒 Security Considerations](#-security-considerations)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

## ✨ Features

- **🔧 Process Management**: Start, restart, and terminate processes with ease.
- **⚙️ System Optimization**: Enhance system performance through dedicated commands.
- **ℹ️ Comprehensive Help**: Access detailed information about all available commands.
- **🔄 Automatic Updates**: Seamlessly update Psang to the latest version from GitHub.
- **🛠️ User-Friendly**: Simple and intuitive command syntax for an efficient workflow.

## 💻 Installation

### 🔧 Prerequisites

- **Operating System**: Windows 7 or later.
- **.NET Framework**: Version 4.7.2 or higher.
- **PowerShell**: Version 5.0 or higher.
- **Curl**: Installed and available in your system PATH (optional, for Command Prompt installation).

### ⚡ Using PowerShell

To install Psang using PowerShell, follow these steps:

1. **Open PowerShell as Administrator** 🛡️:

   - Press `Win + X` and select **Windows PowerShell (Admin)**.
   - Alternatively, search for "PowerShell" in the Start menu, right-click on **Windows PowerShell**, and choose **Run as administrator**.

2. **Run the Installation Command** 📥:

   Execute the following command in your PowerShell window:

   ```powershell
   powershell -c "irm https://raw.githubusercontent.com/imjustprism/psang/master/psang-install.ps1 | iex"

To uninstall Psang using PowerShell, follow these steps:

1. **Open PowerShell as Administrator** 🛡️:

   - Press `Win + X` and select **Windows PowerShell (Admin)**.
   - Alternatively, search for "PowerShell" in the Start menu, right-click on **Windows PowerShell**, and choose **Run as administrator**.

2. **Run the Un-installation Command** 📥:

   Execute the following command in your PowerShell window:

   ```powershell
   powershell -c "irm https://raw.githubusercontent.com/imjustprism/psang/master/psang-uninstall.ps1 | iex"
