/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Psang.Interfaces;
using Psang.Utils;

namespace Psang.Commands
{
    public class GetCommand : ICommand
    {
        public string Name => "get";
        public string Description => "Fetches game launch arguments.\nOptions:\n  -accid\n";
        public IEnumerable<string> Aliases => new[] { "g" };
        public bool RequiresAdmin => false;

        private readonly string[] validArguments = new[] { "-accid" };

        public void Execute(string[] args)
        {
            var process = ProcessHelper.GetProcessByName(new[] { "dro_client64", "dro_client" });
            if (process == null)
            {
                Console.WriteLine("The game is not running.");
                return;
            }
            var commandLine = ProcessHelper.GetCommandLine(process);
            if (string.IsNullOrEmpty(commandLine))
            {
                Console.WriteLine("Failed to retrieve command line arguments.");
                return;
            }
            if (args.Length == 0)
            {
                FetchAllArguments(commandLine);
            }
            else
            {
                var argument = args[0].ToLower();
                if (validArguments.Contains(argument))
                {
                    FetchSingleArgument(commandLine, argument);
                }
                else
                {
                    Console.WriteLine(
                        $"Invalid argument: {argument}. Use one of the following: {string.Join(", ", validArguments)}"
                    );
                }
            }
        }

        private void FetchAllArguments(string commandLine)
        {
            foreach (var arg in validArguments)
            {
                var value = GetArgumentValue(commandLine, arg);
                if (!string.IsNullOrEmpty(value))
                {
                    Console.WriteLine($"{arg}: {value}");
                }
            }
        }

        private void FetchSingleArgument(string commandLine, string argument)
        {
            var value = GetArgumentValue(commandLine, argument);
            if (!string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"{argument}: {value}");
            }
            else
            {
                Console.WriteLine($"{argument} not found in the game's launch arguments.");
            }
        }

        private string GetArgumentValue(string commandLine, string argument)
        {
            var parts = commandLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                if (
                    parts[i].Equals(argument, StringComparison.OrdinalIgnoreCase)
                    && i + 1 < parts.Length
                )
                {
                    return parts[i + 1];
                }
            }
            return string.Empty;
        }
    }
}
