/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System.Collections.Generic;

namespace Psang.Interfaces
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<string> Aliases { get; }
        void Execute(string[] args);
    }
}
