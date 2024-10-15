/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Psang.Interfaces;

namespace Psang.Commands
{
    public class ClearCommand : ICommand
    {
        public string Name => "clear";
        public string Description =>
            "Clears the game caches and fix potential issues.\n"
            + "Options:\n"
            + "  -q  Only delete specific file types (txt, xml, ini, localstorage, log)\n"
            + "  -s  Keep the settings and delete the rest.\n";
        public IEnumerable<string> Aliases => new[] { "c" };

        private readonly string dsoclientPath = Path.Combine(Path.GetTempPath(), "DSOClient");

        public void Execute(string[] args)
        {
            if (!Directory.Exists(dsoclientPath))
            {
                Console.WriteLine("The DSOClient folder does not exist.");
                return;
            }

            bool quickMode = args.Contains("-q");
            bool saveSettings = args.Contains("-s");

            try
            {
                if (quickMode)
                {
                    DeleteSpecificFiles();
                }
                else
                {
                    if (saveSettings)
                    {
                        DeleteExceptSettings();
                    }
                    else
                    {
                        DeleteAllFiles();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void DeleteAllFiles()
        {
            var files = Directory.GetFiles(dsoclientPath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            var directories = Directory.GetDirectories(dsoclientPath);
            foreach (var dir in directories)
            {
                Directory.Delete(dir, true);
            }
        }

        private void DeleteSpecificFiles()
        {
            var extensions = new[] { ".txt", ".xml", ".ini", ".localstorage", ".log" };
            var files = Directory
                .GetFiles(dsoclientPath, "*.*", SearchOption.AllDirectories)
                .Where(f =>
                    extensions.Contains(Path.GetExtension(f)) && !f.EndsWith("settings.xml")
                );

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private void DeleteExceptSettings()
        {
            var files = Directory
                .GetFiles(dsoclientPath, "*", SearchOption.AllDirectories)
                .Where(f => !f.EndsWith("settings.xml"));

            foreach (var file in files)
            {
                File.Delete(file);
            }

            var directories = Directory.GetDirectories(dsoclientPath);
            foreach (var dir in directories)
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
