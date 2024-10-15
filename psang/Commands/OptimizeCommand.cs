/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Psang.Interfaces;
using Psang.Utils;

namespace Psang.Commands
{
    public class OptimizeCommand : ICommand
    {
        public string Name => "optimize";
        public string Description => "Optimizes the game for better performance.";
        public IEnumerable<string> Aliases => new[] { "o" };

        private readonly string[] processNames = { "dro_client64", "dro_client" };

        [Flags]
        private enum ThreadAccess : int
        {
            SUSPEND_RESUME = 0x0002,
            SET_PRIORITY = 0x0020,
            QUERY_INFORMATION = 0x0040,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenThread(
            ThreadAccess dwDesiredAccess,
            bool bInheritHandle,
            uint dwThreadId
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        private static extern bool SetThreadPriority(IntPtr hThread, int priority);

        private const int ci_THREAD_PRIORITY_HIGHEST = 2;

        public void Execute(string[] args)
        {
            try
            {
                var processes = ProcessHelper.GetProcessesByNames(processNames);
                if (processes.Count == 0)
                {
                    Console.WriteLine("The game is not running.");
                    return;
                }

                foreach (var process in processes)
                {
                    if (process.HasExited)
                    {
                        Console.WriteLine($"- Process {process.ProcessName} has already exited.");
                        continue;
                    }

                    if (!process.Responding)
                    {
                        Console.WriteLine($"- Process {process.ProcessName} is not responding.");
                        continue;
                    }

                    SetProcessorAffinity(process);
                    SetProcessPriority(process, ProcessPriorityClass.High);
                    EnablePriorityBoost(process);
                    OptimizeMemoryUsage(process);
                    OptimizeThreadPriorities(process);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(
                    "Insufficient permissions to modify the process. Please run as administrator."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during optimization: {ex.Message}");
            }
        }

        private void SetProcessorAffinity(Process process)
        {
            try
            {
                int processorCount = Environment.ProcessorCount;
                long affinityMask = (1L << processorCount) - 1;
                process.ProcessorAffinity = (IntPtr)affinityMask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"- Failed to set processor affinity: {ex.Message}");
            }
        }

        private void SetProcessPriority(Process process, ProcessPriorityClass priority)
        {
            try
            {
                process.PriorityClass = priority;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"- Failed to set process priority: {ex.Message}");
            }
        }

        private void EnablePriorityBoost(Process process)
        {
            try
            {
                process.PriorityBoostEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"- Failed to enable priority boost: {ex.Message}");
            }
        }

        private void OptimizeMemoryUsage(Process process)
        {
            try
            {
                process.MinWorkingSet = new IntPtr(50_000_000); // 50 MB
                process.MaxWorkingSet = new IntPtr(500_000_000); // 500 MB
            }
            catch (Exception ex)
            {
                Console.WriteLine($"- Failed to optimize memory usage: {ex.Message}");
            }
        }

        private void OptimizeThreadPriorities(Process process)
        {
            try
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr pThread = OpenThread(ThreadAccess.SET_PRIORITY, false, (uint)thread.Id);
                    if (pThread == IntPtr.Zero)
                    {
                        Console.WriteLine($"- Failed to open thread ID: {thread.Id}");
                        continue;
                    }

                    bool success = SetThreadPriority(pThread, ci_THREAD_PRIORITY_HIGHEST);
                    if (success) { }
                    else
                    {
                        Console.WriteLine($"- Failed to set priority for thread ID: {thread.Id}");
                    }

                    CloseHandle(pThread);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"- Failed to optimize thread priorities: {ex.Message}");
            }
        }
    }
}
