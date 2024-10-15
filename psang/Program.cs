/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Psang.Commands;
using Psang.Interfaces;

namespace Psang
{
    class Program
    {
        static void Main(string[] args)
        {
            var commands = new List<ICommand>
            {
                new HelpCommand(null),
                new StartCommand(),
                new RestartCommand(),
                new ClearCommand(),
                new GetCommand(),
                new OptimizeCommand(),
                new UpdateCommand(),
            };

            commands[0] = new HelpCommand(commands);

            var commandDict = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);

            foreach (var cmd in commands)
            {
                if (!string.IsNullOrEmpty(cmd.Name))
                {
                    if (!commandDict.ContainsKey(cmd.Name))
                    {
                        commandDict.Add(cmd.Name, cmd);
                    }

                    foreach (var alias in cmd.Aliases)
                    {
                        if (!commandDict.ContainsKey(alias))
                        {
                            commandDict.Add(alias, cmd);
                        }
                    }
                }
            }

            if (args.Length == 0)
            {
                var helpCommand = commands.FirstOrDefault(c => c.Name == "help");
                helpCommand?.Execute(args);
                return;
            }

            var commandName = args[0].ToLower();
            if (commandDict.TryGetValue(commandName, out var command))
            {
                command.Execute(args.Skip(1).ToArray());
            }
            else
            {
                Console.WriteLine($"Unknown command: {commandName}");
                var helpCommand = commands.FirstOrDefault(c => c.Name == "help");
                helpCommand?.Execute(args);
            }
        }
    }
}
