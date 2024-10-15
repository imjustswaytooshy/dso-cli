/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.IO;
using Psang.Interfaces;
using Psang.Utils;

namespace Psang.Commands
{
    public class RestartCommand : ICommand
    {
        public string Name => "restart";
        public string Description => "Restarts the game.";
        public IEnumerable<string> Aliases => new[] { "r" };
        public bool RequiresAdmin => false;

        private readonly string[] processNames = { "dro_client64", "dro_client", "thinclient" };
        private readonly string tempPath = Path.Combine(Path.GetTempPath(), "DSOClient", "dlcache");
        private readonly string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "psang"
        );
        private readonly string dataFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "psang",
            "config.dat"
        );
        private readonly HashSet<string> excludedArgs = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase
        )
        {
            "-parentwindow",
            "-receiverwindow",
            "-fullscreen",
        };

        public void Execute(string[] args)
        {
            try
            {
                Directory.CreateDirectory(appDataPath);
                var process = ProcessHelper.GetProcessByName(
                    new string[] { "dro_client64", "dro_client" }
                );
                if (process == null)
                {
                    Console.WriteLine("Your game is not running, cannot restart it.");
                    return;
                }
                var commandLine = ProcessHelper.GetCommandLine(process);
                if (string.IsNullOrEmpty(commandLine))
                {
                    Console.WriteLine("Failed to retrieve command line arguments.");
                    return;
                }
                var filteredCommandLine = FilterArguments(commandLine, excludedArgs);
                var encryptedArgs = EncryptionHelper.Encrypt(filteredCommandLine);
                File.WriteAllText(dataFilePath, encryptedArgs);
                ProcessHelper.TerminateProcesses(processNames);
                var decryptedArgs = EncryptionHelper.Decrypt(encryptedArgs);
                string executablePath = GetExecutablePath();
                if (string.IsNullOrEmpty(executablePath))
                {
                    Console.WriteLine("Executable path not found.");
                    return;
                }
                ProcessHelper.StartProcess(executablePath, decryptedArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private string GetExecutablePath()
        {
            string[] possiblePaths =
            {
                Path.Combine(tempPath, "dro_client64.exe"),
                Path.Combine(tempPath, "dro_client.exe"),
            };
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            Console.WriteLine("No executable found in the specified paths.");
            return string.Empty;
        }

        private string FilterArguments(string commandLine, HashSet<string> excludedArgs)
        {
            var args = SplitCommandLine(commandLine);
            var filteredArgs = new List<string>();
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                if (excludedArgs.Contains(arg))
                {
                    if (i + 1 < args.Count && !args[i + 1].StartsWith("-"))
                    {
                        i++;
                    }
                }
                else
                {
                    filteredArgs.Add(arg);
                }
            }
            return string.Join(" ", filteredArgs);
        }

        private List<string> SplitCommandLine(string commandLine)
        {
            var args = new List<string>();
            var currentArg = new System.Text.StringBuilder();
            bool inQuotes = false;
            foreach (var c in commandLine)
            {
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (currentArg.Length > 0)
                    {
                        args.Add(currentArg.ToString());
                        currentArg.Clear();
                    }
                }
                else
                {
                    currentArg.Append(c);
                }
            }
            if (currentArg.Length > 0)
            {
                args.Add(currentArg.ToString());
            }
            return args;
        }
    }
}
