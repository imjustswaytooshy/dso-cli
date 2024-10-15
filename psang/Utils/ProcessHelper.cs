/*
 * Psang, a Drakensang Online Helper Tool
 * Copyright (c) 2024 Prism
 * SPDX-License-Identifier: GPL-3.0-or-later
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Psang.Utils
{
    public static class ProcessHelper
    {
        public static Process GetProcessByName(string[] processNames)
        {
            foreach (var name in processNames)
            {
                var process = Process.GetProcessesByName(name).FirstOrDefault();
                if (process != null)
                {
                    return process;
                }
            }
            return null;
        }

        public static List<Process> GetProcessesByNames(string[] processNames)
        {
            var processes = new List<Process>();
            foreach (var name in processNames)
            {
                processes.AddRange(Process.GetProcessesByName(name));
            }
            return processes;
        }

        public static string GetCommandLine(Process process)
        {
            try
            {
                using (
                    var searcher = new ManagementObjectSearcher(
                        $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"
                    )
                )
                {
                    foreach (ManagementObject @object in searcher.Get())
                    {
                        return @object["CommandLine"]?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (ManagementException mex)
            {
                Console.Error.WriteLine(
                    $"[Error] ManagementException in GetCommandLine for Process ID {process.Id}: {mex.Message}"
                );
            }
            catch (UnauthorizedAccessException uax)
            {
                Console.Error.WriteLine(
                    $"[Error] UnauthorizedAccessException in GetCommandLine for Process ID {process.Id}: {uax.Message}"
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"[Error] Unexpected exception in GetCommandLine for Process ID {process.Id}: {ex.Message}"
                );
            }
            return string.Empty;
        }

        public static void TerminateProcesses(string[] processNames)
        {
            foreach (var name in processNames)
            {
                Process[] processes;
                try
                {
                    processes = Process.GetProcessesByName(name);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(
                        $"Error retrieving processes with name '{name}': {ex.Message}"
                    );
                    continue;
                }

                if (processes.Length == 0)
                {
                    Console.WriteLine($"No processes found with name '{name}'.");
                    continue;
                }

                foreach (var proc in processes)
                {
                    try
                    {
                        proc.Kill();
                        proc.WaitForExit();
                    }
                    catch (InvalidOperationException ioex)
                    {
                        Console.Error.WriteLine(
                            $"Process {proc.ProcessName} (ID: {proc.Id}) has already exited. {ioex.Message}"
                        );
                    }
                    catch (System.ComponentModel.Win32Exception w32ex)
                    {
                        Console.Error.WriteLine(
                            $"Win32Exception while terminating process {proc.ProcessName} (ID: {proc.Id}): {w32ex.Message}"
                        );
                    }
                    catch (NotSupportedException notSupEx)
                    {
                        Console.Error.WriteLine(
                            $"NotSupportedException: Cannot terminate remote process {proc.ProcessName} (ID: {proc.Id}). {notSupEx.Message}"
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(
                            $"Unexpected exception while terminating process {proc.ProcessName} (ID: {proc.Id}): {ex.Message}"
                        );
                    }
                }
            }
        }

        public static void StartProcess(string filePath, string arguments)
        {
            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = arguments,
                        UseShellExecute = false,
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start process: {ex.Message}");
            }
        }
    }
}
