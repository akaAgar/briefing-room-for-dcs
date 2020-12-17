/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace BriefingRoom4DCSWorld.Debug
{
    /// <summary>
    /// Logs notes, warnings and errors.
    /// </summary>
    public class DebugLog
    {
        /// <summary>
        /// Path to the log file.
        /// </summary>
        private static readonly string LOG_FILE_PATH = $"{BRPaths.ROOT}BriefingRoom.log";

        /// <summary>
        /// Singleton of the library.
        /// </summary>
        public static DebugLog Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new DebugLog();

                return _Instance;
            }
        }

        /// <summary>
        /// Static singleton field.
        /// </summary>
        private static DebugLog _Instance = null;

        /// <summary>
        /// Number of errors logged since the last call to <see cref="Clear"/>.
        /// </summary>
        public int ErrorCount { get { return Errors.Count; } }

        /// <summary>
        /// All errors logged since the last call to <see cref="Clear"/>.
        /// </summary>
        private readonly List<string> Errors = new List<string>();

        /// <summary>
        /// The last logged message.
        /// </summary>
        public string LastMessage { get; private set; } = "";

        /// <summary>
        /// Stream writer tasked with writing to the log file.
        /// </summary>
        private StreamWriter LogFileWriter = null;

        /// <summary>
        /// Number of warnings logged since the last call to <see cref="Clear"/>.
        /// </summary>
        public int WarningCount { get { return Warnings.Count; } }

        /// <summary>
        /// All warnings logged since the last call to <see cref="Clear"/>.
        /// </summary>
        private readonly List<string> Warnings = new List<string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public DebugLog() { }

        /// <summary>
        /// Creates <see cref="LogFileWriter"/> so that from now on the logged messages will be written to the log file.
        /// All previous content of the log will be erased.
        /// </summary>
        /// <returns>True if created successfully, false otherwise</returns>
        public bool CreateLogFileWriter()
        {
            if (LogFileWriter != null) return false; // LogStreamWriter already exist.

            try
            {
                LogFileWriter = File.CreateText(LOG_FILE_PATH);
                WriteLine($"Log file created on {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            }
            catch (Exception)
            {
                WriteLine("Failed to create BriefingRoom.log, make sure you have writing rights to the BriefingRoom directory.");
                LogFileWriter = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes and destroys <see cref="LogFileWriter"/>
        /// </summary>
        /// <returns>True if closed successfully, false otherwise</returns>
        public bool CloseLogFileWriter()
        {
            if (LogFileWriter == null) // The log writer wasn't created yet
                return false;

            try
            {
                LogFileWriter.Flush();
                LogFileWriter.Close();
                LogFileWriter.Dispose();
                LogFileWriter = null;
                WriteLine($"Log writer closed successfully");
            }
            catch (Exception)
            {
                WriteLine("Failed to close log writer BriefingRoom.log.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears all errors, warning and log history.
        /// </summary>
        public void Clear()
        {
            Errors.Clear();
            LastMessage = "";
            Warnings.Clear();
            WriteToFile();
        }

        /// <summary>
        /// Writes a line into the debug log.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="messageLevel">Error level of the message</param>
        public void WriteLine(string message, DebugLogMessageErrorLevel messageLevel)
        {
            WriteLine(message, 0, messageLevel);
        }

        /// <summary>
        /// Writes a line into the debug log.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="indent">Indentation level</param>
        /// <param name="messageLevel">Error level of the message</param>
        public void WriteLine(string message = "", int indent = 0, DebugLogMessageErrorLevel messageLevel = DebugLogMessageErrorLevel.Info)
        {
            message = (message ?? "").Trim();
            LastMessage = message;

            // Add information for warning/error debug level
            switch (messageLevel)
            {
                case DebugLogMessageErrorLevel.Error:
                    Errors.Add(message);
                    message = $"ERROR: {message}";
                    break;
                case DebugLogMessageErrorLevel.Warning:
                    Warnings.Add(message);
                    message = $"WARNING: {message}";
                    break;
            }

            // Add indenting
            message = new string(' ', Math.Max(0, indent * 2)) + message;

            // Write the message to the console...
            Console.WriteLine(message);

            // ...and to the log file
            WriteToFile(message);
        }

        /// <summary>
        /// Write a message to the file using <see cref="LogFileWriter"/>.
        /// Automatically called by <see cref="WriteLine(string, int, DebugLogMessageErrorLevel)"/>, only to be used to write lines to the file but NOT to the debug log.
        /// </summary>
        /// <param name="message">Message to write</param>
        private void WriteToFile(string message = "")
        {
            if (LogFileWriter == null) return; // File writer not ready
         
            LogFileWriter.WriteLine(message);
            LogFileWriter.Flush();
        }

        /// <summary>
        /// Returns an array of all error messages logged since the last call to <see cref="Clear"/>.
        /// </summary>
        /// <returns>An array of string containing all errors</returns>
        public string[] GetErrors()
        {
            return Errors.ToArray();
        }

        /// <summary>
        /// Returns an array of all warning messages logged since the last call to <see cref="Clear"/>.
        /// </summary>
        /// <returns>An array of string containing all warnings</returns>
        public string[] GetWarnings()
        {
            return Warnings.ToArray();
        }
    }
}
