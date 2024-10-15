/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Psang.Interfaces;
using Psang.Utils;

namespace Psang.Commands
{
    public class UpdateCommand : ICommand
    {
        public string Name => "update";
        public string Description => "Updates psang to the latest version.";
        public IEnumerable<string> Aliases => new[] { "u" };

        public void Execute(string[] args)
        {
            try
            {
                string latestVersion = GitHubHelper.GetLatestVersion();
                string currentVersion = Assembly
                    .GetExecutingAssembly()
                    .GetName()
                    .Version.ToString();

                if (IsNewerVersion(latestVersion, currentVersion))
                {
                    Console.WriteLine($"v{currentVersion} -> v{latestVersion}");

                    string downloadUrl = GitHubHelper.GetLatestDownloadUrl();
                    string tempFile = Path.Combine(Path.GetTempPath(), "psang_new.exe");

                    GitHubHelper.DownloadFile(downloadUrl, tempFile);
                    InstallUpdate(tempFile);

                    Console.WriteLine("Update initiated.");
                }
                else
                {
                    Console.WriteLine("You are already using the latest version.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed: {ex.Message}");
            }
        }

        private bool IsNewerVersion(string latest, string current)
        {
            if (
                Version.TryParse(latest, out var latestV)
                && Version.TryParse(current, out var currentV)
            )
            {
                return latestV > currentV;
            }
            return false;
        }

        private void InstallUpdate(string newExePath)
        {
            string currentExePath = Assembly.GetExecutingAssembly().Location;
            string batchFilePath = Path.Combine(Path.GetTempPath(), "psang_update.bat");

            string batchContent =
                $@"
@echo off
timeout /t 2 /nobreak
copy /Y ""{newExePath}"" ""{currentExePath}""
start """" ""{currentExePath}""
del ""%~f0""
";

            File.WriteAllText(batchFilePath, batchContent);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = batchFilePath,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            Process.Start(psi);
            Environment.Exit(0);
        }
    }
}
