/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Psang.Interfaces;

namespace Psang.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name => "help";
        public string Description => "Displays help information.";
        public IEnumerable<string> Aliases => new[] { "h" };

        private readonly IEnumerable<ICommand> c_commands;

        public HelpCommand(IEnumerable<ICommand> commands)
        {
            c_commands = commands;
        }

        public void Execute(string[] args)
        {
            Console.WriteLine("Usage: psang <command> [options]\n");
            Console.WriteLine("Available commands:");
            int maxCommandLength = c_commands.Max(c =>
            {
                var aliasLength =
                    c.Aliases != null && c.Aliases.Any()
                        ? $" ({string.Join(", ", c.Aliases)})".Length
                        : 0;
                return c.Name.Length + aliasLength;
            });
            foreach (var command in c_commands.Where(c => c.Name != this.Name))
            {
                string aliasPart = "";
                if (command.Aliases != null && command.Aliases.Any())
                {
                    aliasPart = $" ({string.Join(", ", command.Aliases)})";
                }
                string fullCommand = $"{command.Name}{aliasPart}";
                Console.WriteLine(
                    $"  {fullCommand.PadRight(maxCommandLength + 4)}{command.Description}"
                );
            }
        }
    }
}
